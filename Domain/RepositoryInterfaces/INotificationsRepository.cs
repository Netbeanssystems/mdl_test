using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Domain.RepositoryInterfaces
{
    public interface INotificationsRepository : IRepository<Notifications>
    {
        Task<List<Notifications>> GetAll();
        Task<List<Notifications>> GetByUser(string unm);
        void UpdateWithChildren(Notifications model);
        void CreateWithChildren(Notifications model);
    }
}
