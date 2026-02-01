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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebBank.Pages.Admin.Menus
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class TreeModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly INotyfService _notyf;
        public TreeModel(
            IHttpClientService httpClient,
            INotyfService notyf)
        {
            _httpClient = httpClient;
            _notyf = notyf;
        }
        public List<MenusVM> ModelVms { get; set; }
        [BindProperty] public MenuDTO ModelDto { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var MenusResult = await _httpClient.GetAsync("Menus/GetWithAll", true).ConfigureAwait(false);
            if (MenusResult == "unauthorized")
            {
                _notyf.Information("Please login");
                return RedirectToPage("/Account/Login");
            }
            ModelVms = !string.IsNullOrEmpty(MenusResult) ? JsonConvert.DeserializeObject<List<MenusVM>>(MenusResult) : null;
            if (ModelVms == null || ModelVms.Count <= 0)
                _notyf.Error("Menus not found");
            return Page();
        }
        public async Task<IActionResult> OnGetModel(int id)
        {
            var MenuResult = await _httpClient.GetAsync("Menus/Get", true, id).ConfigureAwait(false);
            ModelDto = !string.IsNullOrEmpty(MenuResult) ? JsonConvert.DeserializeObject<MenuDTO>(MenuResult) : null;
            return new JsonResult(ModelDto);
        }
        public async Task<IActionResult> OnPostManage()
        {
            ModelDto = ModelAuditor<MenuDTO>.SetAudit(User.Identity?.Name, ModelDto.Id == 0 ? "Create" : "Edit", HttpContext.Connection.RemoteIpAddress?.ToString(), ModelDto);
            if (!ModelState.IsValid) { _notyf.Error(ModelState.GetErrorMessageString()); return RedirectToPage("Tree"); }
            var response = ModelDto.Id == 0
                ? await _httpClient.PostAsync("Menus/Create", true, ModelDto).ConfigureAwait(false)
                : await _httpClient.PutAsync("Menus/Edit", true, ModelDto.Id, ModelDto)
                    .ConfigureAwait(false);
            if (response == "unauthorized")
            {
                _notyf.Information("Please login");
                return RedirectToPage("/Account/Login");
            }
            ModelDto = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<MenuDTO>(response) : null;
            if (ModelDto == null)
                _notyf.Error("Save failed");
            else
                _notyf.Success("Saved successfully");
            return RedirectToPage("Tree");
        }
        public async Task<IActionResult> OnPostDelete(int id)
        {
            var DeleteResult = await _httpClient.DeleteAsync("Menus/Delete", true, id).ConfigureAwait(false);
            if (DeleteResult == "unauthorized") return new JsonResult("unauthorized");
            var RowsChanged = !string.IsNullOrEmpty(DeleteResult) && Convert.ToInt32(DeleteResult) > 0;
            return RowsChanged ? new JsonResult("success") : new JsonResult("fail");
        }
    }
}