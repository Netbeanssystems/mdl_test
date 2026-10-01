using Application.ServiceInterfaces;
using Application.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Threading.Tasks;
namespace WebApp.Pages.Admin.Users
{
    [Authorize(Roles = "SuperAdmin,BankAdmin,BidderSuperAdmin")]
    public class RolesModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly INotyfService _notyf;
        public RolesModel(
            IHttpClientService httpClient,
            INotyfService notyf)
        {
            _httpClient = httpClient;
            _notyf = notyf;
        }
        [FromRoute] public string id { get; set; }
        [BindProperty] public UserRolesVM UrVm { get; set; }
        public async Task<IActionResult> OnGet()
        {
            if (string.IsNullOrEmpty(id)) return Page();
            //get the user roles by UserId
            var result = await _httpClient.GetAsync("Users/UserRoles", true, id).ConfigureAwait(false);
            if (string.IsNullOrEmpty(result)) return Page();
            if (result == "unauthorized") return RedirectToPage("/Account/Login");
            var urVm = JsonConvert.DeserializeObject<UserRolesVM>(result);
            if (urVm != null)
                UrVm = urVm;
            return Page();
        }
        public async Task<IActionResult> OnPostAddToRole()
        {
            var result = await _httpClient.PostAsync("Users/AddToRole", true, UrVm).ConfigureAwait(false);
            if (string.IsNullOrEmpty(result)) return Page();
            if (result == "unauthorized") return RedirectToPage("/Account/Login");
            var urVm = JsonConvert.DeserializeObject<UserRolesVM>(result);
            if (urVm == null)
                _notyf.Error("User could not be added to specified role.");
            UrVm = urVm;
            return Page();
        }
        public async Task<IActionResult> OnPostRemoveFromRole()
        {
            var result = await _httpClient.PostAsync("Users/RemoveFromRole", true, UrVm).ConfigureAwait(false);
            if (string.IsNullOrEmpty(result)) return Page();
            if (result == "unauthorized") return RedirectToPage("/Account/Login");
            var urVm = JsonConvert.DeserializeObject<UserRolesVM>(result);
            if (urVm == null)
                _notyf.Error("User could not be removed from specified role.");
            UrVm = urVm;
            return Page();
        }
    }
}