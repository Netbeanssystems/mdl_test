using Application.ServiceInterfaces;
using Application.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebSite.Components
{
    public class WhatsNewViewComponent : ViewComponent
    {
        private readonly IHttpClientServiceSite _httpClient;
        public WhatsNewViewComponent(IHttpClientServiceSite httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            //var MenusResult = await _httpClient.GetAsync("WhatsNew/Get", false);              //----Old
            //var Menus = !string.IsNullOrEmpty(MenusResult) ? JsonConvert.DeserializeObject<List<WhatsNewVM>>(MenusResult) : null;

            var MenusResult = await _httpClient.GetAsync("WhatsNew/GetAllWhatsNew", false);                //-----------Karn 11Dec 2023
            var Menus = !string.IsNullOrEmpty(MenusResult) ? JsonConvert.DeserializeObject<List<WhatsNewVM>>(MenusResult) : null;

            return View(Menus);
        }
    }
}
