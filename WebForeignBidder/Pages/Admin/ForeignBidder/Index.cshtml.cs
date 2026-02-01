using Application.Dtos;
using Application.ServiceInterfaces;
using Application.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace WebForeignBidder.Pages.Admin.ForeignBidder
{
    [Authorize(Roles = "SuperAdmin,BidderSuperAdmin")]
    public class IndexModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly INotyfService _notyf;
        public IndexModel(
            IHttpClientService httpClient,
            IEmailService emailService,
            IConfiguration config,
            IHttpContextAccessor httpContextAccessor,
            INotyfService notyf)
        {
            _httpClient = httpClient;
            _emailService = emailService;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
            _notyf = notyf;
        }
        [BindProperty] public UserDTO user { get; set; }
        public List<UserVM> Users { get; set; }
        public async Task<IActionResult> OnGetAsync(string role)
        {
            //get all the roles for dropdown
            var rolesResult = await _httpClient.GetAsync("Roles/Get", true).ConfigureAwait(false);
            if (rolesResult == "unauthorized") return RedirectToPage("/Account/Login");
            var roles = !string.IsNullOrEmpty(rolesResult) ? JsonConvert.DeserializeObject<List<RoleDTO>>(rolesResult) : null;
            if (roles == null || roles.Count <= 0)
            {
                _notyf.Error("Roles not found");
                return Page();
            }
            roles.Remove(roles.Find(x => x.Name == "Anonymous"));
            if (string.IsNullOrEmpty(role)) role = "ForeignBidder"; //User.FindFirst(ClaimTypes.Role)?.Value;
            ViewData["Roles"] = new SelectList(roles, "Name", "Name");
            var UsersResult = string.IsNullOrEmpty(role) || role == "All" ?
                await _httpClient.GetAsync("Users/UsersForList", true).ConfigureAwait(false) :
                await _httpClient.GetAsync("Users/GetByRole", true, role).ConfigureAwait(false);
            if (UsersResult == "unauthorized")
            {
                _notyf.Information("Please login");
                return RedirectToPage("/Account/Login");
            }
            Users = !string.IsNullOrEmpty(UsersResult) ? JsonConvert.DeserializeObject<List<UserVM>>(UsersResult) : null;

            if (Request.Headers["x-requested-with"] != "XMLHttpRequest") return Page();
            return new PartialViewResult
            {
                ViewName = "_UsersListPartial",
                ViewData = new ViewDataDictionary<List<UserVM>>(ViewData, Users)
            };
            //if (Users == null || Users.Count <= 0)
            //    _notyf.Error("Users not found");
            //return Page();
        }
        public async Task<IActionResult> OnPostSendCreds()
        {
            //if (!ModelState.IsValid) { _notyf.Error(ModelState.GetErrorMessageString()); return Page(); }
            var response = await _httpClient.PostAsync("Auth/GetPasswordResetToken", true, user.Id).ConfigureAwait(false);
            var Code = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<string>(response) : null;
            if (string.IsNullOrEmpty(Code))
            {
                _notyf.Error("Password reset link could not be generated");
                return Page();
            }
            var callbackUrl = Url.Page(
                "/Account/ResetPassword",
                pageHandler: null,
                values: new { Code, user.Id },
                protocol: Request.Scheme);
            var EmailVm = new EmailVM
            {
                ToAddresses = new List<string> { user.Email },
                Subject = "Password Reset Link",
                Body = $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>."
            };
            await _emailService.SendEmailAsync(EmailVm).ConfigureAwait(false);
            _notyf.Success($"Reset password link sent to {user.Email}");
            return RedirectToPage("Index");
        }
        public async Task<IActionResult> OnGetPrivilegeLogin(string Id)
        {
            var result = await _httpClient.GetAsync("Auth/PrivilegeLogin", true, Id).ConfigureAwait(false);
            if (string.IsNullOrEmpty(result)) return RedirectToPage();
            var tokenVm = JsonConvert.DeserializeObject<TokenVM>(result);
            if (tokenVm == null)
            {
                _notyf.Error("Invalid login attempt.");
                return RedirectToPage();
            }
            //sign out of cookie authentication
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);
            //Clear the cookies
            ClearSessionCookies();
            //Set access token cookie
            SetTokenCookies(tokenVm);
            var tokenHandler = new JwtSecurityTokenHandler();
            if (!(tokenHandler.ReadToken(tokenVm.AccessToken) is JwtSecurityToken payload))
            {
                _notyf.Error("Invalid token.");
                return RedirectToPage();
            }
            await UserSignInAsync(payload).ConfigureAwait(false);
            //Set Profile Image to session on login
            var ProfileImage = !string.IsNullOrEmpty(payload.Claims.First(c => c.Type == "img").Value) ? payload.Claims.First(c => c.Type == "img").Value : _config["DefaultUserImage"];
            HttpContext.Session.SetString("ProfileImage", ProfileImage);
            //Check if login for first time with default password
            var chn = payload.Claims.First(c => c.Type == "chn").Value.ToString();
            if (chn != "Y") return LocalRedirect(Url.Content("~/"));
            HttpContext.Session.SetString("chn", "Y");
            return LocalRedirect(Url.Content("~/"));
        }
        private void ClearSessionCookies()
        {
            //delete the session cookie from client  (browser)
            _httpContextAccessor.HttpContext?.Response.Cookies.Delete(_config["Session"]);
            //remove the token cookie from response
            _httpContextAccessor.HttpContext?.Response.Cookies.Delete(_config["AuthToken"]);
            //remove the refresh token cookie from response
            _httpContextAccessor.HttpContext?.Response.Cookies.Delete(_config["RefreshToken"]);

            var options = new CookieOptions
            {
                Domain = _config["Domain"],
                Path = _config["CookiePath"],
                Expires = DateTime.Now.AddDays(-1),
                HttpOnly = true,
                Secure = true,
            };
            //delete the token cookie from client (browser)
            _httpContextAccessor.HttpContext?.Response.Cookies.Append(_config["Session"], "session", options);
            //delete the token cookie from client (browser)
            _httpContextAccessor.HttpContext?.Response.Cookies.Append(_config["AuthToken"], "token", options);
            //delete the refresh token cookie from client (browser)
            _httpContextAccessor.HttpContext?.Response.Cookies.Append(_config["RefreshToken"], "refreshtoken", options);

            //clear the session data
            _httpContextAccessor.HttpContext?.Session.Clear();
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
    }
}