using Application.Dtos;
using Application.Extensions;
using Application.Helpers;
using Application.ServiceInterfaces;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebBank.Pages.Admin.PhotoGallery.Events
{
    [Authorize(Roles = "SuperAdmin,Editors,Admin,ContentCreator,Moderator,Publisher")]
    public class EditNewModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly INotyfService _notyf;
        public EditNewModel(IHttpClientService httpClient, INotyfService notyf)
        {
            _httpClient = httpClient;
            _notyf = notyf;
        }

        public Dictionary<string, bool> permissions => JsonConvert.DeserializeObject<Dictionary<string, bool>>(User.Claims.First(x => x.Type == "per").Value);
        [FromRoute] public int? id { get; set; }
        [BindProperty] public EventDTO Event { get; set; }
        public bool IsNew => Event == null;
        public async Task<IActionResult> OnGet()
        {
            if (!id.HasValue)
                return permissions["CanCreate"] ? Page() : RedirectToPage("/Errors/AccessDenied");
            if (!permissions["CanEdit"]) return RedirectToPage("/Errors/AccessDenied");
            var modelResponse = await _httpClient.GetAsync("Events/Get", true, (int)id).ConfigureAwait(false);
            if (modelResponse == "unauthorized")
            {
                _notyf.Information("Please login");
                return RedirectToPage("/Account/Login");
            }
            Event = !string.IsNullOrEmpty(modelResponse) ? JsonConvert.DeserializeObject<EventDTO>(modelResponse) : null;
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            var response = string.Empty;
            if (id == null || IsNew)
            {
                Event = ModelAuditor<EventDTO>.SetAudit(User.Identity?.Name, "Create", HttpContext.Connection.RemoteIpAddress?.ToString(), Event);
                if (!ModelState.IsValid) { _notyf.Error(ModelState.GetErrorMessageString()); return Page(); }
                response = await _httpClient.PostAsync("Events/Create", true, Event).ConfigureAwait(false);
            }
            else
            {
                Event = ModelAuditor<EventDTO>.SetAudit(User.Identity?.Name, "Edit", HttpContext.Connection.RemoteIpAddress?.ToString(), Event);
                if (!ModelState.IsValid) { _notyf.Error(ModelState.GetErrorMessageString()); return Page(); }
                response = await _httpClient.PutAsync("Events/Edit", true, Event.Id, Event).ConfigureAwait(false);
            }
            if (response == "unauthorized")
            {
                _notyf.Information("Please login");
                return RedirectToPage("/Account/Login");
            }
            Event = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<EventDTO>(response) : null;
            if (Event == null)
                _notyf.Error("Save failed");
            else
                _notyf.Success("Saved successfully");
            return RedirectToPage("Index");
        }
        public async Task<IActionResult> OnPostDelete(int Id)
        {
            var DeleteResult = await _httpClient.DeleteAsync("Events/Delete", true, Id).ConfigureAwait(false);
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
