using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Mail;

namespace WebApp.Pages.Admin.Test
{
    public class IndexModel : PageModel
    {
        private readonly INotyfService _notyf;

        public IndexModel(INotyfService notyf)
        {
            _notyf = notyf;
        }
        public void OnGet()
        {
        }

        public void OnPost()
        {
            //MailMessage mail = new MailMessage();
            //SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            //mail.From = new MailAddress("your_email_address@gmail.com");
            //mail.To.Add("akankit1672@gmail.com");
            //mail.Subject = "Test Mail";
            //mail.Body = "This is for testing SMTP mail from GMAIL";
            //SmtpServer.Port = 25;
            //SmtpServer.Credentials = new System.Net.NetworkCredential("username", "password");
            //SmtpServer.EnableSsl = true;
            //SmtpServer.Send(mail);


            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("mdlrec@mazdock.com");
            mail.To.Add(new MailAddress("akankit1672@gmail.com"));
            mail.CC.Add(new MailAddress("akankit1672@gmail.com"));
            mail.IsBodyHtml = true;
            mail.Subject = "Mail One";
            mail.Body = "Hello This is Mail One";
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "relay.hamaracloud.com";
            smtp.Port = 25;
            smtp.Send(mail);

            _notyf.Success("Mail Sent");
        }
    }
}
