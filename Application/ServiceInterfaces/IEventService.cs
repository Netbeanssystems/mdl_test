using Application.ViewModels;

namespace Application.ServiceInterfaces
{
    public interface IEventService
    {
        void SendNotifications(NotifierEventArgs Notifier);
    }
}