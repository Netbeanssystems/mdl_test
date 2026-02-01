using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Domain.RepositoryInterfaces
{
    public interface INotificationDetailsRepository : IRepository<NotificationDetails>
    {
        Task<List<NotificationDetails>> GetAll();
        Task<List<NotificationDetails>> GetByUser(string unm);
        Task<List<NotificationDetails>> GetByNotificationId(int nid, string unm);
        Task<NotificationDetails> GetByUrlUser(string url, string unm);
    }
}
