using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
namespace Infrastructure.Repositories
{
    public class RefreshTokensRepository : Repository<RefreshToken>, IRefreshTokensRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public RefreshTokensRepository(AppDbContext context) : base(context)
        {
        }
        public Task<RefreshToken> GetRefreshTokenByUserId(string userid)
        {
            return DbContext.RefreshTokens
            .FirstOrDefaultAsync(a => a.UserId == userid);
        }
        public Task<RefreshToken> GetRefreshToken(string token)
        {
            return DbContext.RefreshTokens
                    //.Include(x => x.User)
                    .SingleOrDefaultAsync(i => i.Token == token);
        }
    }
}
