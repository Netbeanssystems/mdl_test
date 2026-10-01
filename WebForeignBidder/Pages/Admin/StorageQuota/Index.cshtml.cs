using Application.ServiceInterfaces;
using AspNetCoreHero.ToastNotification.Abstractions;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebForeignBidder.Pages.Admin.StorageQuota
{
    [Authorize(Roles = "BidderSuperAdmin")]
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
        public List<ProjectResponse> Projects { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            //get all the projects for dropdown
            var projectsResult = await _httpClient.GetAsync("Projects/Get", true).ConfigureAwait(false);
            if (projectsResult == "unauthorized") return RedirectToPage("/Account/Login");
            Projects = !string.IsNullOrEmpty(projectsResult) ? JsonConvert.DeserializeObject<List<ProjectResponse>>(projectsResult) : null;
            if (Projects == null || Projects.Count <= 0)
            {
                _notyf.Error("Record not found");
                return Page();
            }
            return Page();
        }
    }
}