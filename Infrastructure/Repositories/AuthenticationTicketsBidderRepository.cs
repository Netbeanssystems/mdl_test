using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class AuthenticationTicketsBidderRepository : Repository<AuthenticationTicketsBidder>, IAuthenticationTicketsBidderRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public AuthenticationTicketsBidderRepository(AppDbContext context) : base(context)
        {
        }
        public Task<AuthenticationTicketsBidder> GetByIdStr(string Id)
        {
            return DbContext.AuthenticationTicketsBidder.FirstOrDefaultAsync(a => a.Id == Id);
        }
        public Task<AuthenticationTicketsBidder> GetByUserId(string UserId)
        {
            return DbContext.AuthenticationTicketsBidder.FirstOrDefaultAsync(a => a.UserId == UserId);
        }
    }
}
