using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryInterfaces
{
    public interface ITempMediaRepository : IRepository<TempMedia>
    {
        void CreateAudit(TempMedia model);
        Task<List<TempMedia>> GetByAction(string status);
    }
}
