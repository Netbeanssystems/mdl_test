using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
namespace WebBank.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _config;
        public LogoutModel(
            IHttpContextAccessor httpContextAccessor,
            IConfiguration config)
        {
            _httpContextAccessor = httpContextAccessor;
            _config = config;
        }
        public async Task<IActionResult> OnGet()
        {
            //sign out of cookie authentication
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);
            //Clear the cookies
            ClearSessionCookies();
            //redirect to home page
            return RedirectToPage("/Index");
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
    }
}