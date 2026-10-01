using Application.Dtos;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebSite.Pages.Career
{
    public class ExExecutiveModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        public ExExecutiveModel(IHttpClientService httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty] public List<RetireCareerDTO> cont { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {

                string where = " tbl_ExEmployee_CareerJobs.Ex_Employee_Career_CatID='1' AND tbl_ExEmployee_CareerJobs.Status='APP' AND tbl_ExEmployee_CareerJobs.Del_Sts='N' AND DATEPART(yyyy, CONVERT(DATETIME, Ex_Employee_Date_of_Posting,103))>= DATEPART(yyyy, DateAdd(yy, -1, GetDate())) ";
                var modelResponse = await _httpClient.GetAsync("Career/GetExCareers", false, where).ConfigureAwait(false);
                cont = !string.IsNullOrEmpty(modelResponse) ? JsonConvert.DeserializeObject<List<RetireCareerDTO>>(modelResponse) : null;

                if (cont != null && cont.Count > 0)
                {
                    cont = cont.OrderByDescending(s => s.Added_on).ToList();
                   // HttpContext.Session.SetString("UpdateLastDate", cont[0].Added_on.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return Page();
        }

        public async Task<IActionResult> OnGetBindSelectdata(string status)
        {
            string where = "";
            if (status == "archive")
            {
                where = " tbl_ExEmployee_CareerJobs.Ex_Employee_Career_CatID='1' AND tbl_ExEmployee_CareerJobs.Status='APP' AND tbl_ExEmployee_CareerJobs.Del_Sts='N' AND DATEPART(yyyy, CONVERT(DATETIME, Ex_Employee_Date_of_Posting, 103)) < DATEPART(yyyy, DateAdd(yy, -1, GetDate())) ";
            }
            else
            {
                where = " tbl_ExEmployee_CareerJobs.Ex_Employee_Career_CatID='1' AND tbl_ExEmployee_CareerJobs.Status='APP' AND tbl_ExEmployee_CareerJobs.Del_Sts='N' AND DATEPART(yyyy, CONVERT(DATETIME, Ex_Employee_Date_of_Posting,103))>= DATEPART(yyyy, DateAdd(yy, -1, GetDate())) ";
            }
            var modelResponse = await _httpClient.GetAsync("Career/GetExCareers", false, where).ConfigureAwait(false);
            cont = !string.IsNullOrEmpty(modelResponse) ? JsonConvert.DeserializeObject<List<RetireCareerDTO>>(modelResponse) : null;
            string lang = HttpContext.Session.GetString("Lang");
            if (cont != null && cont.Count > 0)
            {
                cont = cont.OrderByDescending(s => s.Added_on).ToList();
                //HttpContext.Session.SetString("UpdateLastDate", cont[0].Added_on.ToString());
            }
            if (string.IsNullOrEmpty(lang) || lang == "English")
            {
                return new PartialViewResult
                {
                    ViewName = "_RetireCareerPartial",
                    ViewData = new ViewDataDictionary<List<RetireCareerDTO>>(ViewData, cont)
                };
            }
            else
            {
                return new PartialViewResult
                {
                    ViewName = "_RetireCareerPartialHindi",
                    ViewData = new ViewDataDictionary<List<RetireCareerDTO>>(ViewData, cont)
                };
            }
        }
    }
}
