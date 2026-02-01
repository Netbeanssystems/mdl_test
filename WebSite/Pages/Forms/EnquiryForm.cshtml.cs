using Application.Dtos;
using Application.Extensions;
using Application.Helpers;
using Application.ServiceInterfaces;
using Application.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;

namespace WebSite.Pages.Forms
{
    public class EnquiryFormModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly INotyfService _notyf;
        private readonly IEmailService _emailService;
        public readonly IConfiguration _config;

        public EnquiryFormModel(IHttpClientService httpClient, INotyfService notyf, IEmailService emailService)
        {
            _httpClient = httpClient;
            _notyf = notyf;
            _emailService = emailService;
        }

        [BindProperty] public EnquiryDTO vigi { get; set; }
        public IActionResult OnGet()
        {
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            vigi.SubmitOn = System.DateTime.Now;
            vigi = ModelAuditor<EnquiryDTO>.SetAudit(User.Identity.Name, "Create", HttpContext.Connection.RemoteIpAddress.ToString(), vigi);
            if (!ModelState.IsValid) { _notyf.Error(ModelState.GetErrorMessageString()); return Page(); }
            var response = await _httpClient.PostAsync("Enquiry/Create", false, vigi).ConfigureAwait(false);

            vigi = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<EnquiryDTO>(response) : null;

            if (vigi != null)
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(_config["SMTPFrom"]);
                mail.To.Add(new MailAddress("feedback@mazdock.com"));
                mail.IsBodyHtml = true;
                mail.Subject = "Enquiry Form Response";
                mail.Body = $"Dear user,<br><br>Thank You for contacting with us.";
                SmtpClient smtp = new SmtpClient();
                smtp.Host = _config["SMTPHost"];
                smtp.Send(mail);


                //var EmailVm = new EmailVM
                //{
                //    ToAddresses = new List<string> { vigi.Email },
                //    Subject = "Enquiry Form Response",
                //    Body = $"Dear user,<br><br>Thank You for contacting with us."
                //};

                //await _emailService.SendEmailAsync(EmailVm);
            }

            if (vigi == null)
                _notyf.Error("Save failed");
            else
                _notyf.Success("Saved successfully");
            return RedirectToPage("/Forms/Enquiry");
        }
    }
}
