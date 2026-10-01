using Application.Dtos;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebSite.Pages.Tenders
{
    public class SBMP_NotificationModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        public SBMP_NotificationModel(IHttpClientService httpClient)
        {
            _httpClient = httpClient;
        }
        [BindProperty] public List<NotificationDTO> ModelDto { get; set; }
        [BindProperty] public NotificationDTO notif { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var modelResponse = await _httpClient.GetAsync("Tenders/GetSBMPNotifications", false, 1, "current").ConfigureAwait(false);
            var ModelDto = !string.IsNullOrEmpty(modelResponse) ? JsonConvert.DeserializeObject<List<SBMPNotificationDTO>>(modelResponse) : null;

            var modeld = await _httpClient.GetAsync("Tenders/GetDocuments", false, 1, "current").ConfigureAwait(false);
            var docs = !string.IsNullOrEmpty(modeld) ? JsonConvert.DeserializeObject<List<DocumentModel>>(modeld) : null;
            if (ModelDto != null && ModelDto.Count > 0)
            {
                ModelDto = ModelDto.OrderByDescending(s => s.Added_on).ToList();
                HttpContext.Session.SetString("UpdateLastDate", ModelDto[0].Added_on.ToString());
            }

            NotificationDTO n = new NotificationDTO();

            n.not = ModelDto;
            n.docs = docs;
            notif = n;

            return Page();
        }
        public async Task<IActionResult> OnGetBindSelectdata(string status)
        {
            var modelResponse = await _httpClient.GetAsync("Tenders/GetSBMPNotifications", false, 1, status).ConfigureAwait(false);
            var ModelDto = !string.IsNullOrEmpty(modelResponse) ? JsonConvert.DeserializeObject<List<SBMPNotificationDTO>>(modelResponse) : null;

            var modeld = await _httpClient.GetAsync("Tenders/GetDocuments", false, 1, status).ConfigureAwait(false);
            var docs = !string.IsNullOrEmpty(modeld) ? JsonConvert.DeserializeObject<List<DocumentModel>>(modeld) : null;
            if (ModelDto != null && ModelDto.Count > 0)
            {
                ModelDto = ModelDto.OrderByDescending(s => s.Added_on).ToList();
                HttpContext.Session.SetString("UpdateLastDate", ModelDto[0].Added_on.ToString());
            }

            NotificationDTO n = new NotificationDTO();

            n.not = ModelDto;
            n.docs = docs;
            notif = n;
            string lang = HttpContext.Session.GetString("Lang");
            if (string.IsNullOrEmpty(lang) || lang == "English")
            {
                return new PartialViewResult
                {
                    ViewName = "_TenderNotificationWithFeePartial",
                    ViewData = new ViewDataDictionary<NotificationDTO>(ViewData, notif)
                };
            }
            else
            {
                return new PartialViewResult
                {
                    ViewName = "_TenderNotificationWithFeePartialHindi",
                    ViewData = new ViewDataDictionary<NotificationDTO>(ViewData, notif)
                };
            }
        }
    }
}
