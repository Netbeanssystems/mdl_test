using Application.Dtos;
using Application.Extensions;
using Application.Helpers;
using Application.ServiceInterfaces;
using Application.Services;
using Application.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace WebBank.Pages.Account
{
   
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private const int OTP_MAX_REQUEST = 3;     // max OTP requests allowed
        private const int OTP_BLOCK_MINUTES = 10;  // block duration

        private readonly IHttpClientService _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _config;
        private readonly IRandomService _randomService;
        private readonly IEmailService _emailService;
        private readonly INotyfService _notyf;
        public LoginModel(
            IHttpClientService httpClient,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration config,
            IEmailService emailService,
            IRandomService randomService,
            INotyfService notyf)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _config = config;
            _emailService = emailService;
            _randomService = randomService;
            _notyf = notyf;
        }
        [BindProperty] public LoginDTO Input { get; set; }
        [BindProperty] public string OtpCode { get; set; }
        public string ReturnUrl { get; set; }
        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {

            returnUrl ??= Url.Content("~/");
            ReturnUrl = returnUrl;
            if (User.Identity?.IsAuthenticated == true) return LocalRedirect(Url.Content(ReturnUrl));
            HttpContext.Session.SetString("Glass", await _randomService.RandomPassword().ConfigureAwait(false));
            return Page();
        }

        public async Task<IActionResult> OnPostValidateUserAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");

            if (!Captcha.ValidateCaptchaCode(Input.CaptchaCode, HttpContext))
                return new JsonResult(new { success = false, message = "Please enter correct captcha." });

            if (!ModelState.IsValid)
                return new JsonResult(new { success = false, message = ModelState.GetErrorMessageString() });

            var emptyGlass = HttpContext.Session.GetString("Glass");
            var filledGlass = Input.EncPassword.Substring(0, 8);

            if (emptyGlass != filledGlass)
            {
                HttpContext.Session.Remove("Glass");
                return new JsonResult(new { success = false, message = "Invalid login attempt." });
            }

            Input.EncPassword = Input.EncPassword.Substring(8);
            Input.EncUsername = Input.EncUsername.Substring(8);

            var result = await _httpClient.PostAsync("Auth/Login", false, Input).ConfigureAwait(false);

            if (string.IsNullOrEmpty(result))
                return new JsonResult(new { success = false, message = "Login failed." });

            var tokenVm = JsonConvert.DeserializeObject<TokenVM>(result);
            if (tokenVm == null)
                return new JsonResult(new { success = false, message = "Invalid credentials." });

            var tokenHandler = new JwtSecurityTokenHandler();
            if (!(tokenHandler.ReadToken(tokenVm.AccessToken) is JwtSecurityToken payload))
                return new JsonResult(new { success = false, message = "Invalid token." });

            // Save required info in session
            HttpContext.Session.SetString("TempToken", result); // Temporarily save token
            HttpContext.Session.SetString("OtpUsername", Input.EncUsername);


            //----------------------------------------------------------------

            // ===== OTP RATE LIMIT CHECK =====

            // Check if user is blocked
            var blockUntilStr = HttpContext.Session.GetString("OtpBlockUntil");

            if (!string.IsNullOrEmpty(blockUntilStr))
            {
                var blockUntil = DateTime.Parse(blockUntilStr);

                if (DateTime.UtcNow < blockUntil)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = $"Too many OTP requests. Try again after {blockUntil.ToLocalTime():hh:mm tt}"
                    });
                }
            }

            // Check request count
            int requestCount = HttpContext.Session.GetInt32("OtpRequestCount") ?? 0;

            if (requestCount >= OTP_MAX_REQUEST)
            {
                var blockUntil = DateTime.UtcNow.AddMinutes(OTP_BLOCK_MINUTES);

                HttpContext.Session.SetString("OtpBlockUntil", blockUntil.ToString());

                return new JsonResult(new
                {
                    success = false,
                    message = $"Too many OTP requests. You are blocked for {OTP_BLOCK_MINUTES} minutes."
                });
            }


            //----------------------------------------------------------------




            // Generate OTP and send to user
            var otp = new Random().Next(100000, 999999).ToString();
            HttpContext.Session.SetString("OTP", otp);


            // ===== Increase OTP request count =====
            int newCount = (HttpContext.Session.GetInt32("OtpRequestCount") ?? 0) + 1;
            HttpContext.Session.SetInt32("OtpRequestCount", newCount);




            // Send OTP via email/SMS
            var userEmail = payload.Claims.FirstOrDefault(c => c.Type == "eml")?.Value;

            var body = $"Please use this OTP <b>{otp}</b> to verify yourself for bank login.";

            if (_config["Environment"].ToString() == "Live")
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(_config["SMTPFrom"]);
                    mail.To.Add(new MailAddress(userEmail));
                    var bccAddresses = _config["SMTPBcc"].Split(';');
                    foreach (var bcc in bccAddresses)
                    {
                        mail.Bcc.Add(new MailAddress(bcc));
                    }
                    mail.IsBodyHtml = true;
                    mail.Subject = "Login OTP for Bank";
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
                    ToAddresses = new List<string> { userEmail },
                    BccAddresses = _config["SMTPBcc"].Split(';').ToList(),
                    Subject = "Login OTP for Bank",
                    Body = body
                };
                await _emailService.SendEmailAsync(EmailVm).ConfigureAwait(false);
            }
            _notyf.Success("Otp sent to your registered mail.");

            return new JsonResult(new { success = true });
        }
        public async Task<IActionResult> OnPostOtpVerificationAsync()
        {
            var sessionOtp = HttpContext.Session.GetString("OTP");
            var tokenData = HttpContext.Session.GetString("TempToken");

            if (OtpCode != sessionOtp)
            {
                _notyf.Warning("Invalid OTP");
                return Page(); // Or you can return JSON if it's an AJAX call
            }

            var tokenVm = JsonConvert.DeserializeObject<TokenVM>(tokenData);
            var tokenHandler = new JwtSecurityTokenHandler();
            var payload = (JwtSecurityToken)tokenHandler.ReadToken(tokenVm.AccessToken);

            // Final login
            SetTokenCookies(tokenVm);
            await UserSignInAsync(payload);

            var profileImage = payload.Claims.FirstOrDefault(c => c.Type == "img")?.Value;
            HttpContext.Session.SetString("ProfileImage", profileImage ?? _config["DefaultUserImage"]);

            var chn = payload.Claims.FirstOrDefault(c => c.Type == "chn")?.Value;
            if (chn == "Y")
                HttpContext.Session.SetString("chn", "Y");

            // Clear temp data
            HttpContext.Session.Remove("OTP");
            HttpContext.Session.Remove("TempToken");
            // Clear OTP rate limit data
            HttpContext.Session.Remove("OtpRequestCount");
            HttpContext.Session.Remove("OtpBlockUntil");


            return LocalRedirect(ReturnUrl ?? "~/");
        }
        private void SetTokenCookies(TokenVM tokenVm)
        {
            var cookieOptions = new CookieOptions
            {
                Domain = _config["Domain"],
                Path = _config["CookiePath"],
                Expires = DateTimeOffset.UtcNow.AddMinutes(Convert.ToInt32(_config["CookieExpiry"])),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                IsEssential = true
            };
            _httpContextAccessor.HttpContext?.Response.Cookies.Append(_config["AuthToken"], tokenVm.AccessToken, cookieOptions);
            var cookieOptions2 = new CookieOptions
            {
                Domain = _config["Domain"],
                Path = _config["CookiePath"],
                Expires = DateTimeOffset.UtcNow.AddMinutes(Convert.ToInt32(_config["CookieExpiry2"])),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                IsEssential = true
            };
            _httpContextAccessor.HttpContext?.Response.Cookies.Append(_config["RefreshToken"], tokenVm.RefreshToken, cookieOptions2);
        }
        private async Task UserSignInAsync(JwtSecurityToken payload)
        {
            var claimsIdentity = new ClaimsIdentity(payload.Claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.Now.AddMinutes(Convert.ToInt32(_config["CookieExpiry"]))
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties).ConfigureAwait(false);
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
