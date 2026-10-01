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
using System;
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

        private const string SESSION_VERIFIED_EMAIL = "Vigi_VerifiedEmail";

        public RegisterComplaintModel(
            IHttpClientService httpClient,
            INotyfService notyf,
            IEmailService emailService,
            IFileService fileService,
            IConfiguration config)
        {
            _httpClient = httpClient;
            _notyf = notyf;
            _emailService = emailService;
            _fileService = fileService;
            _config = config;
        }

        [BindProperty] public VigilanceFormDTO vigi { get; set; }
        public int ComplaintSubmitted { get; set; }
        public string VerifiedEmail { get; set; }

        // ═══════════════════════════════════════════════════════════
        //  GET
        // ═══════════════════════════════════════════════════════════
        public IActionResult OnGet()
        {
            ComplaintSubmitted = 0;
            var verifiedEmail = HttpContext.Session.GetString(SESSION_VERIFIED_EMAIL);
            if (!string.IsNullOrEmpty(verifiedEmail))
            {
                var rec = VigilanceOtpStore.Get(verifiedEmail);
                if (rec != null && rec.IsVerified
                    && !VigilanceOtpStore.IsVerificationExpired(rec))
                {
                    VerifiedEmail = verifiedEmail;
                    vigi = new VigilanceFormDTO { Email = verifiedEmail };
                }
                else
                {
                    HttpContext.Session.Remove(SESSION_VERIFIED_EMAIL);
                }
            }
            return Page();
        }

        // ═══════════════════════════════════════════════════════════
        //  STEP 1: Send OTP
        // ═══════════════════════════════════════════════════════════
        public async Task<IActionResult> OnPostSendOtpAsync(string email)
        {
            System.Diagnostics.Debug.WriteLine($"🔥 OnPostSendOtpAsync HIT — email: {email}");

            if (string.IsNullOrWhiteSpace(email))
                return new JsonResult(new { success = false, message = "Email is required." });

            try
            {
                var response = await _httpClient
                    .PostAsync("Vigilance/SubmitEmail", false, new { Email = email })
                    .ConfigureAwait(false);

                System.Diagnostics.Debug.WriteLine($"📥 API Response: {response}");

                if (string.IsNullOrEmpty(response))
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Unable to process request. Please try again."
                    });

                try
                {
                    var result = JsonConvert.DeserializeObject<dynamic>(response);

                    // ✅ API ne success=false diya — uska message forward karo
                    if (result != null && result.success == false)
                    {
                        return new JsonResult(new
                        {
                            success = false,
                            message = (string)(result.message ?? "Failed to send OTP.")
                        });
                    }

                    return new JsonResult(result);
                }
                catch
                {
                    return new JsonResult(new { success = false, message = "Invalid response from server." });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("═══════ SendOtp EXCEPTION ═══════");
                System.Diagnostics.Debug.WriteLine("Message: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("═════════════════════════════════");

                return new JsonResult(new
                {
                    success = false,
                    message = !string.IsNullOrEmpty(ex.Message) ? ex.Message : "Failed to send OTP."
                });
            }
        }

        // ═══════════════════════════════════════════════════════════
        //  STEP 2: Verify OTP
        // ═══════════════════════════════════════════════════════════
        public async Task<IActionResult> OnPostVerifyOtpAsync(string email, string otp)
        {
            System.Diagnostics.Debug.WriteLine($"🔥 OnPostVerifyOtpAsync HIT — email: {email}");

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(otp))
                return new JsonResult(new { success = false, message = "Email and OTP are required." });

            try
            {
                var response = await _httpClient
                    .PostAsync("Vigilance/VerifyOtp", false, new { Email = email, Otp = otp })
                    .ConfigureAwait(false);

                System.Diagnostics.Debug.WriteLine($"📥 API Response: {response}");

                if (string.IsNullOrEmpty(response))
                    return new JsonResult(new { success = false, message = "Unable to verify OTP. Please try again." });

                try
                {
                    var result = JsonConvert.DeserializeObject<dynamic>(response);

                    if (result != null && result.success == true)
                    {
                        HttpContext.Session.SetString(SESSION_VERIFIED_EMAIL, email.Trim());
                    }

                    if (result != null && result.success == false)
                    {
                        return new JsonResult(new
                        {
                            success = false,
                            message = (string)(result.message ?? "Invalid OTP.")
                        });
                    }

                    return new JsonResult(result);
                }
                catch
                {
                    return new JsonResult(new { success = false, message = "Invalid response from server." });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = !string.IsNullOrEmpty(ex.Message) ? ex.Message : "Verification failed."
                });
            }
        }

        // ═══════════════════════════════════════════════════════════
        //  STEP 2b: Resend OTP
        // ═══════════════════════════════════════════════════════════
        public async Task<IActionResult> OnPostResendOtpAsync(string email)
        {
            System.Diagnostics.Debug.WriteLine($"🔥 OnPostResendOtpAsync HIT — email: {email}");

            if (string.IsNullOrWhiteSpace(email))
                return new JsonResult(new { success = false, message = "Email is required." });

            try
            {
                var response = await _httpClient
                    .PostAsync("Vigilance/ResendOtp", false, new { Email = email })
                    .ConfigureAwait(false);

                System.Diagnostics.Debug.WriteLine($"📥 API Response: {response}");

                if (string.IsNullOrEmpty(response))
                    return new JsonResult(new { success = false, message = "Unable to resend OTP. Please try again." });

                try
                {
                    var result = JsonConvert.DeserializeObject<dynamic>(response);

                    if (result != null && result.success == false)
                    {
                        return new JsonResult(new
                        {
                            success = false,
                            message = (string)(result.message ?? "Resend failed.")
                        });
                    }

                    return new JsonResult(result);
                }
                catch
                {
                    return new JsonResult(new { success = false, message = "Invalid response from server." });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = !string.IsNullOrEmpty(ex.Message) ? ex.Message : "Resend failed."
                });
            }
        }

        // ═══════════════════════════════════════════════════════════
        //  STEP 3: Final Submit
        // ═══════════════════════════════════════════════════════════
        public async Task<IActionResult> OnPostComplaint()
        {
            var verifiedEmail = HttpContext.Session.GetString(SESSION_VERIFIED_EMAIL);

            if (string.IsNullOrEmpty(verifiedEmail))
            {
                _notyf.Error("Email not verified. Please verify your email first.");
                return Page();
            }

            if (!string.Equals(verifiedEmail, vigi.Email?.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                _notyf.Error("Email does not match the verified email.");
                return Page();
            }

            if (!Captcha.ValidateCaptchaCode(vigi.CaptchaCode, HttpContext))
            {
                ModelState.AddModelError("vigi.CaptchaCode", "Please enter correct captcha");
            }

            vigi.SubmitOn = DateTime.Now;

            var filteredmodel = new VigilanceFormDTO
            {
                Id = vigi.Id,
                Name = objbal.OnlyValid(vigi.Name),
                Designation = objbal.OnlyValid(vigi.Designation),
                Organization = objbal.OnlyValid(vigi.Organization),
                Email = objbal.OnlyValid(vigi.Email),
                Contact = objbal.OnlyValid(vigi.Contact),
                Fax = vigi.Fax,
                PostalAddress = objbal.OnlyValid(vigi.PostalAddress),
                ComplaintDetails = objbal.OnlyValid(vigi.ComplaintDetails),
                ShipBuildRelated = vigi.ShipBuildRelated,
                SubHeavyRelated = vigi.SubHeavyRelated,
                IP = vigi.IP,
                SubmitOn = vigi.SubmitOn,
                CaptchaCode = vigi.CaptchaCode
            };

            vigi = ModelAuditor<VigilanceFormDTO>.SetAudit(
                User.Identity.Name,
                "Create",
                HttpContext.Connection.RemoteIpAddress.ToString(),
                filteredmodel);

            if (!ModelState.IsValid)
            {
                _notyf.Error(ModelState.GetErrorMessageString());
                return Page();
            }

            var response = await _httpClient
                .PostAsync("Vigilance/Create", false, filteredmodel)
                .ConfigureAwait(false);

            vigi = !string.IsNullOrEmpty(response)
                ? JsonConvert.DeserializeObject<VigilanceFormDTO>(response)
                : null;

            if (vigi != null)
            {
                // ── Build email body (existing logic) ──
                string strbody = "";
                strbody += "<table width='100%' border='1' cellspacing='0' cellpadding='0'><tr><td valign='top' style='text-align:center'><b style='font-size:18px'><u> Complaint Received Online through MDL Website </u></b></td></tr> <tr><td valign='top'>";
                strbody += "<font color='#00008A' size='2' face='Verdana, Arial, Helvetica, sans-serif'> ";
                strbody += "<br /> <b>To,</b> <br /> <b>The Chief Vigilance Officer,</b> <br /> <b>Mazagon Dock Shipbuilders Ltd</b>";
                strbody += "<br /></td></tr><tr><td><br><br><table width='70%' cellspacing='0' cellpadding='2' style='border:solid 1px navy'><tr style='border:solid 1px navy'>";

                strbody += "<td width='25%' align='left' valign='top' style='border:solid 1px navy'><font size='2' color='#00008A' face='Verdana'><b>Name</b></font></td>";
                strbody += "<td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'><b>" + vigi.Name + "</b></font></td></tr>";
                strbody += "<tr style='border:solid 1px navy'><td align='left' valign='top' style='border:solid 1px navy'><font size='2' color='#00008A' face='Verdana'><b>Designation</b></font></td>";
                strbody += "<td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'><b>" + vigi.Designation + "</b></font></td></tr>";
                strbody += "<tr style='border:solid 1px navy'><td align='left' valign='top' style='border:solid 1px navy'><font size='2' color='#00008A' face='Verdana'><b>Organization</b></font></td>";
                strbody += "<td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'><b>" + vigi.Organization + "</b></font></td></tr>";
                strbody += "<tr style='border:solid 1px navy'><td align='left' valign='top' style='border:solid 1px navy'><font size='2' color='#00008A' face='Verdana'><b>Email ID</b></font></td>";
                strbody += "<td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'><b>" + vigi.Email + "</b></font></td></tr>";
                strbody += "<tr style='border:solid 1px navy'><td align='left' valign='top' style='border:solid 1px navy'><font size='2' color='#00008A' face='Verdana'><b>Postal Address</b></font></td>";
                strbody += "<td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'><b>" + vigi.PostalAddress + "</b></font></td></tr>";
                strbody += "<tr style='border:solid 1px navy'><td align='left' valign='top' style='border:solid 1px navy'><font size='2' color='#00008A' face='Verdana'><b>Contact No</b></font></td>";
                strbody += "<td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'><b>" + vigi.Contact + "</b></font></td></tr>";
                strbody += "<tr style='border:solid 1px navy'><td align='left' valign='top' style='border:solid 1px navy'><font size='2' color='#00008A' face='Verdana'><b>Fax No</b></font></td>";
                strbody += "<td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'><b>" + vigi.Fax + "</b></font></td></tr>";
                strbody += "<tr style='border:solid 1px navy'><td align='left' valign='top' style='border:solid 1px navy'><font size='2' color='#00008A' face='Verdana'><b>Complaint Details</b></font></td>";
                strbody += "<td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'><b>" + vigi.ComplaintDetails + "</b></font></td></tr>";

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

                // ✅ Email — try-catch mein wrap karo, SMTP fail hone pe crash na kare
                try
                {
                    var fromAddress = _config["EmailSettings:FromAddress"];
                    var displayName = _config["EmailSettings:DisplayName"];
                    var host = _config["EmailSettings:Host"];
                    var portStr = _config["EmailSettings:Port"];
                    var userName = _config["EmailSettings:UserName"];
                    var password = _config["EmailSettings:Password"];

                    int port = int.TryParse(portStr, out var p) ? p : 587;

                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress(fromAddress, displayName);
                    mail.To.Add(new MailAddress("cvo@mazdock.com"));
                    mail.IsBodyHtml = true;
                    mail.Subject = "Complaint Received Online Through MDL Website";
                    mail.Body = strbody;

                    SmtpClient smtp = new SmtpClient
                    {
                        Host = host,
                        Port = port,
                        EnableSsl = true,
                        UseDefaultCredentials = false,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        Credentials = new System.Net.NetworkCredential(userName, password)
                    };

                    smtp.Send(mail);
                    System.Diagnostics.Debug.WriteLine("✅ CVO email sent");
                }
                catch (Exception mailEx)
                {
                    System.Diagnostics.Debug.WriteLine("═══════ CVO EMAIL FAILED ═══════");
                    System.Diagnostics.Debug.WriteLine("Message: " + mailEx.Message);
                    // Complaint DB mein already save — continue
                }

                HttpContext.Session.Remove(SESSION_VERIFIED_EMAIL);
            }

            if (vigi == null)
                _notyf.Error("Save failed");
            else
                ComplaintSubmitted = 1;

            return Page();
        }

        public IActionResult OnPostTest()
        {
            _notyf.Success("Saved successfully");
            return Page();
        }

        public async Task<IActionResult> OnGetValidatecapcha(string CaptchaCode)
        {
            var unameResult = await _httpClient
                .GetAsync("Auth/CheckUsername", false, CaptchaCode)
                .ConfigureAwait(false);
            var ccode = HttpContext.Session.GetString("CaptchaCode");
            return new JsonResult(ccode);
        }
    }
}