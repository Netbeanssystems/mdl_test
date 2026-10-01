using Application.Dtos;
using Application.ServiceInterfaces;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace WebForeignBidder.Pages.Admin.Roles
{
    [Authorize(Roles = "SuperAdmin,Admin,BidderSuperAdmin")]
    [IgnoreAntiforgeryToken]
    public class PermissionsModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly INotyfService _notyf;
        private static List<string> RolesNameBidder;
        private readonly IConfiguration _config;
        public PermissionsModel(
            IHttpClientService httpClient,
            INotyfService notyf,
            IConfiguration config)
        {
            _httpClient = httpClient;
            _notyf = notyf;
            _config = config;
            RolesNameBidder = _config.GetSection("RolesNameBidder").GetChildren().Select(x => x.Value).ToList();
        }
        public List<RoleDTO> Roles { get; set; }
        public async Task<IActionResult> OnGet()
        {
            //get all roles with permissions
            var RolesResult = await _httpClient.GetAsync("Roles/Get", true).ConfigureAwait(false);
            if (RolesResult == "unauthorized")
            {
                _notyf.Information("Please login");
                return RedirectToPage("/Account/Login");
            }
            Roles = !string.IsNullOrEmpty(RolesResult) ? JsonConvert.DeserializeObject<List<RoleDTO>>(RolesResult) : null;
            Roles = Roles.Where(x => RolesNameBidder.Any(n => !string.IsNullOrEmpty(n) && x.Name.Contains(n, StringComparison.OrdinalIgnoreCase))).ToList();
            if (Roles == null || Roles.Count <= 0)
                _notyf.Error("Record not found");
            return Page();
        }
        public async Task<JsonResult> OnPostUpdateRolePermissions(string roles)
        {
            var model = JsonConvert.DeserializeObject<List<RoleDTO>>(roles);
            var result = await _httpClient.PostAsync("Roles/UpdatePermissions", true, model).ConfigureAwait(false);
            if (result == "unauthorized") return new JsonResult("unauthorized");
            var rowsChanged = !string.IsNullOrEmpty(result) ? Convert.ToInt32(result) : 0;
            return rowsChanged > 0 ? new JsonResult("Saved successfully") : new JsonResult("Save failed");
        }
    }
}