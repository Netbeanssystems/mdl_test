using System;
namespace Domain.Models
{
    public class RefreshToken
    {
        public string Id { get; set; }
        public string Token { get; set; }
        public DateTime IssuedUtc { get; set; }
        public DateTime ExpiresUtc { get; set; }
        public string UserId { get; set; }
    }
}
