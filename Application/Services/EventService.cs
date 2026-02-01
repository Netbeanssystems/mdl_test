using Application.ServiceInterfaces;
using Application.ViewModels;

namespace Application.Services
{
    public class EventService : IEventService
    {
        private readonly IDataService _dataService;
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;
        public EventService(IDataService dataService, IEmailService emailService, ISmsService smsService)
        {
            _dataService = dataService;
            _emailService = emailService;
            _smsService = smsService;
        }

        public delegate void TaskCompletedEventHandler(object sender, NotifierEventArgs e);
        public event TaskCompletedEventHandler TaskCompleted;
        protected virtual void OnTaskCompleted(NotifierEventArgs e)
        {
            TaskCompleted?.Invoke(this, e);
        }
        public void SendNotifications(NotifierEventArgs Notifier)
        {
            var delegates = TaskCompleted?.GetInvocationList();
            if (delegates?.Length > 0)
                foreach (var item in delegates)
                    TaskCompleted -= (TaskCompletedEventHandler)item;
            if (Notifier.NotificationVm != null)
                TaskCompleted += (source, args) => _dataService.Notifications.OnTaskCompleted(source, args);
            if (Notifier.EmailVm != null)
                TaskCompleted += (source, args) => _emailService.OnTaskCompleted(source, args);
            if (Notifier.SmsVm != null)
                TaskCompleted += (source, args) => _smsService.OnTaskCompleted(source, args);
            OnTaskCompleted(Notifier);
        }
    }
}
