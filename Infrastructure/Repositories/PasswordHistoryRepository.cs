using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class PasswordHistoryRepository : Repository<PasswordHistory>, IPasswordHistoryRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public PasswordHistoryRepository(AppDbContext context) : base(context)
        {
        }
        public Task<PasswordHistory> GetByUsername(string username)
        {
            return DbContext.PasswordHistory.Where(x => x.Username == username).FirstOrDefaultAsync();
        }

        public Task<PasswordHistory> GetByUsernamePwd(string username, string pwd)
        {
            return DbContext.PasswordHistory.Where(x => x.Username == username && ((x.Pwd1 == pwd) || (x.Pwd2 == pwd) || (x.Pwd3 == pwd))).FirstOrDefaultAsync();
        }
    }
}
