using Application.Extensions;
using Application.ServiceInterfaces;
using Application.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebSite.Components
{
    public class MainHeaderViewComponent : ViewComponent
    {
        private readonly IHttpClientServiceSite _httpClient;
        public MainHeaderViewComponent(IHttpClientServiceSite httpClient)
        {
            _httpClient = httpClient;
        }
        //public async Task<IViewComponentResult> InvokeAsync()
        //{
        //    var MenusResult = await _httpClient.GetAsync("MenuHeadings/GetForMenu", false);
        //    var Menus = !string.IsNullOrEmpty(MenusResult) ? JsonConvert.DeserializeObject<List<MenuHeadingsCustomVM>>(MenusResult) : null;
        //    return View(Menus);
        //}

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var MenuHeadingVms = HttpContext.Session.GetObjectFromSession<List<MenuHeadingsCustomVM>>("Menus");     

            if (MenuHeadingVms == null)
            {
                var MenusResult = await _httpClient.GetAsync("MenuHeadings/GetForMenu", false);
                MenuHeadingVms = !string.IsNullOrEmpty(MenusResult) ? JsonConvert.DeserializeObject<List<MenuHeadingsCustomVM>>(MenusResult) : null;
                HttpContext.Session.SetObjectToSession("Menus", MenuHeadingVms);
            }


            return View(MenuHeadingVms);
        }
    }
}
