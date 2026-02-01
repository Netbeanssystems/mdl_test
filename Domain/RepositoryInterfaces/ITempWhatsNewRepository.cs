using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryInterfaces
{
    public interface ITempWhatsNewRepository : IRepository<TempWhatsNew>
    {
        void CreateAudit(TempWhatsNew model);
        Task<List<TempWhatsNew>> GetByAction(string status);
    }
}
