using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryInterfaces
{
    public interface IEventsRepository : IRepository<Events>
    {
        Task<List<Events>> GetDropdownById(int id);


        Task<List<Events>> GetAllEvents();             //------------ Karn
    }
}
