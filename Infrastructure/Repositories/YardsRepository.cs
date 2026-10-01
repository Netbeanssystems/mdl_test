using DocumentFormat.OpenXml.InkML;
using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Infrastructure.Repositories
{
    public class YardsRepository : Repository<BidderYards>, IYardsRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public YardsRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<BidderYards>> GetYard(string projectId)
        {
            var projectIdList = projectId.Split(',').ToList();

            return await DbContext.BidderYards
                .Where(x => projectIdList.Contains(x.ProjectId.ToString()))
                .ToListAsync();
        }
        public async Task<List<BidderYards>> GetYardbyyardis(string projectId)
        {
            var projectIdList = projectId.Split(',').ToList();

            return await DbContext.BidderYards
                .Where(x => projectIdList.Contains(x.Id.ToString()))
                .ToListAsync();
        }

        public async Task<List<ProjectResponse>> GetProjects()
        {
            var result = await (
                    from project in DbContext.BidderProjects.OrderByDescending(x => x.Id)
                    join yard in DbContext.BidderYards
                    on project.Id equals yard.ProjectId
                    select new ProjectResponse
                    {
                        Id = project.Id,
                        ProjectName = project.ProjectName,
                        Yard = yard.YardNumber,
                        Remarks = project.Remarks,
                        CreatedBy = project.CreatedBy,
                        CreatedDate = project.CreatedDate,
                        ModifiedDate = project.ModifiedDate,
                        ModifiedBy = project.ModifiedBy
                    }
                ).ToListAsync();

            return result;
        }
    }
}
