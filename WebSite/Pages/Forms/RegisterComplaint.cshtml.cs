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
using System.IO;
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
        private const string EMAIL_LOG_FILE = "email-failure.log";

        // ═══════════════════════════════════════════════════════════
        //  ⚠️ TESTING SWITCH
        //  false → EmailSettings (Gmail) — TESTING
        //  true  → SMTPHost (internal relay) — PRODUCTION
        // ═══════════════════════════════════════════════════════════
        private const bool USE_SMTP_HOST = true;

        // ⚠️ TESTING recipient (CVO notification testing ke liye)
        private const string TEST_RECIPIENT = "netbeans.nitesh@gmail.com";

        // ✅ PRODUCTION recipient — CVO dedicated email
        private const string PRODUCTION_CVO_RECIPIENT = "cvo@mazdock.com";

        // ✅ Subject — same as old working code
        private const string CVO_EMAIL_SUBJECT = "Complaint Received Online Through MDL Website";

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
        //  LOGGERS
        // ═══════════════════════════════════════════════════════════
        private void LogEmailFailure(string recipient, string subject, Exception ex)
        {
            try
            {
                var logPath = System.IO.Path.Combine(AppContext.BaseDirectory, EMAIL_LOG_FILE);

                var logEntry = $@"
═══════════════════════════════════════════════════════════════
TIMESTAMP : {DateTime.Now:yyyy-MM-dd HH:mm:ss}
RECIPIENT : {recipient ?? "N/A"}
SUBJECT   : {subject ?? "N/A"}
TYPE      : {ex?.GetType().FullName ?? "Unknown"}
MESSAGE   : {ex?.Message ?? "No message"}
INNER     : {ex?.InnerException?.Message ?? "N/A"}
STACK     :
{ex?.StackTrace ?? "N/A"}
═══════════════════════════════════════════════════════════════
";

                System.IO.File.AppendAllText(logPath, logEntry);
            }
            catch { }
        }

        private void LogEmailSuccess(string recipient, string subject)
        {
            try
            {
                var logPath = System.IO.Path.Combine(AppContext.BaseDirectory, EMAIL_LOG_FILE);
                System.IO.File.AppendAllText(logPath,
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ✅ SUCCESS → {recipient} | {subject}{Environment.NewLine}");
            }
            catch { }
        }

        // ═══════════════════════════════════════════════════════════
        //  UNIFIED EMAIL SENDER
        //  Testing  → EmailSettings (Gmail)
        //  Production → SMTPHost (internal relay — same as old code)
        // ═══════════════════════════════════════════════════════════
        private bool SendEmailUnified(string toAddress, string subject, string htmlBody)
        {
            try
            {
                MailMessage mail = new MailMessage
                {
                    IsBodyHtml = true,
                    Subject = subject,
                    Body = htmlBody
                };
                mail.To.Add(new MailAddress(toAddress));

                SmtpClient smtp;

                if (USE_SMTP_HOST)
                {
                    // ═══════════════════════════════════════════════
                    //  PRODUCTION — Internal SMTP relay (no auth)
                    //  SAME as old working code: sirf Host set
                    // ═══════════════════════════════════════════════
                    var smtpHost = _config["SMTPHost"];
                    var smtpFrom = _config["SMTPFrom"];

                    if (string.IsNullOrWhiteSpace(smtpHost) || string.IsNullOrWhiteSpace(smtpFrom))
                    {
                        LogEmailFailure(toAddress, subject,
                            new Exception("SMTPHost or SMTPFrom missing in appsettings."));
                        return false;
                    }

                    mail.From = new MailAddress(smtpFrom);

                    smtp = new SmtpClient
                    {
                        Host = smtpHost
                        // Port 25 default, no SSL, no credentials
                    };
                }
                else
                {
                    // ═══════════════════════════════════════════════
                    //  TESTING — Gmail SMTP (with auth)
                    // ═══════════════════════════════════════════════
                    var fromAddress = _config["EmailSettings:FromAddress"];
                    var displayName = _config["EmailSettings:DisplayName"];
                    var host = _config["EmailSettings:Host"];
                    var portStr = _config["EmailSettings:Port"];
                    var userName = _config["EmailSettings:UserName"];
                    var password = _config["EmailSettings:Password"];

                    if (string.IsNullOrWhiteSpace(fromAddress) || string.IsNullOrWhiteSpace(host))
                    {
                        LogEmailFailure(toAddress, subject,
                            new Exception("EmailSettings missing in appsettings."));
                        return false;
                    }

                    int port = int.TryParse(portStr, out var p) ? p : 587;

                    mail.From = new MailAddress(fromAddress, displayName);

                    smtp = new SmtpClient
                    {
                        Host = host,
                        Port = port,
                        EnableSsl = true,
                        UseDefaultCredentials = false,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        Credentials = new System.Net.NetworkCredential(userName, password)
                    };
                }

                smtp.Send(mail);
                LogEmailSuccess(toAddress, subject);
                return true;
            }
            catch (Exception ex)
            {
                LogEmailFailure(toAddress, subject, ex);
                return false;
            }
        }

        // ═══════════════════════════════════════════════════════════
        //  OTP EMAIL BODY BUILDER — EXACT as before (VigilanceOtpStore)
        // ═══════════════════════════════════════════════════════════
        private string BuildOtpEmailBody(string otp)
        {
            string body = $@"
                    <html><body style='font-family:Verdana,Arial,sans-serif;'>
                    <p>Dear User,</p>
                    <p>Your OTP for <b>Vigilance Complaint Registration</b> on MDL Website is:</p>
                    <h2 style='color:#00008A; letter-spacing:4px;'>{otp}</h2>
                    <p>This OTP is valid for <b>{VigilanceOtpStore.OTP_VALIDITY_MINUTES} minutes</b>.</p>
                    <p>If you did not request this, please ignore this email.</p>
                    <br/>
                    <p>Regards,<br/>MDL Vigilance Team</p>
                    </body></html>";
            return body;
        }

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
            if (string.IsNullOrWhiteSpace(email))
                return new JsonResult(new { success = false, message = "Email is required." });

            try
            {
                var response = await _httpClient
                    .PostAsync("Vigilance/SubmitEmail", false, new { Email = email })
                    .ConfigureAwait(false);

                if (string.IsNullOrEmpty(response))
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Unable to process request. Please try again."
                    });

                dynamic result;
                try
                {
                    result = JsonConvert.DeserializeObject<dynamic>(response);
                }
                catch
                {
                    return new JsonResult(new { success = false, message = "Invalid response from server." });
                }

                // ── ALREADY_SUBMITTED check ──
                if (result != null && result.message != null
                    && result.message.ToString() == "ALREADY_SUBMITTED")
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "ALREADY_SUBMITTED"
                    });
                }

                if (result == null || result.success == false)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = (string)(result?.message ?? "Failed to send OTP.")
                    });
                }

                // ── OTP nikaalo ──
                string otpCode = null;
                try
                {
                    if (result.otp != null)
                        otpCode = (string)result.otp;
                }
                catch { }

                if (string.IsNullOrWhiteSpace(otpCode))
                {
                    LogEmailFailure(email, "OTP Generation",
                        new Exception("WebAPI did not return OTP in response."));
                    return new JsonResult(new
                    {
                        success = false,
                        message = "OTP generation failed. Please try again."
                    });
                }

                // ── Email bhejo ──
                var subject = "MDL Vigilance - Email Verification OTP";
                var body = BuildOtpEmailBody(otpCode);
                var emailSent = SendEmailUnified(email, subject, body);

                if (!emailSent)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "OTP generated but email could not be sent. Please try again."
                    });
                }

                return new JsonResult(new
                {
                    success = true,
                    message = "OTP has been sent to your email address."
                });
            }
            catch (Exception ex)
            {
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
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(otp))
                return new JsonResult(new { success = false, message = "Email and OTP are required." });

            try
            {
                var response = await _httpClient
                    .PostAsync("Vigilance/VerifyOtp", false, new { Email = email, Otp = otp })
                    .ConfigureAwait(false);

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
            if (string.IsNullOrWhiteSpace(email))
                return new JsonResult(new { success = false, message = "Email is required." });

            try
            {
                var response = await _httpClient
                    .PostAsync("Vigilance/ResendOtp", false, new { Email = email })
                    .ConfigureAwait(false);

                if (string.IsNullOrEmpty(response))
                    return new JsonResult(new { success = false, message = "Unable to resend OTP. Please try again." });

                dynamic result;
                try
                {
                    result = JsonConvert.DeserializeObject<dynamic>(response);
                }
                catch
                {
                    return new JsonResult(new { success = false, message = "Invalid response from server." });
                }

                if (result != null && result.message != null
                    && result.message.ToString() == "ALREADY_SUBMITTED")
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "ALREADY_SUBMITTED"
                    });
                }

                if (result == null || result.success == false)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = (string)(result?.message ?? "Resend failed.")
                    });
                }

                string otpCode = null;
                try
                {
                    if (result.otp != null)
                        otpCode = (string)result.otp;
                }
                catch { }

                if (string.IsNullOrWhiteSpace(otpCode))
                {
                    LogEmailFailure(email, "OTP Resend",
                        new Exception("WebAPI did not return OTP in response."));
                    return new JsonResult(new
                    {
                        success = false,
                        message = "OTP regeneration failed. Please try again."
                    });
                }

                var subject = "MDL Vigilance - Email Verification OTP";
                var body = BuildOtpEmailBody(otpCode);
                var emailSent = SendEmailUnified(email, subject, body);

                if (!emailSent)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "OTP regenerated but email could not be sent. Please try again."
                    });
                }

                return new JsonResult(new
                {
                    success = true,
                    message = "A new OTP has been sent to your email address."
                });
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

            if (string.IsNullOrEmpty(verifiedEmail) && vigi != null)
            {
                var postedEmail = vigi.Email?.Trim();
                if (!string.IsNullOrEmpty(postedEmail))
                {
                    var rec = VigilanceOtpStore.Get(postedEmail);
                    if (rec != null && rec.IsVerified
                        && !VigilanceOtpStore.IsVerificationExpired(rec))
                    {
                        verifiedEmail = postedEmail;
                        HttpContext.Session.SetString(SESSION_VERIFIED_EMAIL, postedEmail);
                    }
                }
            }

            VerifiedEmail = verifiedEmail;

            if (vigi == null) vigi = new VigilanceFormDTO();

            if (!string.IsNullOrEmpty(verifiedEmail))
                vigi.Email = verifiedEmail;

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

            var apiResult = !string.IsNullOrEmpty(response)
                ? JsonConvert.DeserializeObject<VigilanceFormDTO>(response)
                : null;

            if (apiResult == null)
            {
                _notyf.Error("Submission failed. Please check your details and try again.");
                return Page();
            }

            vigi = apiResult;

            // ═══════════════════════════════════════════════════════
            //  BUILD CVO NOTIFICATION EMAIL BODY — EXACT as before
            // ═══════════════════════════════════════════════════════
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
            strbody += "<td align='left' valign='top' style='border:solid 1px navy'><font color='#00008A' size='2' face='Verdana'><b>" + (string.IsNullOrEmpty(vigi.Fax) ? "NA" : vigi.Fax) + "</b></font></td></tr>";
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

            // ═══════════════════════════════════════════════════════
            //  SEND CVO NOTIFICATION
            //  Production: cvo@mazdock.com (hardcoded — same as old code)
            //  Testing: TEST_RECIPIENT
            // ═══════════════════════════════════════════════════════
            string cvoRecipient;

            if (USE_SMTP_HOST)
            {
                // PRODUCTION — CVO dedicated email
                cvoRecipient = PRODUCTION_CVO_RECIPIENT;   // "cvo@mazdock.com"
            }
            else
            {
                // TESTING
                cvoRecipient = TEST_RECIPIENT;
            }

            SendEmailUnified(cvoRecipient, CVO_EMAIL_SUBJECT, strbody);

            HttpContext.Session.Remove(SESSION_VERIFIED_EMAIL);
            VerifiedEmail = null;
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