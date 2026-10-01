using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryInterfaces
{
    public interface IURLsTimingRepository : IRepository<URLsTiming>
    {
        new Task<List<URLsTiming>> GetActive();
        Task<List<URLsTiming>> GetWithDocuments();
        Task<List<URLsTiming>> GetClosed();

        Task<List<URLsTiming>> GetByMultipleIds(List<int> ids);

        Task Update(URLsTiming entity);
    }
}
