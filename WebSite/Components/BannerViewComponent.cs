using Application.Dtos;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebSite.Components
{
    public class BannerViewComponent : ViewComponent
    {
        private readonly IHttpClientServiceSite _httpClient;
        public BannerViewComponent(IHttpClientServiceSite httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            //var BannerResult = await _httpClient.GetAsync("Banner/Get", false);                   // Old
            var BannerResult = await _httpClient.GetAsync("Banner/GetAllBanner", false);            // Karn Added
            var Banners = !string.IsNullOrEmpty(BannerResult) ? JsonConvert.DeserializeObject<List<BannerDTO>>(BannerResult) : null;


            return View(Banners);
        }
    }
}
