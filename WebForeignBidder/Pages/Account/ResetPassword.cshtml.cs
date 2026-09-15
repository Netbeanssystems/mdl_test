using Application.Dtos;
using Application.Extensions;
using Application.Helpers;
using Application.ServiceInterfaces;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace WebForeignBidder.Pages.Account
{
    [AllowAnonymous]
    public class ResetPasswordModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly IRandomService _randomService;
        private readonly INotyfService _notyf;
        public ResetPasswordModel(
            IHttpClientService httpClient,
            IRandomService randomService,
            INotyfService notyf)
        {
            _httpClient = httpClient;
            _randomService = randomService;
            _notyf = notyf;
        }
        [BindProperty] public ResetPasswordDTO Input { get; set; }
        public async Task<IActionResult> OnGetAsync(string Code, string Id)
        {
            Input = new ResetPasswordDTO { Code = Code, UserId = Id };
            HttpContext.Session.SetString("Glass", await _randomService.RandomPassword().ConfigureAwait(false));
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            // Validate Captcha Code
            if (!Captcha.ValidateCaptchaCode(Input.CaptchaCode, HttpContext))
                ModelState.AddModelError("Captcha", "Please enter correct captcha");

            if (!ModelState.IsValid)
            {
                _notyf.Error($"{ModelState.GetErrorMessageString()}");
                return Page();
            }
            var emptyGlass = HttpContext.Session.GetString("Glass");
            var filledGlass = Input.EncPassword.Substring(0, 8);
            if (emptyGlass != filledGlass)
            {
                _notyf.Error("Invalid reset attempt.");
                HttpContext.Session.Remove("Glass");
                return Page();
            }
            var result = await _httpClient.PostAsync("Auth/ResetPassword", false, Input).ConfigureAwait(false);
            if (string.IsNullOrEmpty(result))
            {
                HttpContext.Session.Remove("Glass");
                return Page();
            }
            _notyf.Information($"{result}");
            return LocalRedirect("/bidder");
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