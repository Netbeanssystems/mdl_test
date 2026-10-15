using Application.Dtos;
using Application.Helpers;
using Application.ServiceInterfaces;
using AutoMapper;
using Domain.Models;
using Domain.RepositoryInterfaces;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Services
{
    public class VigilanceService : IVigilanceFormService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public VigilanceService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // ═══════════════════════════════════════════════════════════
        //  CREATE — OTP gate + DB rate limit + DB save
        // ═══════════════════════════════════════════════════════════
        public async Task<VigilanceFormDTO> Create(VigilanceFormDTO argModelDto)
        {
            if (argModelDto == null) return null;

            // ── OTP verification gate ──
            var rec = VigilanceOtpStore.Get(argModelDto.Email);
            if (rec == null || !rec.IsVerified) return null;
            if (VigilanceOtpStore.IsVerificationExpired(rec)) return null;

            // ── 24 hr rate limit — DB se check ──
            var isRateLimited = await _unitOfWork.vigiRepo
                .HasRecentComplaintByEmail(argModelDto.Email, VigilanceOtpStore.RATE_LIMIT_HOURS)
                .ConfigureAwait(false);

            if (isRateLimited) return null;

            var model = _mapper.Map<VigilanceForm>(argModelDto);
            _unitOfWork.vigiRepo.Create(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            argModelDto.Id = model.Id;

            // ── Mark submitted (in-memory, UI ke liye) ──
            rec.LastSubmittedAt = DateTime.UtcNow;
            rec.ComplaintCount++;
            rec.IsVerified = false;
            rec.OtpCode = null;
            rec.VerifiedAt = null;
            VigilanceOtpStore.Save(rec);

            return rowsChanged > 0 ? argModelDto : null;
        }

        // ═══════════════════════════════════════════════════════════
        //  SUBMIT EMAIL — Validate + rate limit + OTP generate
        //  ✅ OTP return karo. Email NAHI bhejna. WebSite bhejega.
        // ═══════════════════════════════════════════════════════════
        public async Task<(bool success, string message, string otp, int? resendLeft)> SubmitEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return (false, "Email is required.", null, null);

            email = email.Trim().ToLower();

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return (false, "Invalid email format.", null, null);

            var rec = VigilanceOtpStore.GetOrCreate(email);
            if (rec == null)
                return (false, "Unable to process email.", null, null);

            // ── 24 hr rate limit — DB se check ──
            var isRateLimited = await _unitOfWork.vigiRepo
                .HasRecentComplaintByEmail(email, VigilanceOtpStore.RATE_LIMIT_HOURS)
                .ConfigureAwait(false);

            if (isRateLimited)
                return (false, "ALREADY_SUBMITTED", null, null);

            // Max resend
            if (rec.ResendCount >= VigilanceOtpStore.MAX_RESEND)
                return (false,
                    $"Maximum OTP resend limit ({VigilanceOtpStore.MAX_RESEND}) reached. Please try after some time.",
                    null, null);

            // Cooldown
            if (rec.LastResendAt != null &&
                (DateTime.UtcNow - rec.LastResendAt.Value).TotalSeconds < VigilanceOtpStore.RESEND_COOLDOWN_SECONDS)
            {
                var secLeft = VigilanceOtpStore.RESEND_COOLDOWN_SECONDS
                              - (int)(DateTime.UtcNow - rec.LastResendAt.Value).TotalSeconds;
                return (false, $"Please wait {secLeft} second(s) before requesting a new OTP.", null, null);
            }

            // Generate + save OTP
            var otp = VigilanceOtpStore.GenerateOtp();
            rec.OtpCode = otp;
            rec.OtpCreatedAt = DateTime.UtcNow;
            rec.OtpAttempts = 0;
            rec.ResendCount++;
            rec.LastResendAt = DateTime.UtcNow;
            rec.IsVerified = false;
            VigilanceOtpStore.Save(rec);

            // ✅ OTP return — WebSite email bhejega
            return (true,
                $"OTP generated successfully. Valid for {VigilanceOtpStore.OTP_VALIDITY_MINUTES} minutes.",
                otp,
                VigilanceOtpStore.MAX_RESEND - rec.ResendCount);
        }

        // ═══════════════════════════════════════════════════════════
        //  VERIFY OTP — match + attempts + expiry
        // ═══════════════════════════════════════════════════════════
        public async Task<(bool success, string message, int? attemptsLeft)> VerifyOtp(string email, string otp)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(otp))
                return (false, "Email and OTP are required.", null);

            email = email.Trim().ToLower();
            var rec = VigilanceOtpStore.Get(email);

            if (rec == null || string.IsNullOrEmpty(rec.OtpCode))
                return (false, "No OTP request found. Please request a new OTP.", null);

            if (VigilanceOtpStore.IsOtpExpired(rec))
                return (false, "OTP expired. Please request a new one.", null);

            if (rec.OtpAttempts >= VigilanceOtpStore.MAX_VERIFY_ATTEMPTS)
                return (false, "Maximum verification attempts reached. Please request a new OTP.", 0);

            if (!string.Equals(rec.OtpCode, otp.Trim(), StringComparison.Ordinal))
            {
                rec.OtpAttempts++;
                VigilanceOtpStore.Save(rec);
                var left = VigilanceOtpStore.MAX_VERIFY_ATTEMPTS - rec.OtpAttempts;
                return (false, $"Invalid OTP. {left} attempt(s) remaining.", left);
            }

            rec.IsVerified = true;
            rec.VerifiedAt = DateTime.UtcNow;
            rec.OtpAttempts = 0;
            VigilanceOtpStore.Save(rec);

            return (true, "Email verified successfully.", null);
        }

        // ═══════════════════════════════════════════════════════════
        //  RESEND OTP — regenerate + return, no email
        // ═══════════════════════════════════════════════════════════
        public async Task<(bool success, string message, string otp, int? resendLeft)> ResendOtp(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return (false, "Email is required.", null, null);

            email = email.Trim().ToLower();
            var rec = VigilanceOtpStore.Get(email);

            if (rec == null)
                return (false, "No OTP request found. Please submit email first.", null, null);

            if (rec.IsVerified)
                return (false, "Email already verified. Please proceed with the form.", null, null);

            // ── 24 hr rate limit ──
            var isRateLimited = await _unitOfWork.vigiRepo
                .HasRecentComplaintByEmail(email, VigilanceOtpStore.RATE_LIMIT_HOURS)
                .ConfigureAwait(false);

            if (isRateLimited)
                return (false, "ALREADY_SUBMITTED", null, null);

            if (rec.ResendCount >= VigilanceOtpStore.MAX_RESEND)
                return (false,
                    $"Maximum OTP resend limit ({VigilanceOtpStore.MAX_RESEND}) reached.",
                    null, 0);

            if (rec.LastResendAt != null &&
                (DateTime.UtcNow - rec.LastResendAt.Value).TotalSeconds < VigilanceOtpStore.RESEND_COOLDOWN_SECONDS)
            {
                var secLeft = VigilanceOtpStore.RESEND_COOLDOWN_SECONDS
                              - (int)(DateTime.UtcNow - rec.LastResendAt.Value).TotalSeconds;
                return (false, $"Please wait {secLeft} second(s) before resending.", null, null);
            }

            var otp = VigilanceOtpStore.GenerateOtp();
            rec.OtpCode = otp;
            rec.OtpCreatedAt = DateTime.UtcNow;
            rec.OtpAttempts = 0;
            rec.ResendCount++;
            rec.LastResendAt = DateTime.UtcNow;
            VigilanceOtpStore.Save(rec);

            // ✅ OTP return — WebSite email bhejega
            return (true,
                $"New OTP generated successfully.",
                otp,
                VigilanceOtpStore.MAX_RESEND - rec.ResendCount);
        }
    }
}