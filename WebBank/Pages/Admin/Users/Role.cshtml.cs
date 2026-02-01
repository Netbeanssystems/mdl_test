using Application.Dtos;
using Application.ServiceInterfaces;
using Application.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebBank.Helpers;

namespace WebBank.Pages.Admin.Users
{
    [Authorize(Roles = "SuperAdmin,BankAdmin,AuditAdmin")]
    [IgnoreAntiforgeryToken]
    public class RoleModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly INotyfService _notyf;
        public RoleModel(
            IHttpClientService httpClient,
            INotyfService notyf)
        {
            _httpClient = httpClient;
            _notyf = notyf;
        }
        public async Task<IActionResult> OnGet()
        {
            //get all the users for dropdown
            var users = await DataHelper.GetDropdownStr(_httpClient, "Users").ConfigureAwait(false);
            if (users == null || users.Count <= 0)
            {
                _notyf.Error("Users not found");
                return Page();
            }
            ViewData["Users"] = new SelectList(users, "Id", "Text");
            //get all the roles for dropdown
            var rolesResult = await _httpClient.GetAsync("Roles/Get", true).ConfigureAwait(false);
            if (rolesResult == "unauthorized") return RedirectToPage("/Account/Login");
            var roles = !string.IsNullOrEmpty(rolesResult) ? JsonConvert.DeserializeObject<List<RoleDTO>>(rolesResult) : null;
            if (roles == null || roles.Count <= 0)
            {
                _notyf.Error("Record not found");
                return Page();
            }
            ViewData["Roles"] = new SelectList(roles, "Name", "Name");
            return Page();
        }
        public async Task<JsonResult> OnGetUserRole(string uid)
        {
            //get the role by uid
            var result = await _httpClient.GetAsync("Users/GetRole", true, uid).ConfigureAwait(false);
            if (result == "unauthorized") return new JsonResult("unauthorized");
            var urVm = !string.IsNullOrEmpty(result) ? JsonConvert.DeserializeObject<UserRoleVM>(result) : null;
            return new JsonResult(urVm?.Role);
        }
        public async Task<JsonResult> OnPostSaveUserRole(string urVm)
        {
            var model = JsonConvert.DeserializeObject<UserRoleVM>(urVm);
            var result = await _httpClient.PostAsync("Users/UpdateUserRole", true, model).ConfigureAwait(false);
            if (result == "unauthorized") return new JsonResult("unauthorized");
            var success = !string.IsNullOrEmpty(result) && JsonConvert.DeserializeObject<bool>(result);
            return success ? new JsonResult("Saved successfully") : new JsonResult("Save failed");
        }
    }
}