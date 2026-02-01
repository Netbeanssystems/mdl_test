using Application.Dtos;
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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace WebApp.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly IEmailService _emailService;
        private readonly INotyfService _notyf;
        public readonly IConfiguration _config;
        public ForgotPasswordModel(
            IHttpClientService httpClient,
            IEmailService emailService,
            INotyfService notyf,
            IConfiguration config)
        {
            _httpClient = httpClient;
            _emailService = emailService;
            _notyf = notyf;
            _config = config;
        }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(32, ErrorMessage = "{1} characters max")]
        [BindProperty] public string Username { get; set; }
        [BindProperty] public ForgetPasswordDetailsDTO ForgetPasswordDetailsDTO { get; set; }

        [DisplayName("Captcha Code")]
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(6)]
        [BindProperty] public string CaptchaCode { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var currentdate = DateTime.Now.Date;
            // Validate Captcha Code
            if (!Captcha.ValidateCaptchaCode(CaptchaCode, HttpContext))
                ModelState.AddModelError("Captcha", "Please enter correct captcha");
            if (!ModelState.IsValid) { _notyf.Error(ModelState.GetErrorMessageString()); return Page(); }
            var result = await _httpClient.PostAsync("Auth/ForgotPassword", false, Username).ConfigureAwait(false);
            var forgotPasswordVm = !string.IsNullOrEmpty(result) ? JsonConvert.DeserializeObject<ForgotPasswordVM>(result) : null;

            var resultcount = await _httpClient.GetAsync("ForgetPasswordDetails/GetPasswordsendCount", false, Username, currentdate.ToString()).ConfigureAwait(false);
            var DetailCount = !string.IsNullOrEmpty(resultcount) ? JsonConvert.DeserializeObject<List<ForgetPasswordDetailsVM>>(resultcount) : null;

            if (resultcount != null)
            {
                if (DetailCount.Count >= 3)
                {
                    _notyf.Error("Password reset link could not be generated more than 3 times.");
                    return Page();

                }
            }


            if (forgotPasswordVm?.Code == null || forgotPasswordVm.Email == null)
            {
                _notyf.Error("Password reset link could not be generated");
                return Page();
            }
            var callbackUrl = Url.Page(
                "/Account/ResetPassword",
                pageHandler: null,
                values: new { forgotPasswordVm.Code, forgotPasswordVm.Id },
                protocol: Request.Scheme);
            ForgetPasswordDetailsDTO = new ForgetPasswordDetailsDTO();
            ForgetPasswordDetailsDTO.UserId = Username;
            ForgetPasswordDetailsDTO.Email = forgotPasswordVm.Email;
            ForgetPasswordDetailsDTO.CreatedDate = System.DateTime.Now.Date;
            //var Result = await _httpClient.PostAsync("PasswordResetLimit/Create", false, PasswordResetLimitDTO);
            var Result = await _httpClient.PostAsync("ForgetPasswordDetails/Create", false, ForgetPasswordDetailsDTO);

            //var EmailVm = new EmailVM
            //{
            //    ToAddresses = new List<string> { forgotPasswordVm.Email },
            //    Subject = "Password Reset Link",
            //    Body = $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>."
            //};
            //await _emailService.SendEmailAsync2(EmailVm).ConfigureAwait(false);

            var body = $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.";

            if (_config["Environment"].ToString() == "Live")
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(_config["SMTPFrom"]);
                    mail.To.Add(new MailAddress(forgotPasswordVm.Email));
                    var bccAddresses = _config["SMTPBcc"].Split(';');
                    foreach (var bcc in bccAddresses)
                    {
                        mail.Bcc.Add(new MailAddress(bcc));
                    }
                    mail.IsBodyHtml = true;
                    mail.Subject = "MDL-Bank Portal Password Reset";
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
                    ToAddresses = new List<string> { forgotPasswordVm.Email },
                    BccAddresses = _config["SMTPBcc"].Split(';').ToList(),
                    Subject = "MDL-Bank Portal Password Reset",
                    Body = body
                };
                await _emailService.SendEmailAsync(EmailVm).ConfigureAwait(false);
            }


            _notyf.Success("Please check your email to reset your password");
            return LocalRedirect("/admin/Account/Login");
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