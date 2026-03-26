using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryInterfaces
{
    public interface IURLsTimingRepository : IRepository<URLsTiming>
    {
        new Task<List<URLsTiming>> GetActive();
        Task<List<URLsTiming>> GetWithDocuments();
    }
}
