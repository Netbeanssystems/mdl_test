using Application.AppSettings;
using Application.Dtos;
using Application.ServiceInterfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
namespace Application.Services
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _jwtOptions;
        public JwtService(IOptions<JwtSettings> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
            ThrowIfInvalidOptions(_jwtOptions);
        }
        public async Task<JwtSecurityToken> GenerateEncodedToken(ApplicationUser User, ApplicationRole Role)
        {
            //create role claims
            var roleClaims = await GenerateClaims(User, Role).ConfigureAwait(false);
            var identity = new ClaimsIdentity(new GenericIdentity(User.UserName, "Token"), roleClaims);
            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken(
                _jwtOptions.Issuer,
                _jwtOptions.Audience,
                identity.Claims,
                _jwtOptions.NotBefore,
                _jwtOptions.AccessTokenExpiresUtc,
                _jwtOptions.SigningCredentials);
            return jwt;
        }
        public string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
            //var randomNumber = new byte[32];
            //using (var rng = RandomNumberGenerator.Create())
            //{
            //    rng.GetBytes(randomNumber);
            //    return Convert.ToBase64String(randomNumber);
            //}
        }
        private async Task<IEnumerable<Claim>> GenerateClaims(ApplicationUser user, ApplicationRole Role)
        {
            var rolepermissions = new Dictionary<string, bool>
            {
                {"CanView", Role.CanView},
                {"CanCreate", Role.CanCreate},
                {"CanEdit", Role.CanEdit},
                {"CanDelete", Role.CanDelete}
            };
            var per = JsonConvert.SerializeObject(rolepermissions);
            var claims = new List<Claim>
            {
                new Claim("uid", user.Id),
                new Claim("eml", user.Email),
                new Claim("phn", user.PhoneNumber),
                new Claim("img", user.ProfileImage),
                new Claim("chn", user.ChangePassword?"Y":"N"),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator().ConfigureAwait(false)),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
                new Claim(ClaimTypes.Role, Role.Name),
                new Claim("per", per),
                !string.IsNullOrEmpty(user.Name) ? new Claim("nam", user.Name) : new Claim("nam", string.Empty),
                !string.IsNullOrEmpty(user.BranchName) ? new Claim("bnam", user.BranchName) : new Claim("bnam", string.Empty),
                new Claim("proj", !string.IsNullOrEmpty(user.ProjectId) ? user.ProjectId : string.Empty)
            };
            return claims;
        }
        private static long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() -
                               new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                              .TotalSeconds);
        private static void ThrowIfInvalidOptions(JwtSettings options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (options.AccessTokenExpiry <= 0)
            {
                throw new ArgumentException("Must be a non-zero number.", nameof(JwtSettings.AccessTokenExpiry));
            }
            if (options.RefreshTokenExpiry <= 0)
            {
                throw new ArgumentException("Must be a non-zero number.", nameof(JwtSettings.RefreshTokenExpiry));
            }
            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtSettings.SigningCredentials));
            }
            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtSettings.JtiGenerator));
            }
        }
    }
}