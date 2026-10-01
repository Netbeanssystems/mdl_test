using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
namespace Infrastructure.Repositories
{
    public class DashboardAlertsRepository : Repository<DashboardAlerts>, IDashboardAlertsRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public DashboardAlertsRepository(AppDbContext context) : base(context)
        {
        }
        public Task<DashboardAlerts> GetActiveAlert()
        {
            return DbContext.DashboardAlerts.FirstOrDefaultAsync(x => x.IsActive);
        }
        public Task<DashboardAlerts> CheckDuplicate(DashboardAlerts model)
        {
            var duplicateModel = DbContext.DashboardAlerts.FirstOrDefaultAsync(x =>
                    x.Heading == model.Heading &&
                    x.Message == model.Message);
            return duplicateModel;
        }
    }
}
