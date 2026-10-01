// Application/Helpers/VigilanceOtpStore.cs
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Net.Mail;

namespace Application.Helpers
{
    public static class VigilanceOtpStore
    {
        public class OtpRecord
        {
            public string Email { get; set; }
            public string OtpCode { get; set; }
            public DateTime? OtpCreatedAt { get; set; }
            public int OtpAttempts { get; set; }
            public int ResendCount { get; set; }
            public DateTime? LastResendAt { get; set; }
            public bool IsVerified { get; set; }
            public DateTime? VerifiedAt { get; set; }
            public DateTime? LastSubmittedAt { get; set; }
            public int ComplaintCount { get; set; }
        }

        // ── Config ──
        public const int OTP_VALIDITY_MINUTES = 10;
        public const int MAX_RESEND = 3;
        public const int MAX_VERIFY_ATTEMPTS = 3;
        public const int RESEND_COOLDOWN_SECONDS = 60;
        public const int VERIFIED_VALIDITY_MINUTES = 15;
        public const int RATE_LIMIT_HOURS = 24;

        // Key = email (lowercase)
        private static readonly ConcurrentDictionary<string, OtpRecord> _store
            = new ConcurrentDictionary<string, OtpRecord>();

        // ═══════════════════════════════════════════════════════════
        //  GET OR CREATE
        // ═══════════════════════════════════════════════════════════
        public static OtpRecord GetOrCreate(string email)
        {
            email = email?.Trim().ToLower();
            if (string.IsNullOrEmpty(email)) return null;

            return _store.GetOrAdd(email, e => new OtpRecord
            {
                Email = e,
                OtpAttempts = 0,
                ResendCount = 0,
                IsVerified = false
            });
        }

        // ═══════════════════════════════════════════════════════════
        //  GET
        // ═══════════════════════════════════════════════════════════
        public static OtpRecord Get(string email)
        {
            email = email?.Trim().ToLower();
            if (string.IsNullOrEmpty(email)) return null;

            _store.TryGetValue(email, out var rec);
            return rec;
        }

        // ═══════════════════════════════════════════════════════════
        //  SAVE
        // ═══════════════════════════════════════════════════════════
        public static void Save(OtpRecord rec)
        {
            if (rec == null || string.IsNullOrEmpty(rec.Email)) return;
            _store[rec.Email.Trim().ToLower()] = rec;
        }

        // ═══════════════════════════════════════════════════════════
        //  RATE LIMIT CHECK (24 hr)
        // ═══════════════════════════════════════════════════════════
        public static bool IsRateLimited(OtpRecord rec)
        {
            if (rec?.LastSubmittedAt == null) return false;
            return (DateTime.UtcNow - rec.LastSubmittedAt.Value).TotalHours < RATE_LIMIT_HOURS;
        }

        // ═══════════════════════════════════════════════════════════
        //  HOURS LEFT IN RATE LIMIT
        // ═══════════════════════════════════════════════════════════
        public static double HoursLeft(OtpRecord rec)
        {
            if (rec?.LastSubmittedAt == null) return 0;

            var hrs = RATE_LIMIT_HOURS - (DateTime.UtcNow - rec.LastSubmittedAt.Value).TotalHours;
            return hrs > 0 ? hrs : 0;
        }

        // ═══════════════════════════════════════════════════════════
        //  OTP EXPIRY CHECK (10 min)
        // ═══════════════════════════════════════════════════════════
        public static bool IsOtpExpired(OtpRecord rec)
        {
            if (rec?.OtpCreatedAt == null) return true;
            return (DateTime.UtcNow - rec.OtpCreatedAt.Value).TotalMinutes > OTP_VALIDITY_MINUTES;
        }

        // ═══════════════════════════════════════════════════════════
        //  VERIFICATION EXPIRY CHECK (15 min)
        // ═══════════════════════════════════════════════════════════
        public static bool IsVerificationExpired(OtpRecord rec)
        {
            if (rec?.VerifiedAt == null) return true;
            return (DateTime.UtcNow - rec.VerifiedAt.Value).TotalMinutes > VERIFIED_VALIDITY_MINUTES;
        }

        // ═══════════════════════════════════════════════════════════
        //  GENERATE OTP
        // ═══════════════════════════════════════════════════════════
        public static string GenerateOtp()
        {
            return new Random().Next(100000, 999999).ToString();
        }

        // ═══════════════════════════════════════════════════════════
        //  SEND OTP EMAIL
        //  ✅ Uses EmailSettings from appsettings.json
        // ═══════════════════════════════════════════════════════════
        public static bool SendOtpEmail(string toEmail, string otp, IConfiguration config)
        {
            try
            {
                var fromAddress = config["EmailSettings:FromAddress"];
                var displayName = config["EmailSettings:DisplayName"];
                var host = config["EmailSettings:Host"];
                var portStr = config["EmailSettings:Port"];
                var userName = config["EmailSettings:UserName"];
                var password = config["EmailSettings:Password"];

                System.Diagnostics.Debug.WriteLine($"📧 Config check:");
                System.Diagnostics.Debug.WriteLine($"   FromAddress: {fromAddress}");
                System.Diagnostics.Debug.WriteLine($"   Host: {host}");
                System.Diagnostics.Debug.WriteLine($"   Port: {portStr}");

                if (string.IsNullOrEmpty(fromAddress) || string.IsNullOrEmpty(host))
                {
                    System.Diagnostics.Debug.WriteLine("❌ Email config missing");
                    return false;
                }

                int port = int.TryParse(portStr, out var p) ? p : 587;

                string body = $@"
                    <html><body style='font-family:Verdana,Arial,sans-serif;'>
                    <p>Dear User,</p>
                    <p>Your OTP for <b>Vigilance Complaint Registration</b> on MDL Website is:</p>
                    <h2 style='color:#00008A; letter-spacing:4px;'>{otp}</h2>
                    <p>This OTP is valid for <b>{OTP_VALIDITY_MINUTES} minutes</b>.</p>
                    <p>If you did not request this, please ignore this email.</p>
                    <br/>
                    <p>Regards,<br/>MDL Vigilance Team</p>
                    </body></html>";

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(fromAddress, displayName);
                mail.To.Add(new MailAddress(toEmail));
                mail.IsBodyHtml = true;
                mail.Subject = "MDL Vigilance - Email Verification OTP";
                mail.Body = body;

                SmtpClient smtp = new SmtpClient
                {
                    Host = host,
                    Port = port,
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new System.Net.NetworkCredential(userName, password)
                };

                smtp.Send(mail);

                System.Diagnostics.Debug.WriteLine("✅ Email sent successfully");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("═══════ EMAIL FAILED ═══════");
                System.Diagnostics.Debug.WriteLine("Message: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Inner: " + ex.InnerException?.Message);
                System.Diagnostics.Debug.WriteLine("StackTrace: " + ex.StackTrace);
                System.Diagnostics.Debug.WriteLine("═══════════════════════════");
                return false;
            }
        }
    }
}