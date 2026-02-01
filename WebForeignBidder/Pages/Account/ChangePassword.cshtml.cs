using Application.Dtos;
using Application.Extensions;
using Application.Helpers;
using Application.ServiceInterfaces;
using Application.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace WebForeignBidder.Pages.Account
{
    [Authorize]
    public class ChangePasswordModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly INotyfService _notyf;
        private readonly IRandomService _randomService;

        public ChangePasswordModel(IHttpClientService httpClient, INotyfService notyf, IRandomService randomService)
        {
            _httpClient = httpClient;
            _notyf = notyf;
            _randomService = randomService;
        }

        public string ReturnUrl { get; set; }
        [BindProperty] public ChangePasswordDTO Input { get; set; }
        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ReturnUrl = returnUrl;
            HttpContext.Session.SetString("Glass", await _randomService.RandomPassword().ConfigureAwait(false));
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
            if (!ModelState.IsValid) { _notyf.Error(ModelState.GetErrorMessageString()); return Page(); }
            var result = await _httpClient.PostAsync("Auth/ChangePassword", true, Input).ConfigureAwait(false);
            _notyf.Information($"{result}");
            if (string.IsNullOrEmpty(result)) return Page();

            if (JsonConvert.DeserializeObject<string>(result) == "Password can't be same as previous one")
            {
                _notyf.Error($"{result}");
                return RedirectToPage("ChangePassword");
            }

            if (result == "unauthorized") return RedirectToPage("/Account/Login");
            _notyf.Information($"{result}");
            HttpContext.Session.SetString("chn", "N");
            return LocalRedirect(ReturnUrl);
        }

        public async Task<IActionResult> OnGetOldPassword(string uid, string encpwd)
        {
            var uidplain = EnDeCryptor.DecryptStringAES(uid);
            var pwd = EnDeCryptor.DecryptStringAES(encpwd);

            var PointResult = await _httpClient.GetAsync("PasswordHistory/GetByUsername", true, uid);
            if (PointResult == "unauthorized") return null;

            var Point = !string.IsNullOrEmpty(PointResult) ? JsonConvert.DeserializeObject<PasswordHistoryVM>(PointResult) : null;

            if (Point.Pwd1 == pwd || Point.Pwd2 == pwd || Point.Pwd3 == pwd)
            {
                return new JsonResult(true);
            }
            else
            {
                return new JsonResult(false);
            }

        }
    }
}
