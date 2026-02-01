using Application.Dtos;
using Application.Extensions;
using Application.Helpers;
using Application.ServiceInterfaces;
using Application.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
using WebApp.Extensions;
using WebSite.Helpers;

namespace WebSite.Pages.Forms
{
    public class PublicGrievanceModel : PageModel
    {
        Validate objbal = new Validate();
        private readonly IHttpClientService _httpClient;
        private readonly INotyfService _notyf;
        private readonly IEmailService _emailService;
        public readonly IConfiguration _config;
        public PublicGrievanceModel(IHttpClientService httpClient, INotyfService notyf, IEmailService emailService, IConfiguration config)
        {
            _httpClient = httpClient;
            _notyf = notyf;
            _emailService = emailService;
            _config = config;
        }

        [BindProperty] public GrievanceDTO grei { get; set; }
        public async Task<IActionResult> OnGet()
        {
            //get all the countries for dropdown
            var countries = await DataHelper.GetDropdown(_httpClient, "Countries", false).ConfigureAwait(false);

            if (countries == null || countries.Count <= 0)
            {
                _notyf.Error("Countries not found");
                return Page();
            }
            ViewData["Countries"] = new SelectList(countries, "Id", "Text");
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!Captcha.ValidateCaptchaCode(grei.CaptchaCode, HttpContext))
            {
                ModelState.AddModelError("Captcha", "Please enter correct captcha");
            }
            grei.SubmitOn = System.DateTime.Now;

            if (grei.CountryId == null)
            {
                grei.CountryId = "0";
            }

            if (grei.Fax == null)
            {
                grei.Fax = "0";
            }

            grei = ModelAuditor<GrievanceDTO>.SetAudit(User.Identity.Name, "Create", HttpContext.Connection.RemoteIpAddress.ToString(), grei);
            if (!ModelState.IsValid) { _notyf.Error(ModelState.GetErrorMessageString()); return Page(); }
            var response = await _httpClient.PostAsync("Grievance/Create", false, grei).ConfigureAwait(false);

            grei = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<GrievanceDTO>(response) : null;

            if (grei != null)
            {
                // Mail 1
                string strbody = "";
                strbody = "<table width='100%' border='0' cellspacing='0' cellpadding='0'>";
                strbody += "<tr><td align='left' valign='top'>Dear " + grei.Name + "</td></tr>";
                strbody += "<tr><td align='left' valign='top'>Thank for filling Grievance Details. We will get back to you on same.</td></tr>";
                strbody += "</table>";
                strbody += "<br /><br /><table style='font-family: Verdana; font-size: 10pt;'><tbody>";
                strbody += "<tr><td>With Regards,<br /> <br /> <b>MAZAGON DOCK SHIPBUILDERS LIMITED</b><br /></td></tr><tr><td><img height='39'";
                strbody += "src='https://mazagondock.in/Assets/images/logo.svg' v:shapes='_x0000_i1026' width='102' alt='' /></td></tr>";
                strbody += "</tbody></table>";

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(_config["SMTPFrom"]);
                mail.To.Add(new MailAddress(grei.Email));
                mail.IsBodyHtml = true;
                mail.Subject = "MAZAGON DOCK SHIPBUILDERS LIMITED :: Grievance";
                mail.Body = strbody;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = _config["SMTPHost"];
                smtp.Send(mail);

                //Send email
                //var EmailVm = new EmailVM
                //{
                //    ToAddresses = new List<string> { grei.Email },
                //    Subject = "MAZAGON DOCK SHIPBUILDERS LIMITED :: Grievance",
                //    Body = strbody
                //};
                //await _emailService.SendEmailAsync(EmailVm).ConfigureAwait(false);

                // Mail 2

                string bd = "";
                bd += "Dear Admin <br/>";
                bd += "following Grievance Details received <br/>";
                bd += "<table width='100%' border='1' cellspacing='0' cellpadding='0'>";
                bd += "<tr><td align='left' valign='top'><b>Name</b></td><td>" + grei.Name + "</td></tr>";
                bd += "<tr><td align='left' valign='top'><b>Email ID</b></td><td>" + grei.Email + "</td></tr>";
                bd += "<tr><td align='left' valign='top'><b>Designation</b></td><td>" + grei.Designation + "</td></tr>";
                bd += "<tr><td align='left' valign='top'><b>Organization</b></td><td>" + grei.Organization + "</td></tr>";
                bd += "<tr><td align='left' valign='top'><b>Address</b></td><td>" + grei.Address + "</td></tr>";
                bd += "<tr><td align='left' valign='top'><b>City</b></td><td>" + grei.City + "</td></tr>";
                bd += "<tr><td align='left' valign='top'><b>PIN Code</b></td><td>" + grei.PinCode + "</td></tr>";
                bd += "<tr><td align='left' valign='top'><b>Country</b></td><td>" + grei.CountryId + "</td></tr>";
                bd += "<tr><td align='left' valign='top'><b>Telephone No.</b></td><td>" + grei.Contact + "</td></tr>";
                bd += "<tr><td align='left' valign='top'><b>Fax No.</b></td><td>" + grei.Fax + "</td></tr>";
                bd += "<tr><td align='left' valign='top'><b>Grievance.</b></td><td>" + grei.Grievance + "</td></tr>";
                bd += "</table>";
                bd += "<br /><br /><table style='font-family: Verdana; font-size: 10pt;'><tbody>";
                bd += "<tr><td>With Regards,<br /> <br /> <b>MAZAGON DOCK SHIPBUILDERS LIMITED</b><br /></td></tr><tr><td><img height='39'";
                bd += "src='https://mdl.businesstowork.com/Assets/images/logo.svg' v:shapes='_x0000_i1026' width='102' alt='' /></td></tr>";
                bd += "</tbody></table>";

                //MailMessage mail1 = new MailMessage();
                //mail1.From = new MailAddress(_config["SMTPFrom"]);
                //mail1.To.Add(new MailAddress(_config["SMTPFromSecond"]));
                //mail1.IsBodyHtml = true;
                //mail1.Subject = "MAZAGON DOCK SHIPBUILDERS LIMITED :: Grievance";
                //mail1.Body = bd;
                //SmtpClient smtp1 = new SmtpClient();
                //smtp1.Host = _config["SMTPHost"];
                //smtp1.Send(mail1);

                //Send email
                var EmailVm1 = new EmailVM
                {
                    ToAddresses = new List<string> { _config["SMTPFromSecond"] },
                    Subject = "MAZAGON DOCK SHIPBUILDERS LIMITED :: Grievance",
                    Body = strbody
                };
                await _emailService.SendEmailAsync(EmailVm1).ConfigureAwait(false);
            }

            if (grei == null)
                _notyf.Error("Save failed");
            else
                _notyf.Success("Saved successfully");
            return RedirectToPage("/Forms/PublicGrievance");
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