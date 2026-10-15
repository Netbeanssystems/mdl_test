// Application/Helpers/VigilanceOtpStore.cs
using System;
using System.Collections.Concurrent;

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
        //  ❌ SendOtpEmail() METHOD REMOVED
        //  Ab email bhejne ka kaam sirf WebSite (RegisterComplaintModel) karega.
        //  Koi EmailSettings dependency nahi.
        // ═══════════════════════════════════════════════════════════
    }
}