using Application.Dtos;
using Application.Extensions;
using Application.Helpers;
using Application.ServiceInterfaces;
using Application.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
using WebApp.Extensions;

namespace WebSite.Pages.Forms
{
    public class EnquiryModel : PageModel
    {
        Validate objbal = new Validate();
        private readonly IHttpClientService _httpClient;
        private readonly INotyfService _notyf;
        private readonly IEmailService _emailService;
        public readonly IConfiguration _config;

        public EnquiryModel(IHttpClientService httpClient, INotyfService notyf, IEmailService emailService, IConfiguration config)
        {
            _httpClient = httpClient;
            _notyf = notyf;
            _emailService = emailService;
            _config = config;
        }
        [BindProperty] public EnquiryDTO vigi { get; set; }
        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!Captcha.ValidateCaptchaCode(vigi.CaptchaCode, HttpContext))
            {
                ModelState.AddModelError("Captcha", "Please enter correct captcha");
            }

            vigi.SubmitOn = System.DateTime.Now;

            var filteredmodel = new EnquiryDTO();
            //filteredmodel.Id = vigi.Id;
            filteredmodel.Name = objbal.OnlyValid(vigi.Name);
            filteredmodel.Designation = objbal.OnlyValid(vigi.Designation);
            filteredmodel.Organization = objbal.OnlyValid(vigi.Organization);
            filteredmodel.Email = objbal.OnlyValid(vigi.Email);
            filteredmodel.Contact = objbal.OnlyValid(vigi.Contact);
            filteredmodel.Fax = vigi.Fax;
            filteredmodel.PostalAddress = objbal.OnlyValid(vigi.PostalAddress);
            filteredmodel.ComplaintDetails = objbal.OnlyValid(vigi.ComplaintDetails);
            filteredmodel.SubmitOn = vigi.SubmitOn;
            filteredmodel.CaptchaCode = vigi.CaptchaCode;

            vigi = ModelAuditor<EnquiryDTO>.SetAudit(User.Identity.Name, "Create", HttpContext.Connection.RemoteIpAddress.ToString(), filteredmodel);
            if (!ModelState.IsValid) { _notyf.Error(ModelState.GetErrorMessageString()); return Page(); }
            var response = await _httpClient.PostAsync("Enquiry/Create", false, filteredmodel).ConfigureAwait(false);

            vigi = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<EnquiryDTO>(response) : null;

            if (vigi != null)
            {
                var body = "<table width='100%' border='0' cellspacing='0' cellpadding='0'><tr><td align='left' valign='top'><div align='justify'><font color='#00008A' size='2' face='Verdana, Arial, Helvetica, sans-serif'>" +
                    "<br/><br/>Dear " + filteredmodel.Name + ",<br/><br/>Thank for filling Enquiry Form Details. We will get back to you on same!!<br/><br/>" +
                    "<br/><br/><br/>Thanks & Regards,<br/><br/>MAZAGON DOCK LIMITED. <br/></div></td></tr></table>";

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(_config["SMTPFrom"]);
                mail.To.Add(new MailAddress(filteredmodel.Email));
                mail.IsBodyHtml = true;
                mail.Subject = "MAZAGON DOCK SHIPBUILDERS LIMITED :: Enquiry Form";
                mail.Body = body;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = _config["SMTPHost"];
                smtp.Send(mail);

                //Send email
                //var EmailVm = new EmailVM
                //{
                //    ToAddresses = new List<string> { filteredmodel.Email },
                //    Subject = "MAZAGON DOCK SHIPBUILDERS LIMITED :: Enquiry Form",
                //    Body = body
                //};
                //await _emailService.SendEmailAsync(EmailVm).ConfigureAwait(false);
            }

            if (vigi == null)
                _notyf.Error("Save failed");
            else
                _notyf.Success("Saved successfully");
            return RedirectToPage("/Forms/Enquiry");
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