using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryInterfaces
{
    public interface ITempMenuHeadingsRepository : IRepository<TempMenuHeadings>
    {
        void CreateAudit(TempMenuHeadings model);
        Task<List<TempMenuHeadings>> GetByAction(string status);
        Task<List<TempMenuHeadings>> GetByActionupdate(string status);

    }
}
