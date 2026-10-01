using Application.ViewModels;
using System.Collections.Generic;

namespace Application.Helpers
{
    public class NotifierEventArgsGenerator
    {
        public NotifierEventArgs NotifierEventArgs { get; }

        public NotifierEventArgsGenerator()
        {
            NotifierEventArgs = new NotifierEventArgs();
        }
        public NotifierEventArgsGenerator SendNotification(string IconClass, string Title, string Text, string Role, List<string> UserNames, string TargetUrl, string UserName, string Ip)
        {
            var NotificationVm = new NotificationVM
            {
                IconClass = IconClass,
                Title = Title,
                Text = Text,
                Role = Role,
                UserNames = UserNames,
                TargetUrl = TargetUrl,
                UserName = UserName,
                Ip = Ip
            };
            NotifierEventArgs.NotificationVm = NotificationVm;
            return this;
        }
        public NotifierEventArgsGenerator SendEmail(List<string> ToAddresses, string Subject, string Body, bool IsBodyHtml)
        {
            var EmailVm = new EmailVM
            {
                ToAddresses = ToAddresses,
                Subject = Subject,
                Body = Body,
                IsBodyHtml = IsBodyHtml
            };
            NotifierEventArgs.EmailVm = EmailVm;
            return this;
        }
        public NotifierEventArgsGenerator SendSms(string MessageText, List<string> MobileNos)
        {
            var SmsVm = new SmsVM
            {
                MessageText = MessageText,
                MobileNos = MobileNos
            };
            NotifierEventArgs.SmsVm = SmsVm;
            return this;
        }
    }
}
