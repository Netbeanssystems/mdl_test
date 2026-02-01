using Application.Dtos;
using Application.Extensions;
using Application.Helpers;
using Application.ServiceInterfaces;
using Application.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
namespace WebSite.Pages.Forms
{
    public class FooterEnquiryModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly INotyfService _notyf;
        private readonly IEmailService _emailService;
        public readonly IConfiguration _config;
        public FooterEnquiryModel(IHttpClientService httpClient, INotyfService notyf, IEmailService emailService, IConfiguration config)
        {
            _httpClient = httpClient;
            _notyf = notyf;
            _emailService = emailService;
            _config = config;
        }
        [BindProperty] public EnquiryDTO vigi { get; set; }
        [FromRoute] public string OtherLinkHeading { get; set; }
        [BindProperty] public CommonVM ModelVM { get; set; }

        [BindProperty] public ContactUsDTO contactusform { get; set; }
        public OtherLinkHeadingVM Menus { get; set; }
        //public async Task<IActionResult> OnGet()
        //{
        //    var Result = await _httpClient.GetAsync("OtherLinkHeading/GetMenus", false, OtherLinkHeading.Replace("-", " ").ToLower());
        //    if (Result != null)
        //    {
        //        Menus = !string.IsNullOrEmpty(Result) ? JsonConvert.DeserializeObject<OtherLinkHeadingVM>(Result) : null;
        //        if (HttpContext.Session.GetString("Lang") == null || HttpContext.Session.GetString("Lang") == "English")
        //        {
        //            ModelVM = new CommonVM
        //            {
        //                Heading = Menus.EnglishHeadingName,
        //                Content = Menus.EnglishContentDesc,
        //                Title = Menus.Title,
        //                Description = Menus.Description,
        //                Keywords = Menus.Keyword,
        //                UpdateDate = Menus.UpdateDate
        //            };
        //            HttpContext.Session.SetString("LastDate", Menus.UpdateDate.ToString());
        //        }
        //        else
        //        {
        //            ModelVM = new CommonVM
        //            {
        //                Heading = Menus.HindiHeadingName,
        //                Content = Menus.HindiContentDesc,
        //                HindiTitle = Menus.HindiTitle,
        //                Description = Menus.Description,
        //                Keywords = Menus.Keyword,
        //                UpdateDate = Menus.UpdateDate
        //            };
        //            HttpContext.Session.SetString("LastDate", Menus.UpdateDate.ToString());
        //        }
        //    }
        //    else
        //    {
        //        //TempData["Results"] = "The page you are looking for doesn't exist, you may have mistyped the address or the page may have moved.";
        //        return RedirectToPage("Index");
        //    }

        //    contactusform = new ContactUsDTO
        //    {
        //        Id = 0,
        //        FirstName = "",
        //        LastName = "",
        //        CompanyName = "",
        //        CountryName = "",
        //        InterestedIn = "",
        //        EmailId = "",
        //        ToEmailId = "",
        //        Contact = "",
        //        Message = "",
        //    };

        //    TempData["CategoryTabName"] = ModelVM.UpdateDate ?? DateTime.ParseExact("10-10-2021", "dd-MM-yyyy", CultureInfo.InvariantCulture);
        //    return Page();
        //}

        public async Task<IActionResult> OnPost()
        {
            //if (!Captcha.ValidateCaptchaCode(vigi.CaptchaCode, HttpContext))
            //{
            //    ModelState.AddModelError("Captcha", "Please enter correct captcha");
            //}
            // added for captcha
            // 1️⃣ CAPTCHA VALIDATION (STOP EXECUTION)
            if (!Captcha.ValidateCaptchaCode(contactusform.CaptchaCode, HttpContext))
            {
                ModelState.AddModelError("contactusform.CaptchaCode", "Invalid captcha");
                return Page();   // ⛔ STOP HERE
            }

            // 2️⃣ Model validation
            if (!ModelState.IsValid)
            {
                return Page();
            }

            contactusform.SubmitOn = System.DateTime.Now;
            contactusform.ToEmailId = contactusform.EmailId;
            contactusform.SubmitedFrom = HttpContext.Request.GetDisplayUrl().Split('/').LastOrDefault();
            contactusform = ModelAuditor<ContactUsDTO>.SetAudit(User.Identity.Name, "Create", HttpContext.Connection.RemoteIpAddress.ToString(), contactusform);
            //if (!ModelState.IsValid)
            //{ 
            //    _notyf.Error(ModelState.GetErrorMessageString()); 
            //    return Page();
            //}
            var response = await _httpClient.PostAsync("ContactUs/Create", false, contactusform).ConfigureAwait(false);

            contactusform = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<ContactUsDTO>(response) : null;

            if (contactusform != null)
            {
                //var EmailVm = new EmailVM
                //{
                //    ToAddresses = new List<string> { "justroshan.dev@gmail.com" },
                //    Subject = "Services Enquiry Form Response",
                //    Body = $"Dear user,<br><br>Thank You for contacting with us."
                //};

                var body = "Dear " + contactusform.FirstName + " " + contactusform.LastName + ",<br/><br/>Thank you for filling out the enquiry form!<br/><br/>We appreciate you contacting us. One of our executive will get back in touch with you soon!<br/><br/>Have a great day!<br/><br/>Thank you.<br/><br/>Regards<br/>Marketing Team<br/>Mazagon Dock Shipbuilders Ltd., Mumbai.";

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(_config["SMTPFrom"]);
                    mail.To.Add(new MailAddress(contactusform.EmailId));
                    var bccAddresses = _config["SMTPBcc"].Split(';');
                    foreach (var bcc in bccAddresses)
                    {
                        mail.Bcc.Add(new MailAddress(bcc));
                    }
                    mail.IsBodyHtml = true;
                    mail.Subject = "Services Enquiry Form Response";
                    mail.Body = body;

                    using (SmtpClient smtp = new SmtpClient())
                    {
                        smtp.Host = _config["SMTPHost"];
                        smtp.Send(mail);
                    }
                }

                //Send email
                //var EmailVm = new EmailVM
                //{
                //    ToAddresses = new List<string> { contactusform.EmailId },
                //    BccAddresses = _config["SMTPBcc"].Split(';').ToList(),
                //    Subject = "Services Enquiry Form Response",
                //    Body = body
                //};
                //await _emailService.SendEmailAsync(EmailVm).ConfigureAwait(false);

                StringBuilder Adminbody = new StringBuilder("");
                Adminbody.Append("<html><head></head><body>");
                Adminbody.Append("<b>Dear Admin,</b><br>");
                Adminbody.Append("A new query on your website:<br><br>");
                Adminbody.Append("<table><tr>");
                Adminbody.Append("<th align='left'>Full Name :</th>");
                Adminbody.Append("<td>" + contactusform.FirstName + " " + contactusform.LastName + "</td></tr><tr>");
                Adminbody.Append("<th align='left'>Company :</th>");
                Adminbody.Append("<td>" + contactusform.CompanyName + "</td></tr><tr>");
                Adminbody.Append("<th align='left'>Country :</th>");
                Adminbody.Append("<td>" + contactusform.CountryName + "</td></tr><tr>");
                Adminbody.Append("<th align='left'>Interested In :</th>");
                Adminbody.Append("<td>" + contactusform.InterestedIn + "</td></tr><tr>");
                Adminbody.Append("<th align='left'>Email :</th>");
                Adminbody.Append("<td>" + contactusform.EmailId + "</td></tr><tr>");
                Adminbody.Append("<th align='left'>Contact No. :</th>");
                Adminbody.Append("<td>" + contactusform.Contact + "</td></tr><tr>");
                Adminbody.Append("<th align='left'>Your Enquiry :</th>");
                Adminbody.Append("<td>" + contactusform.Message + "</td></tr>");
                Adminbody.Append("</table><br><br>");
                //Adminbody.Append("<b>Copyright 2022 � Simran International, All rights reserved.</b>");
                Adminbody.Append("</body></html>");

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(_config["SMTPFromAdmin"]);
                    mail.To.Add(new MailAddress(_config["SMTPToAdmin"]));
                    var bccAddresses = _config["SMTPBccAdmin"].Split(';');
                    foreach (var bcc in bccAddresses)
                    {
                        mail.Bcc.Add(new MailAddress(bcc));
                    }
                    mail.IsBodyHtml = true;
                    mail.Subject = "Services Enquiry Form Response";
                    mail.Body = Adminbody.ToString();

                    using (SmtpClient smtp = new SmtpClient())
                    {
                        smtp.Host = _config["SMTPHost"];
                        smtp.Send(mail);
                    }
                }

                //Send email
                //var EmailVmAdmin = new EmailVM
                //{
                //    ToAddresses = new List<string> { _config["SMTPToAdmin"] },
                //    BccAddresses = _config["SMTPBccAdmin"].Split(';').ToList(),
                //    Subject = "Services Enquiry Form Response",
                //    Body = Adminbody.ToString()
                //};
                //await _emailService.SendEmailAsync(EmailVmAdmin).ConfigureAwait(false);
            }

            if (contactusform == null)
                _notyf.Error("Save failed");
            else
                _notyf.Success("Submitted successfully");
            return RedirectToPage();
        }
        //public async Task<IActionResult> OnGetValidatecapcha(string CaptchaCode)
        //{
        //    var message = string.Empty;
        //    //get all the cities by state for dropdown
        //    var unameResult = await _httpClient.GetAsync("Auth/CheckUsername", false, CaptchaCode).ConfigureAwait(false);
        //    var ccode = HttpContext.Session.GetString("CaptchaCode");

        //    return new JsonResult(ccode);
        //}

        //added for captcha
        public IActionResult OnGetValidatecapcha(string CaptchaCode)
        {
            var sessionCaptcha = HttpContext.Session.GetString("CaptchaCode");
            return new JsonResult(sessionCaptcha);
        }

    }
}
