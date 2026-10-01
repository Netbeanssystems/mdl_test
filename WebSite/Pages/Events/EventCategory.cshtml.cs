using Application.ServiceInterfaces;
using Application.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebSite.Pages.Events
{
    public class EventCategoryModel : PageModel
    {
        private readonly IHttpClientServiceSite _httpClient;
        private readonly INotyfService _notyf;
        public EventCategoryModel(IHttpClientServiceSite httpClient, INotyfService notyf)
        {
            _httpClient = httpClient;
            _notyf = notyf;
        }

        [BindProperty] public List<TopEventsVM> topEventsVMs { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            //Get Years for Dropdown
            try
            {
                var yearsResult = await _httpClient.GetAsync("PhotoGallery/GetYears", false);
                var years = !string.IsNullOrEmpty(yearsResult) ? JsonConvert.DeserializeObject<List<DropdownVM>>(yearsResult) : null;
                ViewData["years"] = new SelectList(years, "Id", "Text");

                TempData["latestyear"] = years[0].Id.ToString();

                if (years.Count > 0)
                {
                    var EventResult = await _httpClient.GetAsync("PhotoGallery/GetEventsByYear", false, years[0].Id);
                    //var EventResult = await _httpClient.GetAsync("PhotoGallery/GetAllEvents", false);
                    topEventsVMs = !string.IsNullOrEmpty(EventResult) ? JsonConvert.DeserializeObject<List<TopEventsVM>>(EventResult) : null;
                }

                if (topEventsVMs != null && topEventsVMs.Count > 0)
                {
                    topEventsVMs = topEventsVMs.OrderByDescending(s => s.Added_on).ToList();
                    HttpContext.Session.SetString("LastDate", topEventsVMs[0].Added_on.ToString());
                }
            }
            catch (Exception ex)
            {
                _notyf.Error(ex.Message);
                return Page();
                // return RedirectToPage("/Forms/Enquiry");
                //Console.WriteLine(ex.Message);
            }

            return Page();
        }

        public async Task<IActionResult> OnGetBindSelectdata(int year)
        {
            if (year != 0)
            {
                var EventResult = await _httpClient.GetAsync("PhotoGallery/GetEventsByYear", false, year);
                topEventsVMs = !string.IsNullOrEmpty(EventResult) ? JsonConvert.DeserializeObject<List<TopEventsVM>>(EventResult) : null;
                if (topEventsVMs.Count > 0)
                {
                    topEventsVMs = topEventsVMs.OrderByDescending(s => s.Added_on).ToList();
                    HttpContext.Session.SetString("LastDate", topEventsVMs[0].Added_on.ToString());
                }
            }
            else
            {

                var EventResult = await _httpClient.GetAsync("PhotoGallery/GetAllEvents", false);
                topEventsVMs = !string.IsNullOrEmpty(EventResult) ? JsonConvert.DeserializeObject<List<TopEventsVM>>(EventResult) : null;
                if (topEventsVMs.Count > 0)
                {
                    topEventsVMs = topEventsVMs.OrderByDescending(s => s.Added_on).ToList();
                    HttpContext.Session.SetString("LastDate", topEventsVMs[0].Added_on.ToString());
                }
            }

            return new PartialViewResult
            {
                ViewName = "_EventCategoryPartial",
                ViewData = new ViewDataDictionary<List<TopEventsVM>>(ViewData, topEventsVMs)
            };
        }
    }
}
