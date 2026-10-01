using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace WebSite.Pages.Forms
{
    public class TestModel : PageModel
    {
        private readonly INotyfService _notyf;

        public TestModel(INotyfService notyf)
        {
            _notyf = notyf;
        }

        public void OnGet()
        {
        }
        public IActionResult OnPost()
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("mdlrec@mazdock.com");
                mail.To.Add(new MailAddress("akankit1672@gmail.com"));
                mail.IsBodyHtml = true;
                mail.Subject = "MAZAGON DOCK SHIPBUILDERS LIMITED :: Enquiry Form";
                mail.Body = "Hi this is a test mail";
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "relay.hamaracloud.com";
                smtp.Send(mail);
                _notyf.Success("Saved successfully");
                return Page();
            }
            catch (Exception c)
            {
                _notyf.Error(c.ToString());
                return Page();
            }
        }
    }
}
