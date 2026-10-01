using Application.Extensions;
using Application.ServiceInterfaces;
using Application.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace WebForeignBidder.Components
{
    public class AcademicYearsViewComponent : ViewComponent
    {
        private readonly IHttpClientService _httpClient;
        public AcademicYearsViewComponent(
            IHttpClientService httpClient
            )
        {
            _httpClient = httpClient;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var AcademicYearVms = HttpContext.Session.GetObjectFromSession<List<AcademicYearsVM>>("AcademicYears");
            if (AcademicYearVms == null)
            {
                var response = await _httpClient.GetAsync("AcademicYears/Get", true).ConfigureAwait(false);
                if (string.IsNullOrEmpty(response) || response == "unauthorized") return Content(string.Empty);
                AcademicYearVms = JsonConvert.DeserializeObject<List<AcademicYearsVM>>(response);
                if (AcademicYearVms != null)
                {
                    AcademicYearVms.RemoveAll(x => !x.IsActive);
                    HttpContext.Session.SetObjectToSession("AcademicYears",
                        AcademicYearVms.OrderByDescending(m => m.Year).ToList());
                }
            }
            //var response = await _httpClient.GetAsync("AcademicYears/Get", true).ConfigureAwait(false);
            //if (string.IsNullOrEmpty(response) || response == "unauthorized") return Content(string.Empty);
            //var AcademicYearVms = JsonConvert.DeserializeObject<List<AcademicYearsVM>>(response);
            var setVal = AcademicYearVms.FirstOrDefault(m => m.StartDate < DateTime.UtcNow.AddHours(5.5) && m.EndDate > DateTime.UtcNow.AddHours(5.5));
            foreach (var item in AcademicYearVms)
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetObjectFromSession<string>("AcademicYearId")))
                {
                    if (setVal != null && item.Id == setVal.Id)
                    {
                        HttpContext.Session.SetObjectToSession("AcademicYearId", setVal.Id);
                        item.IsSelect = true;
                    }
                    else
                        item.IsSelect = false;
                }
                else
                    item.IsSelect = item.Id == Convert.ToInt32(HttpContext.Session.GetObjectFromSession<string>("AcademicYearId"));
            }
            return View(AcademicYearVms);
        }
    }
}
