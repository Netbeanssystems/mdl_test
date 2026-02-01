using Application.ServiceInterfaces;
using Application.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Threading.Tasks;
namespace WebForeignBidder.Pages.Admin.Roles
{
    [Authorize(Roles = "SuperAdmin,Admin,BidderSuperAdmin")]
    public class MenusModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly INotyfService _notyf;
        public MenusModel(
            IHttpClientService httpClient,
            INotyfService notyf)
        {
            _httpClient = httpClient;
            _notyf = notyf;
        }
        [FromRoute] public string rolename { get; set; }
        [BindProperty] public RoleMenusVM RoleMenuVm { get; set; }
        public async Task<IActionResult> OnGet()
        {
            if (string.IsNullOrEmpty(rolename)) return Page();
            //get the menus by RoleName
            var result = await _httpClient.GetAsync("RoleMenus/GetAllByRole", true, rolename).ConfigureAwait(false);
            if (string.IsNullOrEmpty(result)) return Page();
            if (result == "unauthorized") return RedirectToPage("/Account/Login");
            var rmVm = JsonConvert.DeserializeObject<RoleMenusVM>(result);
            if (rmVm != null)
                RoleMenuVm = rmVm;
            return Page();
        }
        public async Task<IActionResult> OnPostAssignToRole()
        {
            var result = await _httpClient.PostAsync("RoleMenus/AssignToRole", true, RoleMenuVm).ConfigureAwait(false);
            if (string.IsNullOrEmpty(result)) return Page();
            if (result == "unauthorized") return RedirectToPage("/Account/Login");
            var urVm = JsonConvert.DeserializeObject<RoleMenusVM>(result);
            if (urVm == null)
                _notyf.Error("Menus could not be assigned to specified role.");
            RoleMenuVm = urVm;
            return Page();
        }
        public async Task<IActionResult> OnPostRemoveFromRole()
        {
            var result = await _httpClient.PostAsync("RoleMenus/RemoveFromRole", true, RoleMenuVm).ConfigureAwait(false);
            if (string.IsNullOrEmpty(result)) return Page();
            if (result == "unauthorized") return RedirectToPage("/Account/Login");
            var urVm = JsonConvert.DeserializeObject<RoleMenusVM>(result);
            if (urVm == null)
                _notyf.Error("Menus could not be removed from specified role.");
            RoleMenuVm = urVm;
            return Page();
        }
    }
}