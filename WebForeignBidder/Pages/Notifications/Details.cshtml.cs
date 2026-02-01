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
namespace WebForeignBidder.Pages.Notifications
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly INotyfService _notyf;
        public DetailsModel(
            IHttpClientService httpClient,
            INotyfService notyf)
        {
            _httpClient = httpClient;
            _notyf = notyf;
        }
        [FromRoute] public int? nid { get; set; }
        public List<NotificationDetailsVM> NotificationDetails { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var getResponse = !nid.HasValue
                ? await _httpClient.GetAsync("NotificationDetails/Get", true).ConfigureAwait(false)
                : await _httpClient.GetAsync("NotificationDetails/GetByNotificationId", true, (int)nid, User.Identity.Name)
                    .ConfigureAwait(false);
            if (getResponse == "unauthorized")
            {
                _notyf.Information("Please login");
                return RedirectToPage("/Account/Login");
            }
            NotificationDetails = !string.IsNullOrEmpty(getResponse) ? JsonConvert.DeserializeObject<List<NotificationDetailsVM>>(getResponse) : null;
            return Page();
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
            return nid.HasValue ? RedirectToPage("Details", new { nid = (int)nid }) : RedirectToPage("Details");
        }
        public async Task<IActionResult> OnGetDeleteSelected(string modelStr)
        {
            var Result = await _httpClient.GetAsync("NotificationDetails/DeleteSelected", true, modelStr).ConfigureAwait(false);
            if (string.IsNullOrEmpty(Result)) return new JsonResult("Notification(s) couldn't be deleted");
            if (Result == "unauthorized") return new JsonResult(Result);
            return Convert.ToInt32(Result) > 0 ? new JsonResult("Notification(s) deleted successfully") : new JsonResult("Notification(s) couldn't be deleted");
        }
    }
}