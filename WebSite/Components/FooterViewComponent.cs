using Application.Dtos;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebSite.Components
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly IHttpClientServiceSite _httpClient;
        public FooterViewComponent(IHttpClientServiceSite httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty] public WebsiteCounterDTO Counter { get; set; }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var m = new WebsiteCounterDTO();
            m.Id = 1;

            var modelResponse = await _httpClient.GetAsync("WebsiteCounter/Get", false, (int)m.Id).ConfigureAwait(false);
            Counter = !string.IsNullOrEmpty(modelResponse) ? JsonConvert.DeserializeObject<WebsiteCounterDTO>(modelResponse) : null;

            string pageValue = HttpContext.Request.Path.ToString().ToLower();

            if (pageValue == "/" || pageValue == "/index")
            {
                if (Counter != null)
                {
                    Counter.NoofCount += 1;
                    m.NoofCount = Counter.NoofCount;
                }

                //var MenusResult = await _httpClient.GetAsync("WhatsNew/Get", false); 
                var MenusResult = await _httpClient.GetAsync("WhatsNew/GetFooterDate", false);
                var Menus = !string.IsNullOrEmpty(MenusResult) ? JsonConvert.DeserializeObject<List<FooterDateDTO>>(MenusResult) : null;

                //HttpContext.Session.SetString("LastDate", Menus[0].UpdatedOn.ToString());
                var response = await _httpClient.PutAsync("WebsiteCounter/Edit", false, m.Id, m).ConfigureAwait(false);
            }
            return View(Counter);
        }
    }
}
