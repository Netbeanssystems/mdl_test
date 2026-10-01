using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class SiteAuthenticationTicketsRepository : Repository<SiteAuthenticationTickets>, ISiteAuthenticationTicketsRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public SiteAuthenticationTicketsRepository(AppDbContext context) : base(context)
        {
        }
        public Task<SiteAuthenticationTickets> GetByIdStr(string Id)
        {
            return DbContext.SiteAuthenticationTickets.FirstOrDefaultAsync(a => a.Id == Id);
        }
        public Task<SiteAuthenticationTickets> GetByUserId(string UserId)
        {
            return DbContext.SiteAuthenticationTickets.FirstOrDefaultAsync(a => a.UserId == UserId);
        }
    }
}
