using Application.ServiceInterfaces;
using Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebBank.Pages.Admin.PhotoGallery
{
    [Authorize(Roles = "SuperAdmin,Editors,Admin,ContentCreator,Moderator,Publisher")]
    public class IndexModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        public IndexModel(IHttpClientService httpClient)
        {
            _httpClient = httpClient;
        }

        public string role => User.Claims.First(c => c.Type == ClaimTypes.Role).Value;
        public List<PhotoGalleryVM> ModelVMs { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var Result = await _httpClient.GetAsync("PhotoGallery/Get", true);
            if (Result == "unauthorized")
            {
                TempData["Message"] = "info^Please login/register";
                return RedirectToPage("/Account/Login");
            }
            ModelVMs = !string.IsNullOrEmpty(Result) ? JsonConvert.DeserializeObject<List<PhotoGalleryVM>>(Result) : null;
            return Page();
        }

    }
}
