using Application.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
namespace Application.ServiceInterfaces
{
    public interface IJwtService
    {
        Task<JwtSecurityToken> GenerateEncodedToken(ApplicationUser User, ApplicationRole Role);
        string GenerateRefreshToken();
    }
}