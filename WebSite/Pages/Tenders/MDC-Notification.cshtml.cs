using Application.Dtos;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebSite.Pages.Tenders
{
    public class MDC_NotificationModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        public MDC_NotificationModel(IHttpClientService httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty] public NotificationDTO notif { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var modelResponse = await _httpClient.GetAsync("Tenders/GetMDCNotifications", false, 7, "current").ConfigureAwait(false);
            var ModelDto = !string.IsNullOrEmpty(modelResponse) ? JsonConvert.DeserializeObject<List<SBMPNotificationDTO>>(modelResponse) : null;

            var modeld = await _httpClient.GetAsync("Tenders/GetDocuments", false, 7, "current").ConfigureAwait(false);
            var docs = !string.IsNullOrEmpty(modeld) ? JsonConvert.DeserializeObject<List<DocumentModel>>(modeld) : null;

            NotificationDTO n = new NotificationDTO();

            n.not = ModelDto;
            n.docs = docs;
            notif = n;

            return Page();
        }

        public async Task<IActionResult> OnGetBindSelectdata(string status)
        {
            var modelResponse = await _httpClient.GetAsync("Tenders/GetMDCNotifications", false, 7, status).ConfigureAwait(false);
            var ModelDto = !string.IsNullOrEmpty(modelResponse) ? JsonConvert.DeserializeObject<List<SBMPNotificationDTO>>(modelResponse) : null;

            var modeld = await _httpClient.GetAsync("Tenders/GetDocuments", false, 7, status).ConfigureAwait(false);
            var docs = !string.IsNullOrEmpty(modeld) ? JsonConvert.DeserializeObject<List<DocumentModel>>(modeld) : null;

            NotificationDTO n = new NotificationDTO();

            n.not = ModelDto;
            n.docs = docs;
            notif = n;
            return new PartialViewResult
            {
                ViewName = "_TenderNotificationWithFeePartial",
                ViewData = new ViewDataDictionary<NotificationDTO>(ViewData, notif)
            };
        }
    }
}
