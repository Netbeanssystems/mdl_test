using Application.Dtos;
using Application.Extensions;
using Application.Helpers;
using Application.ServiceInterfaces;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Threading.Tasks;
using WebApp.Extensions;

namespace WebSite.Pages.Forms
{


    public class RegisterComplaintModel : PageModel
    {
        Validate objbal = new Validate();
        private readonly IHttpClientService _httpClient;
        private readonly INotyfService _notyf;
        private readonly IEmailService _emailService;
        private readonly IFileService _fileService;
        public readonly IConfiguration _config;

        public RegisterComplaintModel(IHttpClientService httpClient, INotyfService notyf, IEmailService emailService, IFileService fileService, IConfiguration config)
        {
            _httpClient = httpClient;
            _notyf = notyf;
            _emailService = emailService;
            _fileService = fileService;
            _config = config;
        }

        [BindProperty] public VigilanceFormDTO vigi { get; set; }

        public int ComplaintSubmitted { get; set; }


        public IActionResult OnGet()
        {
            ComplaintSubmitted = 0;
            return Page();
        }

        public async Task<IActionResult> OnPostComplaint()
        {
            if (!Captcha.ValidateCaptchaCode(vigi.CaptchaCode, HttpContext))
            {
                ModelState.AddModelError("Captcha", "Please enter correct captcha");
            }

            vigi.SubmitOn = System.DateTime.Now;
            var filteredmodel = new VigilanceFormDTO();
            filteredmodel.Id = vigi.Id;
            filteredmodel.Name = objbal.OnlyValid(vigi.Name);
            filteredmodel.Designation = objbal.OnlyValid(vigi.Designation);
            filteredmodel.Organization = objbal.OnlyValid(vigi.Organization);
            filteredmodel.Email = objbal.OnlyValid(vigi.Email);
            filteredmodel.Contact = objbal.OnlyValid(vigi.Contact);
            filteredmodel.Fax = vigi.Fax;
            filteredmodel.PostalAddress = objbal.OnlyValid(vigi.PostalAddress);
            filteredmodel.ComplaintDetails = objbal.OnlyValid(vigi.ComplaintDetails);
            filteredmodel.ShipBuildRelated = vigi.ShipBuildRelated;
            filteredmodel.SubHeavyRelated = vigi.SubHeavyRelated;
            filteredmodel.IP = vigi.IP;
            filteredmodel.SubmitOn = vigi.SubmitOn;
            filteredmodel.CaptchaCode = vigi.CaptchaCode;

            if (vigi.UploadFile != null)
            {
                //...........Check Valid File ...................
                if (!_fileService.CheckValidFile(vigi.UploadFile))
                {
                    //......Redirect  
                    _notyf.Error("Please Upload Valid File PDF Only");
                    return Page();
                }
                filteredmodel.UploadFileName = await _fileService.SaveImageAsync(@"\img\Uploads\VigilanceForm\", vigi.UploadFile);
                filteredmodel.UploadFile = null;
            }

            vigi = ModelAuditor<VigilanceFormDTO>.SetAudit(User.Identity.Name, "Create", HttpContext.Connection.RemoteIpAddress.ToString(), filteredmodel);
            if (!ModelState.IsValid) { _notyf.Error(ModelState.GetErrorMessageString()); return Page(); }
            var response = await _httpClient.PostAsync("Vigilance/Create", false, filteredmodel).ConfigureAwait(false);

            vigi = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<VigilanceFormDTO>(response) : null;

            if (vigi != null)
            {
                // Mail 1
                string strbody = "";
                strbody += "<table width='100%' border='1' cellspacing='0' cellpadding='0'><tr><td valign='top' style='text-align:center'><b style='font-size:18px'><u> Complaint Received Online through MDL Website </u></b></td></tr> <tr><td valign='top'>";
                strbody += "<font color='#00008A' size='2' face='Verdana, Arial, Helvetica, sans-serif'> ";
                strbody += "<br /> <b>To,</b> <br /> <b>The Chief Vigilance Officer,</b> <br /> <b>Mazagon Dock Shipbuilders Ltd</b>";
                strbody += "<br /></td></tr><tr><td><br><br><table width='70%' cellspacing='0' cellpadding='2' style='border:solid 1px navy'><tr style='border:solid 1px navy'>";

                strbody += "<td width='25%' align='left' valign='top' style='border:solid 1px navy'><font size='2' color='#00008A' face='Verdana'><b>Name</b></font></td>";
                strbody += "<td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'><b>" + vigi.Name + "</b></font></td></tr><tr style='border:solid 1px navy'>";
                strbody += "<td align='left' valign='top' style='border:solid 1px navy'><font size='2' color='#00008A' face='Verdana'><b>Designation</b></font></td>";
                strbody += "<td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'><b>" + vigi.Designation + "</b></font></td></tr>";
                strbody += "<tr style='border:solid 1px navy'><td align='left' valign='top' style='border:solid 1px navy'><font size='2' color='#00008A' face='Verdana'><b>Organization</b></font>";
                strbody += " </td><td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'><b>" + vigi.Organization + "</b></font>";
                strbody += "</td></tr><tr style='border:solid 1px navy'><td align='left' valign='top' style='border:solid 1px navy'><font size='2' color='#00008A' face='Verdana'>";

                strbody += "<b>Email ID</b></font></td><td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'>";
                strbody += "<b>" + vigi.Email + "</b></font></td></tr><tr style='border:solid 1px navy'><td align='left' valign='top' style='border:solid 1px navy'>";
                strbody += "<font size='2' color='#00008A' face='Verdana'><b stle='font-size:16px'>Postal Address</b></font></td><td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'>";
                strbody += "<b stle='font-size:16px'>" + vigi.PostalAddress + "</b></font></td></tr><tr style='border:solid 1px navy'><td align='left' valign='top' style='border:solid 1px navy'><font size='2' color='#00008A' face='Verdana'>";
                strbody += "<b>Contact No</b></font></td><td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'><b>" + vigi.Contact + "</b>";
                strbody += "</font></td></tr><tr style='border:solid 1px navy'><td align='left' valign='top' style='border:solid 1px navy'><font size='2' color='#00008A' face='Verdana'><b>Fax No</b></font>";
                strbody += "</td><td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'><b>" + vigi.Fax + "</b></font></td></tr>";

                strbody += "<tr style='border:solid 1px navy'><td align='left' valign='top' style='border:solid 1px navy'><font size='2' color='#00008A' face='Verdana'><b>Complaint Details</b></font>";
                strbody += "</td><td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'><b>" + vigi.ComplaintDetails + "</b></font></td></tr>";

                if (vigi.UploadFileName != null)
                {
                    var fileURL = "https://mazagondock.in/img/Uploads/VigilanceForm/" + vigi.UploadFileName.Trim();
                    strbody += "<tr style='border:solid 1px navy'><td align='left' valign='top' style='border:solid 1px navy'><font size='2' color='#00008A' face='Verdana'><b>Attachment (if any) </b></font>";
                    strbody += "</td><td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'><b> <a href='" + fileURL + "' target'_blank'> View File</a></b></font></td></tr></table>";

                }
                else
                {
                    strbody += "<tr style='border:solid 1px navy'><td align='left' valign='top' style='border:solid 1px navy'><font size='2' color='#00008A' face='Verdana'><b>Attachment (if any) </b></font>";
                    strbody += "</td><td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'><b>NA</b></font></td></tr></table>";

                }

                strbody += "</td></tr><tr><td><br>Kindly go through these details. <br /><br>Thanks.<br></p>";
                strbody += "<font color='#00008A' size='2' face='Verdana, Arial, Helvetica, sans-serif'><hr><br>";
                strbody += "Note: This email is generated by Vigilance Complaint Register of MDL website ( <a href='http://www.mazagondock.in/' target='_blank'><font color='#990000'>www.mazagondock.in</font></a> ).</td></tr>";
                strbody += "</table>";

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(_config["SMTPFrom"]);
                // mail.To.Add(new MailAddress("mdlvigilance_complaint@mazdock.com")); //Old Mail
                mail.To.Add(new MailAddress("cvo@mazdock.com")); // New 
                                                                 // mail.To.Add(new MailAddress("mdlvigilance_complaint@mazdock.com")); // New 
                                                                 //mail.To.Add(new MailAddress("akashprajapatig220@gmail.com")); // New 
                mail.IsBodyHtml = true;
                //mail.Subject = "MAZAGON DOCK SHIPBUILDERS LIMITED :: Vigilance Complaint"; // Old
                mail.Subject = "Complaint Received Online Through MDL Website";// New
                mail.Body = strbody;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = _config["SMTPHost"];
                smtp.Send(mail);


                //var EmailVm = new EmailVM
                //{
                //    ToAddresses = new List<string> { vigi.Email, "akumar@planetecomsolutions.com" },
                //    Subject = "Complaint Received Online Through MDL Website",
                //    Body = strbody,
                //    IsBodyHtml = true
                //};
                //try
                //{
                //    await _emailService.SendEmailAsync(EmailVm);
                //}
                //catch (Exception ex)
                //{

                //}

                // Mail 2
                //string bd = "";
                //bd += "<table width='100%' border='1' cellspacing='0' cellpadding='0'><tr><td valign='top'>";
                //bd += "<font color='#00008A' size='2' face='Verdana, Arial, Helvetica, sans-serif'> ";
                //bd += " To, <br /> " + vigi.Name + "<br /><br>We have received following Vigilance Enquiry is Received.<br />";
                //bd += "<br /></td></tr><tr><td><br><br><table width='70%' cellspacing='0' cellpadding='2' style='border:solid 1px navy'><tr style='border:solid 1px navy'>";

                //bd += "<td width='25%' align='left' valign='top' style='border:solid 1px navy'><font size='2' color='#00008A' face='Verdana'><b>Name</b></font></td>";
                //bd += "<td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'><b>" + vigi.Name + "</b></font></td></tr><tr style='border:solid 1px navy'>";
                //bd += "<td align='left' valign='top' style='border:solid 1px navy'><font size='2' color='#00008A' face='Verdana'><b>Designation</b></font></td>";
                //bd += "<td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'><b>" + vigi.Designation + "</b></font></td></tr>";
                //bd += "<tr style='border:solid 1px navy'><td align='left' valign='top' style='border:solid 1px navy'><font size='2' color='#00008A' face='Verdana'><b>Organization</b></font>";
                //bd += " </td><td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'><b>" + vigi.Organization + "</b></font>";
                //bd += "</td></tr><tr style='border:solid 1px navy'><td align='left' valign='top' style='border:solid 1px navy'><font size='2' color='#00008A' face='Verdana'>";

                //bd += "<b>Email ID</b></font></td><td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'>";
                //bd += "<b>" + vigi.Email + "</b></font></td></tr><tr style='border:solid 1px navy'><td align='left' valign='top' style='border:solid 1px navy'>";
                //bd += "<font size='2' color='#00008A' face='Verdana'><b>Postal Address</b></font></td><td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'>";
                //bd += "<b>" + vigi.PostalAddress + "</b></font></td></tr><tr style='border:solid 1px navy'><td align='left' valign='top' style='border:solid 1px navy'><font size='2' color='#00008A' face='Verdana'>";
                //bd += "<b>Contact No</b></font></td><td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'><b>" + vigi.Contact + "</b>";
                //bd += "</font></td></tr><tr style='border:solid 1px navy'><td align='left' valign='top' style='border:solid 1px navy'><font size='2' color='#00008A' face='Verdana'><b>Fax No</b></font>";
                //bd += "</td><td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'><b>" + vigi.Fax + "</b></font></td></tr>";

                //bd += "<tr style='border:solid 1px navy'><td align='left' valign='top' style='border:solid 1px navy'><font size='2' color='#00008A' face='Verdana'><b>Complaint Details</b></font>";
                //bd += "</td><td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'><b>" + vigi.ComplaintDetails + "</b></font></td></tr></table>";

                //bd += "</td></tr><tr><td><br>Kindly go through these details. <br /><br>Thanks.<br></p>";
                //bd += "<font color='#00008A' size='2' face='Verdana, Arial, Helvetica, sans-serif'><hr><br>";
                //bd += "Note: This email is generated by Vigilance Complaint Register of MDL website ( <a href='http://www.mazagondock.in/' target='_blank'><font color='#990000'>www.mazagondock.in</font></a> ).</td></tr>";
                //bd += "</table>";

                //MailMessage mail1 = new MailMessage();
                //mail1.From = new MailAddress(_config["SMTPFrom"]);
                //mail1.To.Add(new MailAddress("mdlvigilance_complaint@mazdock.com"));
                //mail1.IsBodyHtml = true;
                //mail1.Subject = "MAZAGON DOCK SHIPBUILDERS LIMITED :: Vigilance Complaint";
                //mail1.Body = bd;
                //SmtpClient smtp1 = new SmtpClient();
                //smtp1.Host = _config["SMTPHost"];
                //smtp1.Send(mail1);
            }


            if (filteredmodel == null)
                _notyf.Error("Save failed");
            else
                ComplaintSubmitted = 1;
            //  _notyf.Success("Saved successfully");
            //  return RedirectToPage("/Forms/RegisterComplaint");
            return Page();
        }

        public IActionResult OnPostTest()
        {
            _notyf.Success("Saved successfully");
            return Page();
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
