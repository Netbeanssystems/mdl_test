using Application.Dtos;
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
namespace WebBank.Pages.Logs
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class LoginLogsModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly INotyfService _notyf;
        public LoginLogsModel(
            IHttpClientService httpClient,
            INotyfService notyf)
        {
            _httpClient = httpClient;
            _notyf = notyf;
        }
        public List<LoginLogsVM> AppLogs { get; set; }
        [BindProperty] public DeletionDTO DeleteDto { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var modelResponse = await _httpClient.GetAsync("LoginLogs/Get", true).ConfigureAwait(false);
            if (modelResponse == "unauthorized")
            {
                _notyf.Information("Please login");
                return RedirectToPage("/Account/Login");
            }
            AppLogs = !string.IsNullOrEmpty(modelResponse) ? JsonConvert.DeserializeObject<List<LoginLogsVM>>(modelResponse) : null;
            return Page();
        }
        public async Task<IActionResult> OnPostDelete()
        {
            var DeleteResult = await _httpClient.DeleteAsync("LoginLogs/Delete", true, DeleteDto.Id).ConfigureAwait(false);
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
            return RedirectToPage();
        }
    }
}