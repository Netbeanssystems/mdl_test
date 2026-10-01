using Application.ServiceInterfaces;
using Application.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace WebApp.Pages.Admin.Video
{
    [Authorize(Roles = "SuperAdmin,Editors,Admin,ContentCreator,Moderator,Publisher")]

    public class PendingModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly INotyfService _notyf;
        public PendingModel(
            IHttpClientService httpClient, INotyfService notyf)
        {
            _httpClient = httpClient;
            _notyf = notyf;
        }

        public string role => User.Claims.First(c => c.Type == ClaimTypes.Role).Value;
        public List<TempVideoVM> TempvdoVM { get; set; }
        public PetrolPriceBuildupListVM PetrolPriceBuildupListVm { get; set; }

        //TempVideo
        public async Task<IActionResult> OnGetAsync(string status = "Pending")
        {
            PetrolPriceBuildupListVm = new PetrolPriceBuildupListVM { role = role };
            var tempNewProjectsResult = await _httpClient.GetAsync("TempVideo/GetByAction", true, status);
            if (tempNewProjectsResult == "unauthorized")
            {
                _notyf.Information("Please login/register");
                return RedirectToPage("/Account/Login");
            }
            PetrolPriceBuildupListVm.TempvdoVM = !string.IsNullOrEmpty(tempNewProjectsResult) ? JsonConvert.DeserializeObject<List<TempVideoVM>>(tempNewProjectsResult) : null;
            if (Request.Headers["x-requested-with"] != "XMLHttpRequest") return Page();
            return new PartialViewResult
            {
                ViewName = "_LayoutVideo",
                ViewData = new ViewDataDictionary<PetrolPriceBuildupListVM>(ViewData, PetrolPriceBuildupListVm)
            };

        }
    }
}
