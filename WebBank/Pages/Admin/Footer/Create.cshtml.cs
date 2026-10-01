using Application.Dtos;
using Application.Extensions;
using Application.Helpers;
using Application.ServiceInterfaces;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace WebBank.Pages.Admin.Footer
{
    public class CreateModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly IFileService _fileService;
        private readonly INotyfService _notyf;
        public CreateModel(IHttpClientService httpClient, IFileService fileService, INotyfService notyf)
        {
            _httpClient = httpClient;
            _fileService = fileService;
            _notyf = notyf;
        }
        [FromRoute] public int? pid { get; set; }
        [BindProperty] public FooterContentDTO footer { get; set; }
        public async Task<IActionResult> OnPost()
        {
            footer = ModelAuditor<FooterContentDTO>.SetAudit(User.Identity?.Name, "Create", HttpContext.Connection.RemoteIpAddress?.ToString(), footer);
            if (!ModelState.IsValid) { _notyf.Error(ModelState.GetErrorMessageString()); return Page(); }

            var response = await _httpClient.PostAsync("Footer/Create", true, footer).ConfigureAwait(false);

            if (response == "unauthorized")
            {
                _notyf.Information("Please login");
                return RedirectToPage("/Account/Login");
            }
            footer = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<FooterContentDTO>(response) : null;
            if (footer == null)
                _notyf.Error("Save failed");
            else
                _notyf.Success("Saved successfully");
            return RedirectToPage("Index");
        }
    }
}
