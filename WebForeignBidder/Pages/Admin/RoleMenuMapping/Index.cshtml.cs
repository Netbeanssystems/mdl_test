using Application.Dtos;
using Application.ServiceInterfaces;
using Application.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace WebForeignBidder.Pages.Admin.RoleMenuMapping
{
    [Authorize(Roles = "SuperAdmin,Admin,BidderSuperAdmin")]
    [IgnoreAntiforgeryToken]
    public class IndexModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly INotyfService _notyf;
        private static List<string> RolesNameBidder;
        private readonly IConfiguration _config;
        public IndexModel(
            IHttpClientService httpClient,
            INotyfService notyf,
            IConfiguration config)
        {
            _httpClient = httpClient;
            _notyf = notyf;
            _config = config;
            RolesNameBidder = _config.GetSection("RolesNameBidder").GetChildren().Select(x => x.Value).ToList();
        }
        [BindProperty] public RoleMenusVM RoleMenuVm { get; set; }
        public async Task<IActionResult> OnGet()
        {
            //get all the roles for dropdown
            var rolesResult = await _httpClient.GetAsync("Roles/Get", true).ConfigureAwait(false);
            if (rolesResult == "unauthorized") return RedirectToPage("/Account/Login");
            var roles = !string.IsNullOrEmpty(rolesResult) ? JsonConvert.DeserializeObject<List<RoleDTO>>(rolesResult) : null;
            roles = roles.Where(x => RolesNameBidder.Any(n => !string.IsNullOrEmpty(n) && x.Name.Contains(n, StringComparison.OrdinalIgnoreCase))).ToList();
            if (roles == null || roles.Count <= 0)
            {
                _notyf.Error("Roles not found");
                return Page();
            }
            ViewData["Roles"] = new SelectList(roles, "Name", "Name");
            //get all menus for dropdown
            var menusResult = await _httpClient.GetAsync("Menus/GetWithAll", true).ConfigureAwait(false);
            if (menusResult == "unauthorized") return RedirectToPage("/Account/Login");
            var menus = !string.IsNullOrEmpty(menusResult) ? JsonConvert.DeserializeObject<List<MenuDTO>>(menusResult) : null;
            menus = menus.Where(x => x.CreatedBy.Contains("BidderSuperAdmin") || x.CreatedBy.Contains("superadmin")).ToList();
            if (menus == null || menus.Count <= 0)
            {
                _notyf.Error("Menus not found");
                return Page();
            }
            ViewData["Menus"] = menus;
            return Page();
        }
        public async Task<JsonResult> OnGetMenuByRole(string rolename)
        {
            //get the menus by RoleName
            var result = await _httpClient.GetAsync("RoleMenus/GetAllByRole", true, rolename).ConfigureAwait(false);
            if (result == "unauthorized") return new JsonResult("unauthorized");
            var rmVm = !string.IsNullOrEmpty(result) ? JsonConvert.DeserializeObject<RoleMenusVM>(result) : null;
            return new JsonResult(rmVm?.assignedMenus);
        }
        public async Task<JsonResult> OnPostSaveRoleMenus(string roleMenus)
        {
            var model = JsonConvert.DeserializeObject<List<RoleMenusDTO>>(roleMenus);
            var result = await _httpClient.PostAsync("RoleMenus/UpdateMenus", true, model).ConfigureAwait(false);
            if (result == "unauthorized") return new JsonResult("unauthorized");
            var rowsChanged = !string.IsNullOrEmpty(result) ? Convert.ToInt32(result) : 0;
            return rowsChanged > 0 ? new JsonResult("Saved successfully") : new JsonResult("Save failed");
        }
    }
}