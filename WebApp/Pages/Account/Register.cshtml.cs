using Application.Dtos;
using Application.Extensions;
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
using System.Security.Claims;
using System.Threading.Tasks;


namespace WebApp.Pages.Account
{
    [Authorize(Roles = "SuperAdmin")]
    public class RegisterModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _config;
        private readonly INotyfService _notyf;
        public RegisterModel(
            IHttpClientService httpClient,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration config,
            INotyfService notyf)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _config = config;
            _notyf = notyf;
        }
        [BindProperty] public RegisterDTO Input { get; set; }
        public string ReturnUrl { get; set; }
        public void OnGet(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ReturnUrl = returnUrl;
        }
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
            if (string.IsNullOrEmpty(Input.ProfileImage))
                Input.ProfileImage = _config["DefaultUserImage"];
            //// Validate Captcha Code
            //if (!Captcha.ValidateCaptchaCode(Input.CaptchaCode, HttpContext))
            //    ModelState.AddModelError("Captcha", "Please enter correct captcha");
            if (!ModelState.IsValid) { _notyf.Error(ModelState.GetErrorMessageString()); return Page(); }
            var result = await _httpClient.PostAsync("Auth/Register", false, Input).ConfigureAwait(false);
            if (string.IsNullOrEmpty(result)) return Page();
            var tokenVm = JsonConvert.DeserializeObject<TokenVM>(result);
            if (tokenVm == null)
            {
                _notyf.Error("Invalid login attempt");
                return Page();
            }
            //Set access token cookie
            SetTokenCookies(tokenVm);
            var tokenHandler = new JwtSecurityTokenHandler();
            if (!(tokenHandler.ReadToken(tokenVm.AccessToken) is JwtSecurityToken payload))
            {
                _notyf.Error("Invalid token.");
                return RedirectToPage("/Index");
            }
            await UserSignInAsync(payload).ConfigureAwait(false);
            return LocalRedirect(ReturnUrl);
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
            var authProperties = new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(Convert.ToInt32(_config["CookieExpiry"]))
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties).ConfigureAwait(false);
        }
        public async Task<IActionResult> OnGetCheckUsername(string uname)
        {
            //get all the cities by state for dropdown
            var unameResult = await _httpClient.GetAsync("Auth/CheckUsername", false, uname).ConfigureAwait(false);
            var userExists = !string.IsNullOrEmpty(unameResult) && Convert.ToBoolean(unameResult);
            return new JsonResult(userExists);
        }
    }
}
