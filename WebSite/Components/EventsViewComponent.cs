using Application.ServiceInterfaces;
using Application.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebSite.Components
{
    public class EventsViewComponent : ViewComponent
    {
        private readonly IHttpClientServiceSite _httpClient;
        public EventsViewComponent(IHttpClientServiceSite httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var EventResult = await _httpClient.GetAsync("PhotoGallery/GetTopEvents", false);
            var Events = !string.IsNullOrEmpty(EventResult) ? JsonConvert.DeserializeObject<List<TopEventsVM>>(EventResult) : null;
            return View(Events);
            //return View();
        }
    }
}
