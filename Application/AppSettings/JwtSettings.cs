using Microsoft.IdentityModel.Tokens;
using System;
using System.Threading.Tasks;
namespace Application.AppSettings
{
    public class JwtSettings
    {
        //public string Subject { get; set; }
        public string Issuer { get; set; } //setable from appsettings.json
        public string Audience { get; set; } //setable from appsettings.json
        public int AccessTokenExpiry { get; set; }//setable from appsettings.json
        public int RefreshTokenExpiry { get; set; }//setable from appsettings.json
        public DateTime AccessTokenExpiresUtc => IssuedAt.Add(TimeSpan.FromMinutes(AccessTokenExpiry));
        public DateTime RefreshTokenExpiresUtc => IssuedAt.Add(TimeSpan.FromMinutes(RefreshTokenExpiry));
        public DateTime NotBefore => DateTime.UtcNow;
        public DateTime IssuedAt => DateTime.UtcNow;
        public Func<Task<string>> JtiGenerator => () => Task.FromResult(Guid.NewGuid().ToString());
        public SigningCredentials SigningCredentials { get; set; } //setable from appsettings.json
    }
}