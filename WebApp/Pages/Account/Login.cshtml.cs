using Application.Dtos;
using Application.Extensions;
using Application.Helpers;
using Application.ServiceInterfaces;
using Application.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _config;
        private readonly IRandomService _randomService;
        private readonly INotyfService _notyf;
        private readonly IEmailService _emailService;
        public LoginModel(
            IHttpClientService httpClient,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration config,
            IRandomService randomService,
            INotyfService notyf,
            IEmailService emailService)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _config = config;
            _randomService = randomService;
            _notyf = notyf;
            _emailService = emailService;
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

        //public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        //{
        //    ReturnUrl = returnUrl ?? Url.Content("~/");
        //    // Validate Captcha Code
        //    if (!Captcha.ValidateCaptchaCode(Input.CaptchaCode, HttpContext))
        //        ModelState.AddModelError("Captcha", "Please enter correct captcha");
        //    //Validate model
        //    if (!ModelState.IsValid)
        //    {
        //        _notyf.Error($"{ModelState.GetErrorMessageString()}");
        //        return Page();
        //    }
        //    Input.Username = "U$erName";
        //    //Input.EncUsername = Input.EncUsername.Substring(8);
        //    var emptyGlass = HttpContext.Session.GetString("Glass");
        //    var filledGlass = Input.EncPassword.Substring(0, 8);
        //    if (emptyGlass != filledGlass)
        //    {
        //        _notyf.Error("Invalid login attempt.");
        //        HttpContext.Session.Remove("Glass");
        //        return RedirectToPage();
        //    }
        //    Input.EncPassword = Input.EncPassword.Substring(8);
        //    Input.EncUsername = Input.EncUsername.Substring(8);

        //    //Input.EncUsername = "test";
        //    //Input.EncPassword = "test123";

        //    var result = await _httpClient.PostAsync("Auth/Login", false, Input).ConfigureAwait(false);
        //    if (string.IsNullOrEmpty(result))
        //    {
        //        HttpContext.Session.Remove("Glass");
        //        return RedirectToPage();
        //    }
        //    var tokenVm = JsonConvert.DeserializeObject<TokenVM>(result);
        //    if (tokenVm == null)
        //    {
        //        _notyf.Error("Invalid login attempt.");
        //        HttpContext.Session.Remove("Glass");
        //        return RedirectToPage();
        //    }
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    if (!(tokenHandler.ReadToken(tokenVm.AccessToken) is JwtSecurityToken payload))
        //    {
        //        _notyf.Error("Invalid token.");
        //        HttpContext.Session.Remove("Glass");
        //        return RedirectToPage();
        //    }
        //    HttpContext.Session.Remove("Glass");
        //    //Set access token cookie
        //    SetTokenCookies(tokenVm);
        //    await UserSignInAsync(payload).ConfigureAwait(false);
        //    //Set Profile Image to session on login
        //    var ProfileImage = !string.IsNullOrEmpty(payload.Claims.First(c => c.Type == "img").Value) ? payload.Claims.First(c => c.Type == "img").Value : _config["DefaultUserImage"];
        //    HttpContext.Session.SetString("ProfileImage", ProfileImage);
        //    //Check if login for first time with default password
        //    var chn = payload.Claims.First(c => c.Type == "chn").Value.ToString();
        //    if (chn != "Y") return LocalRedirect(ReturnUrl);
        //    HttpContext.Session.SetString("chn", "Y");
        //    return LocalRedirect(Url.Content("~/"));
        //}

        // Validate with Username + Password + Otp 
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            ReturnUrl ??= Url.Content("~/");

            if (!Captcha.ValidateCaptchaCode(Input.CaptchaCode, HttpContext))
                return new JsonResult(new { success = false, message = "Invalid captcha" });

            if (!ModelState.IsValid)
                return new JsonResult(new { success = false, message = "Invalid input" });

            var emptyGlass = HttpContext.Session.GetString("Glass");
            var filledGlass = Input.EncPassword.Substring(0, 8);

            if (emptyGlass != filledGlass)
                return new JsonResult(new { success = false, message = "Invalid login attempt" });

            Input.EncPassword = Input.EncPassword.Substring(8);
            Input.EncUsername = Input.EncUsername.Substring(8);

            var result = await _httpClient.PostAsync("Auth/Login", false, Input);

            if (string.IsNullOrEmpty(result))
                return new JsonResult(new { success = false, message = "Login failed" });

            var tokenVm = JsonConvert.DeserializeObject<TokenVM>(result);

            if (tokenVm == null)
                return new JsonResult(new { success = false, message = "Invalid login" });

            //  Save token temporarily
            HttpContext.Session.SetString("TempToken", result);

            //  Generate OTP
            var otp = new Random().Next(100000, 999999).ToString();
            HttpContext.Session.SetString("OTP", otp);

            //  Extract email from token
            var handler = new JwtSecurityTokenHandler();
            var payload = (JwtSecurityToken)handler.ReadToken(tokenVm.AccessToken);
            var email = payload.Claims.FirstOrDefault(x => x.Type == "eml")?.Value;

            //  Send OTP
            var body = $"Your Admin Login OTP is <b>{otp}</b>";

            await _emailService.SendEmailAsync(new EmailVM
            {
                ToAddresses = new() { email },
                Subject = "Admin Login OTP",
                Body = body
            });

            return new JsonResult(new { success = true });
        }

        // Otp Veryfication 
        public async Task<IActionResult> OnPostOtpVerificationAsync()
        {
            var sessionOtp = HttpContext.Session.GetString("OTP");
            var tokenData = HttpContext.Session.GetString("TempToken");

            if (OtpCode != sessionOtp)
            {
                _notyf.Error("Invalid OTP");
                return Page();
            }

            var tokenVm = JsonConvert.DeserializeObject<TokenVM>(tokenData);
            var handler = new JwtSecurityTokenHandler();
            var payload = (JwtSecurityToken)handler.ReadToken(tokenVm.AccessToken);

            // Final login
            SetTokenCookies(tokenVm);
            await UserSignInAsync(payload);

            // Cleanup
            HttpContext.Session.Remove("OTP");
            HttpContext.Session.Remove("TempToken");

            return LocalRedirect("~/");
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
