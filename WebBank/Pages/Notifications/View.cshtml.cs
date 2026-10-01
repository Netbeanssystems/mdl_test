using Application.ServiceInterfaces;
using Application.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
namespace WebBank.Pages.Notifications
{
    [Authorize]
    public class ViewModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly INotyfService _notyf;
        public ViewModel(IHttpClientService httpClient,
            INotyfService notyf)
        {
            _httpClient = httpClient;
            _notyf = notyf;
        }
        [FromRoute] public int id { get; set; }
        public NotificationsVM Notifications { get; set; }
        public async Task<IActionResult> OnGet()
        {
            var response = await _httpClient.GetAsync("Notifications/GetVM", true, id).ConfigureAwait(false);
            if (response == "unauthorized")
            {
                _notyf.Information("Please login");
                return RedirectToPage("/Account/Login");
            }
            Notifications = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<NotificationsVM>(response) : null;
            if (Notifications != null) return Page();
            _notyf.Error("Not found");
            return RedirectToPage("Index");
        }
        public async Task<IActionResult> OnPostDelete(int Id)
        {
            var DeleteResult = await _httpClient.DeleteAsync("NotificationDetails/Delete", true, Id).ConfigureAwait(false);
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
            return RedirectToPage("View", new { id });
        }
    }
}
