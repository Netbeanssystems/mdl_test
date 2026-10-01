using Domain.Models;
using System.Threading.Tasks;
namespace Domain.RepositoryInterfaces
{
    public interface IProjectsRepository : IRepository<BidderProjects>
    {
        Task<int> UploadQuota(BidderProjects bidderProjects);
    }
}
