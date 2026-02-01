using Application.ServiceInterfaces;
using AspNetCoreHero.ToastNotification.Abstractions;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebForeignBidder.Pages.Admin.Projects
{
    [Authorize(Roles = "SuperAdmin,BidderSuperAdmin")]
    public class IndexModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly INotyfService _notyf;
        public IndexModel(
            IHttpClientService httpClient,
            IEmailService emailService,
            IConfiguration config,
            IHttpContextAccessor httpContextAccessor,
            INotyfService notyf)
        {
            _httpClient = httpClient;
            _emailService = emailService;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
            _notyf = notyf;
        }
        public List<ProjectResponse> Projects { get; set; }
        public async Task<IActionResult> OnGetAsync(string role)
        {
            //get all the roles for dropdown
            var Result = await _httpClient.GetAsync("Projects/GetProjects", true).ConfigureAwait(false);
            if (Result == "unauthorized") return RedirectToPage("/Account/Login");
            Projects = !string.IsNullOrEmpty(Result) ? JsonConvert.DeserializeObject<List<ProjectResponse>>(Result) : null;
            //Projects.ForEach(project =>
            //{
            //    project.CreatedDate = project.CreatedDate > project.ModifiedDate ? project.CreatedDate : project.ModifiedDate
            //});
            if (Projects == null || Projects.Count <= 0)
            {
                _notyf.Error("Projects not found");
                return Page();
            }
            return Page();
        }
    }
}