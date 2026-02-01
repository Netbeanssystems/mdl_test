using Application.Dtos;
using Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Application.ServiceInterfaces
{
    public interface INotificationsService
    {
        //Common Methods
        Task<List<NotificationsVM>> Get();
        Task<NotificationsDTO> Get(int id);
        Task<NotificationsVM> GetVM(int id);
        Task<NotificationsDTO> Create(NotificationsDTO argModelDto);
        Task<NotificationsDTO> Update(NotificationsDTO argModelDto);
        Task<int> Delete(int id);
        //Custom Methods
        Task<List<NotificationsVM>> GetAll();
        Task<List<NotificationsVM>> GetByUser(string unm);
        Task<NotificationsDTO> Notify(NotificationVM NotificationVm);
        //Event Handlers
        void OnTaskCompleted(object sender, NotifierEventArgs args);
    }
}
