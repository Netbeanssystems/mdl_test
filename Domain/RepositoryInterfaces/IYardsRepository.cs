using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Domain.RepositoryInterfaces
{
    public interface IYardsRepository : IRepository<BidderYards>
    {
        Task<List<BidderYards>> GetYard(string projectId);
        Task<List<BidderYards>> GetYardbyyardis(string projectId);
        Task<List<ProjectResponse>> GetProjects();
    }
}
