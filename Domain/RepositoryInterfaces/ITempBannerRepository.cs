using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryInterfaces
{
    public interface ITempBannerRepository : IRepository<TempBanners>
    {
        void CreateAudit(TempBanners model);
        Task<List<TempBanners>> GetByAction(string status);
    }
}
