using Application.Dtos;
using Application.ServiceInterfaces;
using Application.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace WebBank.Pages.Admin.RoleMenuMapping
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    [IgnoreAntiforgeryToken]
    public class IndexModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly INotyfService _notyf;
        public IndexModel(
            IHttpClientService httpClient,
            INotyfService notyf)
        {
            _httpClient = httpClient;
            _notyf = notyf;
        }
        [BindProperty] public RoleMenusVM RoleMenuVm { get; set; }
        public async Task<IActionResult> OnGet()
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
            ViewData["Roles"] = new SelectList(roles, "Name", "Name");
            //get all menus for dropdown
            var menusResult = await _httpClient.GetAsync("Menus/GetWithAll", true).ConfigureAwait(false);
            if (menusResult == "unauthorized") return RedirectToPage("/Account/Login");
            var menus = !string.IsNullOrEmpty(menusResult) ? JsonConvert.DeserializeObject<List<MenuDTO>>(menusResult) : null;
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