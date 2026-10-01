using Application.Dtos;
using Application.Helpers;
using Application.ServiceInterfaces;
using AutoMapper;
using Domain.Models;
using Domain.RepositoryInterfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Services
{
    public class VigilanceService : IVigilanceFormService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public VigilanceService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _config = config;
        }

        // ═══════════════════════════════════════════════════════════
        //  EXISTING: Create (OTP gate + DB rate limit)
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
        //  NEW #1: SubmitEmail → Send OTP
        // ═══════════════════════════════════════════════════════════
        public async Task<(bool success, string message, int? resendLeft)> SubmitEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return (false, "Email is required.", null);

            email = email.Trim().ToLower();

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return (false, "Invalid email format.", null);

            var rec = VigilanceOtpStore.GetOrCreate(email);
            if (rec == null)
                return (false, "Unable to process email.", null);

            // ── 24 hr rate limit — DB se check (persistent) ──
            var isRateLimited = await _unitOfWork.vigiRepo
                .HasRecentComplaintByEmail(email, VigilanceOtpStore.RATE_LIMIT_HOURS)
                .ConfigureAwait(false);

            if (isRateLimited)
                return (false, "ALREADY_SUBMITTED", null);

            // Max resend
            if (rec.ResendCount >= VigilanceOtpStore.MAX_RESEND)
                return (false,
                    $"Maximum OTP resend limit ({VigilanceOtpStore.MAX_RESEND}) reached. Please try after some time.",
                    null);

            // Cooldown
            if (rec.LastResendAt != null &&
                (DateTime.UtcNow - rec.LastResendAt.Value).TotalSeconds < VigilanceOtpStore.RESEND_COOLDOWN_SECONDS)
            {
                var secLeft = VigilanceOtpStore.RESEND_COOLDOWN_SECONDS
                              - (int)(DateTime.UtcNow - rec.LastResendAt.Value).TotalSeconds;
                return (false, $"Please wait {secLeft} second(s) before requesting a new OTP.", null);
            }

            // Generate + save
            var otp = VigilanceOtpStore.GenerateOtp();
            rec.OtpCode = otp;
            rec.OtpCreatedAt = DateTime.UtcNow;
            rec.OtpAttempts = 0;
            rec.ResendCount++;
            rec.LastResendAt = DateTime.UtcNow;
            rec.IsVerified = false;
            VigilanceOtpStore.Save(rec);

            // Send email
            var sent = VigilanceOtpStore.SendOtpEmail(email, otp, _config);
            if (!sent)
                return (false, "Failed to send OTP email. Please try again.", null);

            return (true,
                $"OTP sent to {MaskEmail(email)}. Valid for {VigilanceOtpStore.OTP_VALIDITY_MINUTES} minutes.",
                VigilanceOtpStore.MAX_RESEND - rec.ResendCount);
        }

        // ═══════════════════════════════════════════════════════════
        //  NEW #2: VerifyOtp
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
        //  NEW #3: ResendOtp
        // ═══════════════════════════════════════════════════════════
        public async Task<(bool success, string message, int? resendLeft)> ResendOtp(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return (false, "Email is required.", null);

            email = email.Trim().ToLower();
            var rec = VigilanceOtpStore.Get(email);

            if (rec == null)
                return (false, "No OTP request found. Please submit email first.", null);

            if (rec.IsVerified)
                return (false, "Email already verified. Please proceed with the form.", null);

            // ── 24 hr rate limit — DB se check (persistent) ──
            var isRateLimited = await _unitOfWork.vigiRepo
                .HasRecentComplaintByEmail(email, VigilanceOtpStore.RATE_LIMIT_HOURS)
                .ConfigureAwait(false);

            if (isRateLimited)
                return (false, "ALREADY_SUBMITTED", null);

            if (rec.ResendCount >= VigilanceOtpStore.MAX_RESEND)
                return (false,
                    $"Maximum OTP resend limit ({VigilanceOtpStore.MAX_RESEND}) reached.",
                    0);

            if (rec.LastResendAt != null &&
                (DateTime.UtcNow - rec.LastResendAt.Value).TotalSeconds < VigilanceOtpStore.RESEND_COOLDOWN_SECONDS)
            {
                var secLeft = VigilanceOtpStore.RESEND_COOLDOWN_SECONDS
                              - (int)(DateTime.UtcNow - rec.LastResendAt.Value).TotalSeconds;
                return (false, $"Please wait {secLeft} second(s) before resending.", null);
            }

            var otp = VigilanceOtpStore.GenerateOtp();
            rec.OtpCode = otp;
            rec.OtpCreatedAt = DateTime.UtcNow;
            rec.OtpAttempts = 0;
            rec.ResendCount++;
            rec.LastResendAt = DateTime.UtcNow;
            VigilanceOtpStore.Save(rec);

            var sent = VigilanceOtpStore.SendOtpEmail(email, otp, _config);
            if (!sent)
                return (false, "Failed to send OTP email.", null);

            return (true,
                $"New OTP sent to {MaskEmail(email)}.",
                VigilanceOtpStore.MAX_RESEND - rec.ResendCount);
        }

        // ── Helper ──
        private static string MaskEmail(string email)
        {
            if (string.IsNullOrEmpty(email) || !email.Contains("@")) return email;
            var parts = email.Split('@');
            var name = parts[0];
            if (name.Length <= 2) return email;
            return name.Substring(0, 2) + new string('*', Math.Max(1, name.Length - 2)) + "@" + parts[1];
        }
    }
}