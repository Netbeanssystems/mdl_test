using Domain.Models;
using System.Threading.Tasks;
namespace Domain.RepositoryInterfaces
{
    public interface IRefreshTokensRepository : IRepository<RefreshToken>
    {
        Task<RefreshToken> GetRefreshTokenByUserId(string userid);
        Task<RefreshToken> GetRefreshToken(string token);
    }
}
