using Application.Extensions;
using Application.Helpers;
using Application.ServiceInterfaces;
using Application.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace WebBank.Pages.Account
{
    [AllowAnonymous]
    public class ForgotUsernameModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly IEmailService _emailService;
        private readonly INotyfService _notyf;
        public readonly IConfiguration _config;
        public ForgotUsernameModel(
            IHttpClientService httpClient,
            IEmailService emailService,
            INotyfService notyf, IConfiguration config)
        {
            _httpClient = httpClient;
            _emailService = emailService;
            _notyf = notyf;
            _config = config;
        }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(64, ErrorMessage = "{1} characters max")]
        [RegularExpression("^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9]+)*\\.([a-z]{2,4})$", ErrorMessage = "Enter a valid email id")]
        [BindProperty] public string Email { get; set; }

        [DisplayName("Captcha Code")]
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(6)]
        [BindProperty] public string CaptchaCode { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            // Validate Captcha Code
            if (!Captcha.ValidateCaptchaCode(CaptchaCode, HttpContext))
                ModelState.AddModelError("Captcha", "Please enter correct captcha");
            if (!ModelState.IsValid) { _notyf.Error(ModelState.GetErrorMessageString()); return Page(); }
            var result = await _httpClient.PostAsync("Auth/ForgotUsername", false, Email).ConfigureAwait(false);
            var username = !string.IsNullOrEmpty(result) && result != "unauthorized" ? result.Trim('\"', ' ') : null;
            if (string.IsNullOrEmpty(username) || username.StartsWith("System.") || username.Contains("Exception"))
            {
                _notyf.Error("Username could not be found");
                return Page();
            }

            var body = $"Your Username is: <b>{username}</b>";
            var subject = "MAZAGON DOCK SHIPBUILDERS LIMITED :: Your Username";

            if (_config["Environment"].ToString() == "Live")
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(_config["SMTPFrom"]);
                    mail.To.Add(new MailAddress(Email));
                    var bccAddresses = _config["SMTPBcc"].Split(';');
                    foreach (var bcc in bccAddresses)
                    {
                        mail.Bcc.Add(new MailAddress(bcc));
                    }
                    mail.IsBodyHtml = true;
                    mail.Subject = subject;
                    mail.Body = body;

                    using (SmtpClient smtp = new SmtpClient())
                    {
                        smtp.Host = _config["SMTPHost"];
                        smtp.Send(mail);
                    }
                }
            }
            else
            {
                var EmailVm = new EmailVM
                {
                    ToAddresses = new List<string> { Email },
                    BccAddresses = _config["SMTPBcc"].Split(';').ToList(),
                    Subject = subject,
                    Body = body
                };
                await _emailService.SendEmailAsync(EmailVm).ConfigureAwait(false);
            }
            _notyf.Success("Username sent to your registered mail.");
            return LocalRedirect("/bank");

            //await _emailService.SendEmailAsync2(EmailVm).ConfigureAwait(false);
            //_notyf.Success("Your username has been sent to your email");
            //return LocalRedirect("/Account/Login");
        }
        public async Task<IActionResult> OnGetValidatecapcha(string CaptchaCode)
        {
            var message = string.Empty;
            //get all the cities by state for dropdown
            var unameResult = await _httpClient.GetAsync("Auth/CheckUsername", false, CaptchaCode).ConfigureAwait(false);
            var ccode = HttpContext.Session.GetString("CaptchaCode");

            return new JsonResult(ccode);
        }
    }
}