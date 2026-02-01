using Application.ServiceInterfaces;
using Application.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace WebBank.Pages.Admin.Masters.Districts
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class IndexModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly INotyfService _notyf;
        public IndexModel(
            IHttpClientService httpClient,
            INotyfService notyf)
        {
            _httpClient = httpClient;
            _notyf = notyf;
        }
        public Dictionary<string, bool> permissions => JsonConvert.DeserializeObject<Dictionary<string, bool>>(User.Claims.First(x => x.Type == "per").Value);
        public List<DistrictsVM> Districts { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            if (!permissions["CanView"]) return RedirectToPage("/Errors/AccessDenied");
            var modelResponse = await _httpClient.GetAsync("Districts/Get", true).ConfigureAwait(false);
            if (modelResponse == "unauthorized")
            {
                _notyf.Information("Please login");
                return RedirectToPage("/Account/Login");
            }
            Districts = !string.IsNullOrEmpty(modelResponse) ? JsonConvert.DeserializeObject<List<DistrictsVM>>(modelResponse) : null;
            if (Districts == null || Districts.Count <= 0)
                _notyf.Error("Districts not found");
            return Page();
        }
    }
}