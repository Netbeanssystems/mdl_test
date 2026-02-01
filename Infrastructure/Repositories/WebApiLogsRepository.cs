using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
namespace Infrastructure.Repositories
{
    public class WebApiLogsRepository : Repository<WebApiLogs>, IWebApiLogsRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public WebApiLogsRepository(AppDbContext context) : base(context)
        {
        }
    }
}
