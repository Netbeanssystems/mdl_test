using Application.Dtos;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebSite.Pages.Career
{
    public class Career_ExecutivesModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        public Career_ExecutivesModel(IHttpClientService httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty] public List<CareerDTO> cont { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            string where = " tbl_Career_Jobs.Career_Cat_ID='1' AND tbl_Career_Jobs.Status='APP' AND tbl_Career_Jobs.Del_Sts='N'  AND tbl_Career_Jobs.Added_on >= DATEADD(mm, -6, GETDATE()) ";
            var modelResponse = await _httpClient.GetAsync("Career/GetCareers", false, where).ConfigureAwait(false);
            cont = !string.IsNullOrEmpty(modelResponse) ? JsonConvert.DeserializeObject<List<CareerDTO>>(modelResponse) : null;

            if (cont != null && cont.Count > 0)
            {
                cont = cont.OrderByDescending(s => s.Added_on).ToList();
                HttpContext.Session.SetString("LastDate", cont[0].Added_on.ToString());
            }

            return Page();
        }

        public async Task<IActionResult> OnGetBindSelectdata(string status)
        {
            string where = "";
            if (status == "archive")
            {
                where = " tbl_Career_Jobs.Career_Cat_ID='1' AND tbl_Career_Jobs.Status='APP' AND tbl_Career_Jobs.Del_Sts='N' AND tbl_Career_Jobs.Added_on < DATEADD(mm, -6, GETDATE()) ";
            }
            else
            {
                where = " tbl_Career_Jobs.Career_Cat_ID='1' AND tbl_Career_Jobs.Status='APP' AND tbl_Career_Jobs.Del_Sts='N'  AND tbl_Career_Jobs.Added_on >= DATEADD(mm, -6, GETDATE()) ";
            }

            var modelResponse = await _httpClient.GetAsync("Career/GetCareers", false, where).ConfigureAwait(false);
            cont = !string.IsNullOrEmpty(modelResponse) ? JsonConvert.DeserializeObject<List<CareerDTO>>(modelResponse) : null;
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
                    ViewName = "_CareerPartial",
                    ViewData = new ViewDataDictionary<List<CareerDTO>>(ViewData, cont)
                    
                   
                };
            }
            else
            {
                return new PartialViewResult
                {
                    ViewName = "_CareerPartialHindi",
                    ViewData = new ViewDataDictionary<List<CareerDTO>>(ViewData, cont)
                };
            }
            
        }
    }
}