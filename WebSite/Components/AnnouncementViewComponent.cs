using Application.Dtos;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebSite.Components
{
    public class AnnouncementViewComponent : ViewComponent
    {
        private readonly IHttpClientServiceSite _httpClient;
        public AnnouncementViewComponent(IHttpClientServiceSite httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            //var MenusResult = await _httpClient.GetAsync("News/Get", false);                        //-----Old
            var MenusResult = await _httpClient.GetAsync("News/GetAllNewsList", false);                        //------Karn 11 Dec 2023
            var Menus = !string.IsNullOrEmpty(MenusResult) ? JsonConvert.DeserializeObject<List<NewsDTO>>(MenusResult) : null;

            //HttpContext.Session.SetString("LastDate", Menus[0].SubmitDate.ToString());
            return View(Menus);
        }
    }
}
