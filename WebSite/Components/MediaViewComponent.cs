using Application.Dtos;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebSite.Components
{
    public class MediaViewComponent : ViewComponent
    {
        private readonly IHttpClientServiceSite _httpClient;
        public MediaViewComponent(IHttpClientServiceSite httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var MenusResult = await _httpClient.GetAsync("Media/Get", false);
            var Menus = !string.IsNullOrEmpty(MenusResult) ? JsonConvert.DeserializeObject<List<MediaDTO>>(MenusResult) : null;
            return View(Menus);
        }
    }
}
