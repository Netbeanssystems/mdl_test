using Domain.Models;
using System.Threading.Tasks;
namespace Domain.RepositoryInterfaces
{
    public interface IDashboardAlertsRepository : IRepository<DashboardAlerts>
    {
        Task<DashboardAlerts> GetActiveAlert();
        Task<DashboardAlerts> CheckDuplicate(DashboardAlerts model);
    }
}
