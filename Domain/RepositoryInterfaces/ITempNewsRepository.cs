using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryInterfaces
{
    public interface ITempNewsRepository : IRepository<TempNews>
    {
        void CreateAudit(TempNews model);
        Task<List<TempNews>> GetByAction(string status);
    }
}
