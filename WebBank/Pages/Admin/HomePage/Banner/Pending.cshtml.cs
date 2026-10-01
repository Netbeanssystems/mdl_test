using Application.ServiceInterfaces;
using Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebBank.Pages.Admin.HomePage.Banner
{
    [Authorize(Roles = "SuperAdmin,Editors,Admin,ContentCreator,Moderator,Publisher")]
    //[SessionManage]
    public class PendingModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        public PendingModel(IHttpClientService httpClient)
        {
            _httpClient = httpClient;
        }

        public string role => User.Claims.First(c => c.Type == ClaimTypes.Role).Value;
        public List<TempBannerVM> TemphomepagebannersVM { get; set; }
        public PetrolPriceBuildupListVM PetrolPriceBuildupListVm { get; set; }
        public async Task<IActionResult> OnGetAsync(string status = "Pending")
        {
            PetrolPriceBuildupListVm = new PetrolPriceBuildupListVM { role = role };
            var Result = await _httpClient.GetAsync("TempBanner/GetByAction", true, status);
            if (Result == "unauthorized")
            {
                TempData["Message"] = "info^Please login/register";
                return RedirectToPage("/Account/Login");
            }
            PetrolPriceBuildupListVm.TempbannersVM = !string.IsNullOrEmpty(Result) ? JsonConvert.DeserializeObject<List<TempBannerVM>>(Result) : null;
            if (Request.Headers["x-requested-with"] != "XMLHttpRequest") return Page();

            return new PartialViewResult
            {
                ViewName = "_BannersLayout",
                ViewData = new ViewDataDictionary<PetrolPriceBuildupListVM>(ViewData, PetrolPriceBuildupListVm)
            };
        }
    }
}