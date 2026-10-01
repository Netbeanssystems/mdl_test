using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;

namespace Infrastructure.Repositories
{
    public class LoginLogsRepository : Repository<LoginLogs>, ILoginLogsRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public LoginLogsRepository(AppDbContext context) : base(context)
        {
        }
    }
}
