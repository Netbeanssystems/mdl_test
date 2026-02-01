using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
namespace Infrastructure.Repositories
{
    public class ProjectsRepository : Repository<BidderProjects>, IProjectsRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public ProjectsRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<int> UploadQuota(BidderProjects bidderProjects)
        {
            var model = await DbContext.BidderProjects
                .Where(c => c.Id == bidderProjects.Id)
                .FirstOrDefaultAsync();

            if (model != null)
            {
                var BO = Convert.ToDecimal(bidderProjects.OccupiedQuota);
                model.OccupiedQuota = model.OccupiedQuota + BO;
                await DbContext.SaveChangesAsync();
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
