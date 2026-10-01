using Application.ServiceInterfaces;
using Application.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace WebBank.Pages.Notifications
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
        public List<NotificationsVM> Notifications { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var getResponse = await _httpClient.GetAsync("Notifications/Get", true).ConfigureAwait(false);
            if (getResponse == "unauthorized")
            {
                _notyf.Information("Please login");
                return RedirectToPage("/Account/Login");
            }
            Notifications = !string.IsNullOrEmpty(getResponse) ? JsonConvert.DeserializeObject<List<NotificationsVM>>(getResponse) : null;
            return Page();
        }
    }
}