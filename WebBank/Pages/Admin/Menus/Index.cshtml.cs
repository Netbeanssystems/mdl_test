using Application.ServiceInterfaces;
using Application.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace WebBank.Pages.Admin.Menus
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
        public List<MenusVM> Menus { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var MenusResult = await _httpClient.GetAsync("Menus/GetWithAll", true).ConfigureAwait(false);
            if (MenusResult == "unauthorized")
            {
                _notyf.Information("Please login");
                return RedirectToPage("/Account/Login");
            }
            Menus = !string.IsNullOrEmpty(MenusResult) ? JsonConvert.DeserializeObject<List<MenusVM>>(MenusResult) : null;
            if (Menus == null || Menus.Count <= 0)
                _notyf.Error("Menus not found");
            return Page();
        }
    }
}