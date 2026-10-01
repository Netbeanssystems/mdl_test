using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Domain.RepositoryInterfaces
{
    public interface IStatesRepository : IRepository<States>
    {
        Task<List<States>> GetStatesByCountry(int countryid);
    }
}
