using Application.Extensions;
using Application.ServiceInterfaces;
using Application.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebBank.Helpers;


namespace WebBank.Components
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly IHttpClientService _httpClient;
        public MenuViewComponent(
            IHttpClientService httpClient
            )
        {
            _httpClient = httpClient;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var role = DataHelper.GetUserRole(HttpContext.User);
            var MenuVms = HttpContext.Session.GetObjectFromSession<List<MenusVM>>("Menus");
            if (MenuVms?.Count > 0) return View(MenuVms);
            var response = await _httpClient.GetAsync("Menus/GetAllByRole", true, role).ConfigureAwait(false);
            if (string.IsNullOrEmpty(response) || response == "unauthorized") return Content(string.Empty);
            MenuVms = JsonConvert.DeserializeObject<List<MenusVM>>(response);
            HttpContext.Session.SetObjectToSession("Menus", MenuVms);
            return View(MenuVms);
        }
    }
}
