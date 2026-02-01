using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class AuthenticationTicketsRepository : Repository<AuthenticationTickets>, IAuthenticationTicketsRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public AuthenticationTicketsRepository(AppDbContext context) : base(context)
        {
        }
        public Task<AuthenticationTickets> GetByIdStr(string Id)
        {
            return DbContext.AuthenticationTickets.FirstOrDefaultAsync(a => a.Id == Id);
        }
        public Task<AuthenticationTickets> GetByUserId(string UserId)
        {
            return DbContext.AuthenticationTickets.FirstOrDefaultAsync(a => a.UserId == UserId);
        }
    }
}
