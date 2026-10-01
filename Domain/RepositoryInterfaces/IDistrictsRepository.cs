using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Domain.RepositoryInterfaces
{
    public interface IDistrictsRepository : IRepository<Districts>
    {
        Task<List<Districts>> GetDistrictsByState(int stateid);
    }
}
