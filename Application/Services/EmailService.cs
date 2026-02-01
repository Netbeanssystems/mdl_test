using Application.AppSettings;
using Application.ServiceInterfaces;
using Application.ViewModels;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
namespace Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }
        public async Task SendEmailAsync(EmailVM EmailVm)
        {
            await SendEmail(EmailVm).ConfigureAwait(false);
        }

        public async Task SendEmailAsync2(EmailVM EmailVm)
        {
            await SendEmail2(EmailVm).ConfigureAwait(false);
        }

        public async Task OnTaskCompleted(object sender, NotifierEventArgs args)
        {
            await SendEmail(args.EmailVm).ConfigureAwait(false);
        }

        private Task SendEmail(EmailVM EmailVm)
        {
            var message = new MailMessage
            {
                From = new MailAddress(_emailSettings.FromAddress, _emailSettings.DisplayName),
                Subject = EmailVm.Subject,
                Body = EmailVm.Body,
                IsBodyHtml = EmailVm.IsBodyHtml
            };
            foreach (var ToAddress in EmailVm.ToAddresses)
                message.To.Add(new MailAddress(ToAddress));
            if (EmailVm.CcAddresses?.Count > 0)
            {
                foreach (var CcAddress in EmailVm.CcAddresses)
                    message.CC.Add(new MailAddress(CcAddress));
            }
            if (EmailVm.BccAddresses?.Count > 0)
            {
                foreach (var BccAddress in EmailVm.BccAddresses)
                    message.Bcc.Add(new MailAddress(BccAddress));
            }
            if (EmailVm.Attachments?.Count > 0)
            {
                foreach (var Attachment in EmailVm.Attachments)
                    message.Attachments.Add(Attachment);
            }
            var smtpClient = new SmtpClient
            {
                Host = _emailSettings.Host,
                Port = _emailSettings.Port,
                EnableSsl = true,
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(_emailSettings.UserName, _emailSettings.Password)
            };
            smtpClient.Send(message);
            return Task.CompletedTask;

            ////Uncomment the code below to send email via Send Grid API
            //var client = new SendGridClient(_emailSettings.ApiKey);
            //var from = new EmailAddress(_emailSettings.FromAddress, _emailSettings.DisplayName);
            //var subject = EmailVm.Subject;
            //var plainTextContent = "";
            //var htmlContent = EmailVm.Body;
            ////var to = new EmailAddress(EmailVm.ToAddresses[0]);
            //var toAddresses = new List<EmailAddress>(EmailVm.ToAddresses.Count);
            //toAddresses.AddRange(EmailVm.ToAddresses.Select(x => new EmailAddress(x)));
            //var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, toAddresses, subject, plainTextContent, htmlContent);
            //if (EmailVm.CcAddresses != null && EmailVm.CcAddresses.Count > 0)
            //{
            //    var ccAddresses = new List<EmailAddress>(EmailVm.CcAddresses.Count);
            //    ccAddresses.AddRange(EmailVm.CcAddresses.Select(ccAddress => new EmailAddress(ccAddress)));
            //    msg.AddCcs(ccAddresses);
            //}
            //if (EmailVm.SendGridAttachments != null && EmailVm.SendGridAttachments.Count > 0)
            //    msg.AddAttachments(EmailVm.SendGridAttachments);
            //var response = await client.SendEmailAsync(msg);
            //if (response.StatusCode == HttpStatusCode.Accepted)
            //{
            //    //Mail sent successfully
            //}
        }


        //For NIC Server
        private Task SendEmail2(EmailVM EmailVm)
        {
            var message = new MailMessage
            {
                From = new MailAddress(_emailSettings.FromAddressSec, _emailSettings.FromAddressSec),
                Subject = EmailVm.Subject,
                Body = EmailVm.Body,
                IsBodyHtml = EmailVm.IsBodyHtml
            };
            foreach (var ToAddress in EmailVm.ToAddresses)
                message.To.Add(new MailAddress(ToAddress));
            if (EmailVm.CcAddresses?.Count > 0)
            {
                foreach (var CcAddress in EmailVm.CcAddresses)
                    message.CC.Add(new MailAddress(CcAddress));
            }
            if (EmailVm.BccAddresses?.Count > 0)
            {
                foreach (var BccAddress in EmailVm.BccAddresses)
                    message.Bcc.Add(new MailAddress(BccAddress));
            }
            if (EmailVm.Attachments?.Count > 0)
            {
                foreach (var Attachment in EmailVm.Attachments)
                    message.Attachments.Add(Attachment);
            }

            var smtpClient = new SmtpClient
            {
                Host = _emailSettings.Host,
                Port = _emailSettings.Port,
                EnableSsl = false,
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(_emailSettings.UserName, _emailSettings.Password)
            };
            smtpClient.Send(message);
            return Task.CompletedTask;
        }
    }
}
