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


namespace WebApp.Pages.Admin.OtherLinkHeading
{
    [Authorize(Roles = "SuperAdmin,Editors,Admin,ContentCreator,Moderator,Publisher")]

    public class PendingModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly INotyfService _notyf;
        public PendingModel(IHttpClientService httpClient, INotyfService notyf)
        {
            _httpClient = httpClient;
            _notyf = notyf;
        }

        public string role => User.Claims.First(c => c.Type == ClaimTypes.Role).Value;
        public List<TempOtherLinkHeadingVM> TempModelVM { get; set; }
        public PetrolPriceBuildupListVM PetrolPriceBuildupListVm { get; set; }
        public async Task<IActionResult> OnGetAsync(string status = "Pending")
        {
            PetrolPriceBuildupListVm = new PetrolPriceBuildupListVM { role = role };
            //var Result = await _httpClient.GetAsync("TempOtherLinkHeading/GetByAction", true, status);
            var Result = await _httpClient.GetAsync("TempOtherLink/GetByAction", true, status);
            if (Result == "unauthorized")
            {
                _notyf.Information("Please login/register");
                return RedirectToPage("/Account/Login");
            }
            PetrolPriceBuildupListVm.ListProduct = !string.IsNullOrEmpty(Result) ? JsonConvert.DeserializeObject<List<TempOtherLinkHeadingVM>>(Result) : null;
            if (Request.Headers["x-requested-with"] != "XMLHttpRequest") return Page();

            return new PartialViewResult
            {
                ViewName = "_ProductTable",
                ViewData = new ViewDataDictionary<PetrolPriceBuildupListVM>(ViewData, PetrolPriceBuildupListVm)
            };
        }
    }
}