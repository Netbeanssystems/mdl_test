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
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
using WebApp.Extensions;

namespace WebSite.Pages.Forms
{
    public class FeedbackModel : PageModel
    {
        Validate objbal = new Validate();
        private readonly IHttpClientService _httpClient;
        private readonly INotyfService _notyf;
        private readonly IEmailService _emailService;
        public readonly IConfiguration _config;

        public FeedbackModel(IHttpClientService httpClient, INotyfService notyf, IEmailService emailService, IConfiguration config)
        {
            _httpClient = httpClient;
            _notyf = notyf;
            _emailService = emailService;
            _config = config;
        }
        [BindProperty] public FeedbackDTO vigi { get; set; }
        [BindProperty] public TestDTO TEST { get; set; }
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

            var filteredmodel = new FeedbackDTO();
            filteredmodel.Id = vigi.Id;
            filteredmodel.RatingNo1 = vigi.RatingNo1;
            filteredmodel.RatingNo2 = vigi.RatingNo2;
            filteredmodel.RatingNo3 = vigi.RatingNo3;
            filteredmodel.RatingNo4 = vigi.RatingNo4;
            filteredmodel.RatingNo5 = vigi.RatingNo5;
            filteredmodel.RatingNo6 = vigi.RatingNo6;
            filteredmodel.RatingNo7 = vigi.RatingNo7;
            filteredmodel.RatingNo8 = vigi.RatingNo8;
            filteredmodel.ReasonSatisfied = objbal.OnlyValid(vigi.ReasonSatisfied);
            filteredmodel.ReasonDisSatisfied = objbal.OnlyValid(vigi.ReasonDisSatisfied);
            filteredmodel.Name = objbal.OnlyValid(vigi.Name);
            filteredmodel.Designation = objbal.OnlyValid(vigi.Designation);
            filteredmodel.Organization = objbal.OnlyValid(vigi.Organization);
            filteredmodel.City = objbal.OnlyValid(vigi.City);
            filteredmodel.Email = objbal.OnlyValid(vigi.Email);
            filteredmodel.Address = objbal.OnlyValid(vigi.Address);
            filteredmodel.Comment = objbal.OnlyValid(vigi.Comment);
            filteredmodel.SubmitOn = vigi.SubmitOn;
            filteredmodel.CaptchaCode = vigi.CaptchaCode;

            vigi = ModelAuditor<FeedbackDTO>.SetAudit(User.Identity.Name, "Create", HttpContext.Connection.RemoteIpAddress.ToString(), filteredmodel);
            if (!ModelState.IsValid) { _notyf.Error(ModelState.GetErrorMessageString()); return Page(); }
            var response = await _httpClient.PostAsync("Test/Create", false, filteredmodel).ConfigureAwait(false);

            vigi = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<FeedbackDTO>(response) : null;

            if (vigi != null)
            {
                string subject = "Feedback from MDL Website - ";

                if (filteredmodel.RatingNo1 != 0)
                {
                    subject += "Website" + ",";
                }

                if (filteredmodel.RatingNo2 != 0)
                {
                    subject += "Online Recruitment" + ",";
                }

                if (filteredmodel.RatingNo3 != 0)
                {
                    subject += "Vender Registration" + ",";
                }

                if (filteredmodel.RatingNo4 != 0)
                {
                    subject += "EProcurement" + ",";
                }

                if (filteredmodel.RatingNo5 != 0)
                {
                    subject += "Online Bill Status" + ",";
                }

                if (filteredmodel.RatingNo6 != 0)
                {
                    subject += "Balance Confirmation" + ",";
                }

                subject = subject.TrimEnd(',');

                var bd = "<table width='100%' border='0' cellspacing='0' cellpadding='0'><tr><td valign='top'><font color='#00008A' size='2' face='Verdana, Arial, Helvetica, sans-serif'>" +
                    " To, <br /> MDL Website Administrator, <br />Mazagon Dock Shipbuilders Limited,<br>Mumbai-400010.<br /><br>Following Feedback is" +
                    "Received from " + vigi.Name + " <br /><br /></td></tr><tr><td><br><br><table width='70%' cellspacing='0' cellpadding='2' style='border:solid 1px navy'>";

                if (!string.IsNullOrEmpty(vigi.Name))
                {
                    bd += "<tr style='border:solid 1px navy'><td align='left' valign='top' style='border:solid 1px navy'><font size='2' color='#00008A' face='Verdana'>";
                    bd += "<b>Name : </b></font></td><td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'>";
                    bd += "<b>" + vigi.Name + "</b></font></td></tr>";
                }

                if (!string.IsNullOrEmpty(vigi.Designation))
                {
                    bd += "<tr style='border:solid 1px navy'><td align='left' valign='top' style='border:solid 1px navy'><font size='2' color='#00008A' face='Verdana'>";
                    bd += "<b>Designation : </b></font></td><td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'>";
                    bd += "<b>" + vigi.Designation + "</b></font></td></tr>";
                }

                if (!string.IsNullOrEmpty(vigi.Organization))
                {
                    bd += "<tr style='border:solid 1px navy'><td align='left' valign='top' style='border:solid 1px navy'><font size='2' color='#00008A' face='Verdana'>";
                    bd += "<b>Organization : </b></font></td><td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'>";
                    bd += "<b>" + vigi.Organization + "</b></font></td></tr>";
                }

                if (!string.IsNullOrEmpty(vigi.Address))
                {
                    bd += "<tr style='border:solid 1px navy'><td align='left' valign='top' style='border:solid 1px navy'><font size='2' color='#00008A' face='Verdana'>";
                    bd += "<b>Address : </b></font></td><td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'>";
                    bd += "<b>" + vigi.Address + "</b></font></td></tr>";
                }

                if (!string.IsNullOrEmpty(vigi.City))
                {
                    bd += "<tr style='border:solid 1px navy'><td align='left' valign='top' style='border:solid 1px navy'><font size='2' color='#00008A' face='Verdana'>";
                    bd += "<b>City : </b></font></td><td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'>";
                    bd += "<b>" + vigi.City + "</b></font></td></tr>";
                }

                if (!string.IsNullOrEmpty(vigi.Email))
                {
                    bd += "<tr style='border:solid 1px navy'><td align='left' valign='top' style='border:solid 1px navy'><font size='2' color='#00008A' face='Verdana'>";
                    bd += "<b>Email ID : </b></font></td><td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'>";
                    bd += "<b>" + vigi.Email + "</b></font></td></tr>";
                }

                if (!string.IsNullOrEmpty(vigi.Comment))
                {
                    bd += "<tr style='border:solid 1px navy'><td align='left' valign='top' style='border:solid 1px navy'><font size='2' color='#00008A' face='Verdana'>";
                    bd += "<b>Comments : </b></font></td><td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'>";
                    bd += "<b>" + vigi.Comment + "</b></font></td></tr>";
                }

                bd += "</table><table width='100%' border='0' cellspacing='0' cellpadding='0'><tr><td valign='top'>";
                bd += "<tr style='border:solid 1px navy'><td align='left' valign='top' style='border:solid 1px navy'><font size='2' color='#00008A' face='Verdana'>";
                bd += "<b>Parameter</b></font></td><td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'>";
                bd += "<b>Rating</b></font></td></tr>";

                if (filteredmodel.RatingNo1 != 0)
                {
                    bd += "<tr style='border:solid 1px navy'><td align='left' valign='top' style='border:solid 1px navy'><font size='2' color='#00008A' face='Verdana'>";
                    bd += "<b>MDL website</b></font></td><td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'>";
                    bd += "<b>" + vigi.RatingNo1 + "</b></font></td></tr>";
                }

                if (filteredmodel.RatingNo2 != 0)
                {
                    bd += "<tr style='border:solid 1px navy'><td align='left' valign='top' style='border:solid 1px navy'><font size='2' color='#00008A' face='Verdana'>";
                    bd += "<b>MDL Online Recruitment </b></font></td><td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'>";
                    bd += "<b>" + vigi.RatingNo2 + "</b></font></td></tr>";
                }

                if (filteredmodel.RatingNo3 != 0)
                {
                    bd += "<tr style='border:solid 1px navy'><td align='left' valign='top' style='border:solid 1px navy'><font size='2' color='#00008A' face='Verdana'>";
                    bd += "<b>MDL Online Vendor Registration </b></font></td><td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'>";
                    bd += "<b>" + vigi.RatingNo3 + "</b></font></td></tr>";
                }

                if (filteredmodel.RatingNo4 != 0)
                {
                    bd += "<tr style='border:solid 1px navy'><td align='left' valign='top' style='border:solid 1px navy'><font size='2' color='#00008A' face='Verdana'>";
                    bd += "<b>MDL E-Procurement </ b></font></td><td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'>";
                    bd += "<b>" + vigi.RatingNo4 + "</b></font></td></tr>";
                }

                if (filteredmodel.RatingNo5 != 0)
                {
                    bd += "<tr style='border:solid 1px navy'><td align='left' valign='top' style='border:solid 1px navy'><font size='2' color='#00008A' face='Verdana'>";
                    bd += "<b>MDL Online Bill Status </b></font></td><td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'>";
                    bd += "<b>" + vigi.RatingNo5 + "</b></font></td></tr>";
                }

                if (filteredmodel.RatingNo6 != 0)
                {
                    bd += "<tr style='border:solid 1px navy'><td align='left' valign='top' style='border:solid 1px navy'><font size='2' color='#00008A' face='Verdana'>";
                    bd += "<b>MDL Vendor Balance Confirmation </b></font></td><td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'>";
                    bd += "<b>" + vigi.RatingNo6 + "</b></font></td></tr>";
                }

                if (!string.IsNullOrEmpty(filteredmodel.ReasonSatisfied))
                {
                    bd += "<tr style='border:solid 1px navy'><td align='left' valign='top' style='border:solid 1px navy'><font size='2' color='#00008A' face='Verdana'>";
                    bd += "<b>Where you were Highly Satisfied</b></font></td><td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'>";
                    bd += "<b>" + filteredmodel.ReasonSatisfied + "</b></font></td></tr>";
                }

                if (!string.IsNullOrEmpty(filteredmodel.ReasonDisSatisfied))
                {
                    bd += "<tr style='border:solid 1px navy'><td align='left' valign='top' style='border:solid 1px navy'><font size='2' color='#00008A' face='Verdana'>";
                    bd += "<b>Where you were Dissatisfied : </b></font></td><td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'>";
                    bd += "<b>" + filteredmodel.ReasonDisSatisfied + "</b></font></td></tr>";
                }

                bd += "</table></td></tr><tr><td><br>Kindly go through these details. <br /><br>Thanks.<br></p>";
                bd += "<font color='#00008A' size='2' face='Verdana, Arial, Helvetica, sans-serif'><hr><br>";
                bd += "Note: This email is generated by Feedback module of MDL website ( <a href='http://www.mazagondock.in/' target='_blank'><font color='#990000'>www.mazagondock.in</font></a> ).</td></tr>";
                bd += "</table>";

                try
                {
                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress(_config["SMTPFrom"]);
                    mail.To.Add(new MailAddress("feedback@mazdock.com"));
                    mail.IsBodyHtml = true;
                    mail.Subject = subject;
                    mail.Body = bd;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = _config["SMTPHost"];
                    smtp.Send(mail);


                    //Send email
                    //var EmailVm = new EmailVM
                    //{
                    //    ToAddresses = new List<string> { "feedback@mazdock.com" },
                    //    Subject = subject,
                    //    Body = bd
                    //};
                   // await _emailService.SendEmailAsync(EmailVm).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _notyf.Error(ex.Message);
                }
            }

            if (vigi == null)
                _notyf.Error("Save failed");
            else
                _notyf.Success("Saved successfully");
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostcreatenew()
        {
            var response = await _httpClient.PostAsync("Test/CreateTest", false, TEST).ConfigureAwait(false);
            TEST = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<TestDTO>(response) : null;
            return RedirectToPage("/Forms/Feedback");
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