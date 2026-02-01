using Application.Dtos;
using Application.ServiceInterfaces;
using Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebBank.Pages.Admin.OtherLinkHeading
{
    [Authorize(Roles = "SuperAdmin,Editors,Admin,ContentCreator,Moderator,Publisher")]

    public class IndexModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        public IndexModel(IHttpClientService httpClient)
        {
            _httpClient = httpClient;
        }

        public List<OtherLinkHeadingVM> Headings { get; set; }
        public OtherLinkHeadingDTO FirstLblMenuHeadingDto { get; set; }
        [BindProperty(SupportsGet = true)]
        public int CategoryId { get; set; }
        public string Status = "";

        public async Task<IActionResult> OnGetFirstLayerMenuList()
        {
            var Result = await _httpClient.GetAsync("OtherLinkHeading/GetMenuHeadingsWithAll", true);
            Headings = !string.IsNullOrEmpty(Result) ? JsonConvert.DeserializeObject<List<OtherLinkHeadingVM>>(Result) : null;
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

        public async Task<IActionResult> OnGetSubCategories()
        {
            var Result = await _httpClient.GetAsync("OtherLinkHeading/GetTest", true, CategoryId);
            Headings = !string.IsNullOrEmpty(Result) ? JsonConvert.DeserializeObject<List<OtherLinkHeadingVM>>(Result) : null;
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
            var Result = await _httpClient.GetAsync("OtherLinkHeading/Get", true, ID);
            FirstLblMenuHeadingDto = !string.IsNullOrEmpty(Result) ? JsonConvert.DeserializeObject<OtherLinkHeadingDTO>(Result) : null;
            if (Result == "unauthorized")
            {
                Status = "Unauthorized";
                return new JsonResult(Status);
            }
            else
            {
                if (FirstLblMenuHeadingDto == null)
                    Status = "Failed";
                else
                    Status = "Success";
            }
            return new JsonResult(Result);
        }
        public async Task<IActionResult> OnGetPriority(string menuHeadingDTOs)
        {
            var menuHeadings = JsonConvert.DeserializeObject<List<OtherLinkListPriorityDTO>>(menuHeadingDTOs);
            var Result = await _httpClient.PostAsync("OtherLinkHeading/UpdatePriority", true, menuHeadings);
            string Headings = JsonConvert.DeserializeObject<string>(Result);
            return new JsonResult(Headings);
        }
    }
}