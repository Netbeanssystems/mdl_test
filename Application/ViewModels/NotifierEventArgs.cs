using System;

namespace Application.ViewModels
{
    public class NotifierEventArgs : EventArgs
    {
        public NotificationVM NotificationVm { get; set; }
        public SmsVM SmsVm { get; set; }
        public EmailVM EmailVm { get; set; }
    }
}