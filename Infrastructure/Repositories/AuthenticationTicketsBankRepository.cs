using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class AuthenticationTicketsBankRepository : Repository<AuthenticationTicketsBank>, IAuthenticationTicketsBankRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public AuthenticationTicketsBankRepository(AppDbContext context) : base(context)
        {
        }
        public Task<AuthenticationTicketsBank> GetByIdStr(string Id)
        {
            return DbContext.AuthenticationTicketsBank.FirstOrDefaultAsync(a => a.Id == Id);
        }
        public Task<AuthenticationTicketsBank> GetByUserId(string UserId)
        {
            return DbContext.AuthenticationTicketsBank.FirstOrDefaultAsync(a => a.UserId == UserId);
        }
    }
}
