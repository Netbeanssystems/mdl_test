using Application.Dtos;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;
namespace WebForeignBidder.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _config;
        private readonly IHttpClientService _httpClient;
         public LogoutModel(
            IHttpContextAccessor httpContextAccessor,
            IConfiguration config,
            IHttpClientService httpClient)
        {
            _httpContextAccessor = httpContextAccessor;
            _config = config;
            _httpClient = httpClient;
        }
        public async Task<IActionResult> OnGet()
        {
            try
            {
                // 1. Capture user info BEFORE signing out
                var userName = User.Identity?.Name;
                // Adjust the claim type to match how your IDs are stored (usually NameIdentifier)
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (!string.IsNullOrEmpty(userName))
                {
                    var logDto = new LoginLogsDTO
                    {
                        TimeStamp = DateTime.Now,
                        Action = "Logout",
                        UserName = userName,
                        UserId = userId ?? ""
                    };

                    // Use your HttpClient to hit the NEW controller's Create method
                    await _httpClient.PostAsync("LoginLogs/Create", true, logDto).ConfigureAwait(false);
                }

            }
            catch (Exception ex)
            {
                // Log locally but continue with logout so user isn't stuck
                Console.WriteLine(ex.Message);
            }

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