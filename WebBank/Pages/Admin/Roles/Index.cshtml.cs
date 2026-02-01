using Application.Dtos;
using Application.Extensions;
using Application.Helpers;
using Application.ServiceInterfaces;
using Application.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace WebBank.Pages.Admin.Roles
{
    [Authorize(Roles = "SuperAdmin,Admin")]
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
        public List<RoleDTO> ModelVms { get; set; }
        [BindProperty] public RoleDTO ModelDto { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var RolesResult = await _httpClient.GetAsync("Roles/Get", true).ConfigureAwait(false);
            if (RolesResult == "unauthorized")
            {
                _notyf.Information("Please login");
                return RedirectToPage("/Account/Login");
            }
            ModelVms = !string.IsNullOrEmpty(RolesResult) ? JsonConvert.DeserializeObject<List<RoleDTO>>(RolesResult) : null;
            if (ModelVms == null || ModelVms.Count <= 0)
                _notyf.Error("Record not found");
            return Page();
        }
        public async Task<JsonResult> OnGetModel(string id)
        {
            var response = await _httpClient.GetAsync("Roles/Get", true, id).ConfigureAwait(false);
            ModelDto = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<RoleDTO>(response) : null;
            return new JsonResult(ModelDto);
        }
        public async Task<IActionResult> OnPostManage()
        {
            ModelDto = ModelAuditor<RoleDTO>.SetAudit(User.Identity.Name, string.IsNullOrEmpty(ModelDto.Id) ? "Create" : "Edit", HttpContext.Connection.RemoteIpAddress.ToString(), ModelDto);
            if (!ModelState.IsValid)
            {
                _notyf.Error($"{ModelState.GetErrorMessageString()}");
                return RedirectToPage("Index");
            }
            //Remove any space in Role Name
            ModelDto.Name = ModelDto.Name.RemoveSpace();

            var response = string.IsNullOrEmpty(ModelDto.Id)
            ? await _httpClient.PostAsync("Roles/Create", true, ModelDto).ConfigureAwait(false)
            : await _httpClient.PutAsync("Roles/Edit", true, ModelDto.Id, ModelDto)
            .ConfigureAwait(false);
            if (response == "unauthorized")
            {
                _notyf.Information("Please login");
                return RedirectToPage("/Account/Login");
            }
            ModelDto = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<RoleDTO>(response) : null;
            if (ModelDto == null)
                _notyf.Error("Save failed");
            else
                _notyf.Success("Saved successfully");
            return RedirectToPage("Index");
        }
        //public async Task<IActionResult> OnGetDelete(int id)
        //{
        //    var DeleteResult = await _httpClient.DeleteAsync("Roles/Delete", true, id).ConfigureAwait(false);
        //    if (DeleteResult == "unauthorized") return new JsonResult("unauthorized");
        //    var RowsChanged = !string.IsNullOrEmpty(DeleteResult) && Convert.ToInt32(DeleteResult) > 0;
        //    return RowsChanged ? new JsonResult("success") : new JsonResult("fail");
        //}
        public async Task<IActionResult> OnGetUsersByRole(string role)
        {
            var modelResponse = await _httpClient.GetAsync("Users/GetByRole", true, role).ConfigureAwait(false);
            var UserVms = !string.IsNullOrEmpty(modelResponse) && modelResponse != "unauthorized" ? JsonConvert.DeserializeObject<List<UserVM>>(modelResponse) : null;
            return new JsonResult(UserVms);
        }
    }
}