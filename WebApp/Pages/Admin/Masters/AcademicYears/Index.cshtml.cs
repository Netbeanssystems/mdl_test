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

namespace WebApp.Pages.Admin.Masters.AcademicYears
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
        public List<AcademicYearsVM> ModelVms { get; set; }
        [BindProperty] public AcademicYearsDTO ModelDto { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            var modelResponse = await _httpClient.GetAsync("AcademicYears/Get", true).ConfigureAwait(false);
            if (modelResponse == "unauthorized")
            {
                _notyf.Information("Please login");
                return RedirectToPage("/Account/Login");
            }
            ModelVms = !string.IsNullOrEmpty(modelResponse) ? JsonConvert.DeserializeObject<List<AcademicYearsVM>>(modelResponse) : null;
            if (ModelVms == null || ModelVms.Count <= 0)
                _notyf.Error("Academic Years not found");
            return Page();
        }
        public async Task<JsonResult> OnGetModel(int id)
        {
            var response = await _httpClient.GetAsync("AcademicYears/Get", true, id).ConfigureAwait(false);
            ModelDto = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<AcademicYearsDTO>(response) : null;
            return new JsonResult(ModelDto);
        }
        public async Task<IActionResult> OnPostManage()
        {
            ModelDto = ModelAuditor<AcademicYearsDTO>.SetAudit(User.Identity?.Name, ModelDto.Id == 0 ? "Create" : "Edit", HttpContext.Connection.RemoteIpAddress?.ToString(), ModelDto);
            if (!ModelState.IsValid)
            {
                _notyf.Error($"{ModelState.GetErrorMessageString()}");
                return RedirectToPage("Index");
            }
            var response = ModelDto.Id == 0
            ? await _httpClient.PostAsync("AcademicYears/Create", true, ModelDto).ConfigureAwait(false)
            : await _httpClient.PutAsync("AcademicYears/Edit", true, ModelDto.Id, ModelDto)
            .ConfigureAwait(false);
            if (response == "unauthorized")
            {
                _notyf.Information("Please login");
                return RedirectToPage("/Account/Login");
            }
            ModelDto = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<AcademicYearsDTO>(response) : null;
            if (ModelDto == null)
                _notyf.Error("Save failed");
            else
                _notyf.Success("Saved successfully");
            return RedirectToPage("Index");
        }
        public async Task<IActionResult> OnGetDelete(int id)
        {
            var DeleteResult = await _httpClient.DeleteAsync("AcademicYears/Delete", true, id).ConfigureAwait(false);
            if (DeleteResult == "unauthorized") return new JsonResult("unauthorized");
            var RowsChanged = !string.IsNullOrEmpty(DeleteResult) && Convert.ToInt32(DeleteResult) > 0;
            return RowsChanged ? new JsonResult("success") : new JsonResult("fail");
        }
    }
}