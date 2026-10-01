using Application.Dtos;
using Application.Extensions;
using Application.Helpers;
using Application.ServiceInterfaces;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBank.Extensions;
using WebBank.Helpers;

namespace WebBank.Pages.Admin.Masters.Districts
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class ManageModel : PageModel
    {
        Validate objbal = new Validate();
        private readonly IHttpClientService _httpClient;
        private readonly INotyfService _notyf;
        public ManageModel(
            IHttpClientService httpClient,
            INotyfService notyf)
        {
            _httpClient = httpClient;
            _notyf = notyf;
        }
        public Dictionary<string, bool> permissions => JsonConvert.DeserializeObject<Dictionary<string, bool>>(User.Claims.First(x => x.Type == "per").Value);
        [FromRoute] public int? id { get; set; }
        [BindProperty] public DistrictsDTO District { get; set; }
        public bool IsNew => District == null;
        public async Task<IActionResult> OnGet()
        {
            //get all the states for dropdown
            var states = await DataHelper.GetChildrenDropdownByParentId(_httpClient, "States", "Country", 1, false).ConfigureAwait(false);
            if (states == null || states.Count <= 0)
            {
                _notyf.Error("States not found");
                return Page();
            }
            ViewData["States"] = new SelectList(states, "Id", "Text");
            if (!id.HasValue)
                return permissions["CanCreate"] ? Page() : RedirectToPage("/Errors/AccessDenied");
            if (!permissions["CanEdit"]) return RedirectToPage("/Errors/AccessDenied");
            var modelResponse = await _httpClient.GetAsync("Districts/Get", true, (int)id).ConfigureAwait(false);
            if (modelResponse == "unauthorized")
            {
                _notyf.Information("Please login");
                return RedirectToPage("/Account/Login");
            }
            District = !string.IsNullOrEmpty(modelResponse) ? JsonConvert.DeserializeObject<DistrictsDTO>(modelResponse) : null;
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            var response = string.Empty;

            var filteredmodel = new DistrictsDTO();
            filteredmodel.Id = District.Id;
            filteredmodel.StateId = District.StateId;
            filteredmodel.DistrictName = objbal.OnlyValid(District.DistrictName);
            filteredmodel.DistrictCode = objbal.OnlyValid(District.DistrictCode);

            if (id == null || IsNew)
            {
                District = ModelAuditor<DistrictsDTO>.SetAudit(User.Identity?.Name, "Create", HttpContext.Connection.RemoteIpAddress?.ToString(), filteredmodel);
                if (!ModelState.IsValid) { _notyf.Error(ModelState.GetErrorMessageString()); return Page(); }
                response = await _httpClient.PostAsync("Districts/Create", true, filteredmodel).ConfigureAwait(false);
            }
            else
            {
                District = ModelAuditor<DistrictsDTO>.SetAudit(User.Identity?.Name, "Edit", HttpContext.Connection.RemoteIpAddress?.ToString(), filteredmodel);
                if (!ModelState.IsValid) { _notyf.Error(ModelState.GetErrorMessageString()); return Page(); }
                response = await _httpClient.PutAsync("Districts/Edit", true, filteredmodel.Id, filteredmodel).ConfigureAwait(false);
            }
            if (response == "unauthorized")
            {
                _notyf.Information("Please login");
                return RedirectToPage("/Account/Login");
            }
            District = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<DistrictsDTO>(response) : null;
            if (District == null)
                _notyf.Error("Save failed");
            else
                _notyf.Success("Saved successfully");
            return RedirectToPage("Index");
        }
        public async Task<IActionResult> OnPostDelete(int Id)
        {
            var DeleteResult = await _httpClient.DeleteAsync("Districts/Delete", true, Id).ConfigureAwait(false);
            if (DeleteResult == "unauthorized")
            {
                _notyf.Information("Please login");
                return RedirectToPage("/Account/Login");
            }
            var RowsChanged = !string.IsNullOrEmpty(DeleteResult) && Convert.ToInt32(DeleteResult) > 0;
            if (RowsChanged)
                _notyf.Success("Deleted successfully");
            else
                _notyf.Error("Delete failed. There might be active child records.");
            return RedirectToPage("Index");
        }
    }
}