using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryInterfaces
{
    public interface ITempVideoRepository : IRepository<TempVideo>
    {
        void CreateAudit(TempVideo model);
        Task<List<TempVideo>> GetByAction(string status);
    }
}
