using Application.ServiceInterfaces;
using Application.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebSite.Pages.Events
{
    public class ViewEventModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        public ViewEventModel(IHttpClientService httpClient)
        {
            _httpClient = httpClient;
        }
        [FromRoute] public int? id { get; set; }
        [BindProperty] public List<EventPhotosVM> events { get; set; }
        public async Task<IActionResult> OnGet()
        {
            if (!id.HasValue) return RedirectToPage("/Events/EventCategory");
            var Result = await _httpClient.GetAsync("PhotoGallery/GetPhotos", false, (int)id);
            events = !string.IsNullOrEmpty(Result) ? JsonConvert.DeserializeObject<List<EventPhotosVM>>(Result) : null;

            if (events.Count > 0)
            {
                events = events.OrderByDescending(s => s.Added_on).ToList();
                HttpContext.Session.SetString("LastDate", events[0].Added_on.ToString());
            }
            return Page();
        }
    }
}
