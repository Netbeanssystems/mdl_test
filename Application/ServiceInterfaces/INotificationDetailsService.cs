using Application.Dtos;
using Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Application.ServiceInterfaces
{
    public interface INotificationDetailsService
    {
        //Common Methods
        Task<List<NotificationDetailsVM>> Get();
        Task<NotificationDetailsDTO> Get(int id);
        Task<NotificationDetailsVM> GetVM(int id);
        Task<NotificationDetailsDTO> Create(NotificationDetailsDTO argModelDto);
        Task<NotificationDetailsDTO> Update(NotificationDetailsDTO argModelDto);
        Task<int> Delete(int id);
        //Custom Methods
        Task<List<NotificationDetailsVM>> GetAll();
        Task<List<NotificationDetailsVM>> GetByUser(string unm);
        Task<List<NotificationDetailsVM>> GetByNotificationId(int id, string unm);
        Task<NotificationDetailsVM> GetByTargetUrlUserName(string url, string uname);
    }
}
