using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
namespace Infrastructure.Repositories
{
    public class WebAppLogsRepository : Repository<WebAppLogs>, IWebAppLogsRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public WebAppLogsRepository(AppDbContext context) : base(context)
        {
        }
    }
}
