using Application.Dtos;
using Application.ServiceInterfaces;
using Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApp.Pages.Admin.HomePage.Banner
{
    [Authorize(Roles = "SuperAdmin,Editors,Admin,ContentCreator,Moderator,Publisher")]
    //[SessionManage]
    [IgnoreAntiforgeryToken]
    public class IndexModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        public IndexModel(IHttpClientService httpClient)
        {
            _httpClient = httpClient;
        }
        public List<BannerVM> Headings { get; set; }
        public BannerDTO FirstLblMenuDTO { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CategoryId { get; set; }

        public string Status = "";

        public async Task<IActionResult> OnGetFirstLayerMenuList()
        {
            var Result = await _httpClient.GetAsync("Banner/GetMenuHeadingsWithAll", true);
            Headings = !string.IsNullOrEmpty(Result) ? JsonConvert.DeserializeObject<List<BannerVM>>(Result) : null;
            if (Result == "unauthorized")
            {
                Status = "Unauthorized";
                return new JsonResult(Status);
            }
            else
            {
                if (Headings == null)
                    Status = "Failed";
                else
                    Status = "Success";
            }
            return new JsonResult(Headings);
        }

        public async Task<IActionResult> OnGetSubCategories(int CategoryId)
        {
            var Result = await _httpClient.GetAsync("Banner/GetTest", true, CategoryId);
            Headings = !string.IsNullOrEmpty(Result) ? JsonConvert.DeserializeObject<List<BannerVM>>(Result) : null;
            if (Result == "unauthorized")
            {
                Status = "Unauthorized";
                return new JsonResult(Status);
            }
            else
            {
                if (Headings == null)
                    Status = "Failed";
                else
                    Status = "Success";
            }
            return new JsonResult(Headings);
        }

        public async Task<JsonResult> OnGetMenuHeading(int ID)
        {
            var Result = await _httpClient.GetAsync("Banner/Get", true, ID);
            FirstLblMenuDTO = !string.IsNullOrEmpty(Result) ? JsonConvert.DeserializeObject<BannerDTO>(Result) : null;
            if (Result == "unauthorized")
            {
                Status = "Unauthorized";
                return new JsonResult(Status);
            }
            else
            {
                if (FirstLblMenuDTO == null)
                    Status = "Failed";
                else
                    Status = "Success";
            }
            return new JsonResult(Result);
        }

        public async Task<IActionResult> OnGetPriority(string menuHeadingDTOs)
        {
            var menuHeadings = JsonConvert.DeserializeObject<List<MenuHeadingListPriorityDto>>(menuHeadingDTOs);
            var Result = await _httpClient.PostAsync("Banner/UpdatePriority", true, menuHeadings);
            string Headings = JsonConvert.DeserializeObject<string>(Result);
            return new JsonResult(Headings);
        }
    }
}