using Application.ServiceInterfaces;
using Application.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace WebForeignBidder.Components
{
    public class NotificationsViewComponent : ViewComponent
    {
        private readonly IHttpClientService _httpClient;
        public NotificationsViewComponent(
            IHttpClientService httpClient
            )
        {
            _httpClient = httpClient;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            //var NotificationVms = HttpContext.Session.GetObjectFromSession<List<NotificationsVM>>("Notifications");
            //if (NotificationVms != null && NotificationVms.Count > 0) return View(NotificationVms);
            var response = await _httpClient.GetAsync("Notifications/GetByUser", true, HttpContext.User.Identity?.Name).ConfigureAwait(false);
            if (string.IsNullOrEmpty(response) || response == "unauthorized") return Content(string.Empty);
            var NotificationVms = JsonConvert.DeserializeObject<List<NotificationsVM>>(response);
            //HttpContext.Session.SetObjectToSession("Notifications", NotificationVms);
            return View(NotificationVms);
        }
    }
}
