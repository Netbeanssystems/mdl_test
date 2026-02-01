using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
namespace Infrastructure.Repositories
{
    public class WebBankLogsRepository : Repository<WebBankLogs>, IWebBankLogsRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public WebBankLogsRepository(AppDbContext context) : base(context)
        {
        }
    }
}
