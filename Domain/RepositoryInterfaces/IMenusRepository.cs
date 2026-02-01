using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Domain.RepositoryInterfaces
{
    public interface IMenusRepository : IRepository<Menus>
    {
        Task<List<Menus>> GetWithAll();
    }
}
