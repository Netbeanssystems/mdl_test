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

namespace WebBank.Pages.Admin.PhotoGallery
{
    [Authorize(Roles = "SuperAdmin,Editors,Admin,ContentCreator,Moderator,Publisher")]
    public class PendingModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        public PendingModel(
            IHttpClientService httpClient
        )
        {
            _httpClient = httpClient;
        }

        public string role => User.Claims.First(c => c.Type == ClaimTypes.Role).Value;
        public List<TempPhotoGalleryVM> Tempphotogallery { get; set; }
        public PetrolPriceBuildupListVM PetrolPriceBuildupListVm { get; set; }
        public async Task<IActionResult> OnGetAsync(string status = "Pending")
        {
            PetrolPriceBuildupListVm = new PetrolPriceBuildupListVM { role = role };
            var tempNewProjectsResult = await _httpClient.GetAsync("TempPhotoGallery/GetByAction", true, status);
            if (tempNewProjectsResult == "unauthorized")
            {
                TempData["Message"] = "info^Please login/register";
                return RedirectToPage("/Account/Login");
            }
            PetrolPriceBuildupListVm.Tempphotogallery = !string.IsNullOrEmpty(tempNewProjectsResult) ? JsonConvert.DeserializeObject<List<TempPhotoGalleryVM>>(tempNewProjectsResult) : null;
            if (Request.Headers["x-requested-with"] != "XMLHttpRequest") return Page();
            return new PartialViewResult
            {
                ViewName = "_LayoutPhotoGallery",
                ViewData = new ViewDataDictionary<PetrolPriceBuildupListVM>(ViewData, PetrolPriceBuildupListVm)
            };

        }
    }
}
