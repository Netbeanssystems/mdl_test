using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Domain.RepositoryInterfaces
{
    public interface IDocumentsRepository : IRepository<Documents>
    {
        Task<List<Documents>> Getbycreatedby(string createdby);
    }
}
