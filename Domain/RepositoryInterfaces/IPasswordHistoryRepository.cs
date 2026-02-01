using Domain.Models;
using System.Threading.Tasks;

namespace Domain.RepositoryInterfaces
{
    public interface IPasswordHistoryRepository : IRepository<PasswordHistory>
    {
        Task<PasswordHistory> GetByUsername(string username);
        Task<PasswordHistory> GetByUsernamePwd(string username, string pwd);
    }
}
