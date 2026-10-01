using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
namespace Infrastructure.Repositories
{
    public class WebSiteLogsRepository : Repository<WebSiteLogs>, IWebSiteLogsRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public WebSiteLogsRepository(AppDbContext context) : base(context)
        {
        }
    }
}
