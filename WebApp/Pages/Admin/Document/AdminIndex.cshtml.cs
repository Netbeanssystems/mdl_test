using Application.Dtos;
using Application.Helpers;
using Application.ServiceInterfaces;
using Application.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApp.Pages.Admin.Document
{
    [Authorize(Roles = "BankAdmin")]

    public class AdminIndexModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly INotyfService _notyf;
        public AdminIndexModel(
           IHttpClientService httpClient,
           INotyfService notyf)
        {
            _httpClient = httpClient;
            _notyf = notyf;
        }
        public List<DocumentsVM> ModelVms { get; set; }
        [BindProperty] public DocumentsDTO ModelDto { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var modelResponse = await _httpClient.GetAsync("Documents/Get", true).ConfigureAwait(false);
            if (modelResponse == "unauthorized")
            {
                _notyf.Information("Please login");
                return RedirectToPage("/Account/Login");
            }
            ModelVms = !string.IsNullOrEmpty(modelResponse) ? JsonConvert.DeserializeObject<List<DocumentsVM>>(modelResponse) : null;
            if (ModelVms == null || ModelVms.Count <= 0)
                _notyf.Error("Academic Years not found");

            foreach (var item in ModelVms)
            {
                item.DocumentNameDecrypted = EnDeCryptor.DecryptStringAES(item.DocumentName.Split(".")[0].Replace("B_S", "\"").Replace("F_S", "/"));
            }
            return Page();
        }
    }
}
