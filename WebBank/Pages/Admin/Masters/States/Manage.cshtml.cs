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
using System.Threading.Tasks;
using WebBank.Extensions;
using WebBank.Helpers;

namespace WebBank.Pages.Admin.Masters.States
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
        [FromRoute] public int? id { get; set; }
        [BindProperty] public StatesDTO State { get; set; }
        public bool IsNew => State == null;
        public async Task<IActionResult> OnGet()
        {
            //get all the countries for dropdown
            var countries = await DataHelper.GetDropdown(_httpClient, "Countries", false).ConfigureAwait(false);
            if (countries == null || countries.Count <= 0)
            {
                _notyf.Error("Countries not found");
                return Page();
            }
            ViewData["Countries"] = new SelectList(countries, "Id", "Text");
            if (!id.HasValue) return Page();
            var modelResponse = await _httpClient.GetAsync("States/Get", true, (int)id).ConfigureAwait(false);
            if (modelResponse == "unauthorized")
            {
                _notyf.Information("Please login");
                return RedirectToPage("/Account/Login");
            }
            State = !string.IsNullOrEmpty(modelResponse) ? JsonConvert.DeserializeObject<StatesDTO>(modelResponse) : null;
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            var response = string.Empty;
            var filteredmodel = new StatesDTO();
            filteredmodel.Id = State.Id;
            filteredmodel.CountryId = State.CountryId;
            filteredmodel.StateName = objbal.OnlyValid(State.StateName);
            filteredmodel.StateCode = objbal.OnlyValid(State.StateCode);

            if (id == null || IsNew)
            {
                State = ModelAuditor<StatesDTO>.SetAudit(User.Identity.Name, "Create", HttpContext.Connection.RemoteIpAddress.ToString(), filteredmodel);
                if (!ModelState.IsValid) { _notyf.Error(ModelState.GetErrorMessageString()); return Page(); }
                response = await _httpClient.PostAsync("States/Create", true, filteredmodel).ConfigureAwait(false);
            }
            else
            {
                State = ModelAuditor<StatesDTO>.SetAudit(User.Identity.Name, "Edit", HttpContext.Connection.RemoteIpAddress.ToString(), filteredmodel);
                if (!ModelState.IsValid) { _notyf.Error(ModelState.GetErrorMessageString()); return Page(); }
                response = await _httpClient.PutAsync("States/Edit", true, filteredmodel.Id, filteredmodel).ConfigureAwait(false);
            }
            if (response == "unauthorized")
            {
                _notyf.Information("Please login");
                return RedirectToPage("/Account/Login");
            }
            State = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<StatesDTO>(response) : null;
            if (State == null)
                _notyf.Error("Save failed");
            else
                _notyf.Success("Saved successfully");
            return RedirectToPage("Index");
        }
        public async Task<IActionResult> OnPostDelete(int Id)
        {
            var DeleteResult = await _httpClient.DeleteAsync("States/Delete", true, Id).ConfigureAwait(false);
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