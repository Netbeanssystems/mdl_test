using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;

namespace Infrastructure.Repositories
{
    internal class WebsiteCounterRepository : Repository<WebSiteCounters>, IWebsiteCounterRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public WebsiteCounterRepository(AppDbContext context) : base(context)
        {
        }
    }
}
