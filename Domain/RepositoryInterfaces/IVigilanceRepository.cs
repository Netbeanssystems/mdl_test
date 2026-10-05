using Domain.Models;
using System.Threading.Tasks;

namespace Domain.RepositoryInterfaces
{
    public interface IVigilanceRepository : IRepository<VigilanceForm>
    {
        // ✅ 24-hour rate limit check — DB se
        Task<bool> HasRecentComplaintByEmail(string email, int hours);
    }
}
