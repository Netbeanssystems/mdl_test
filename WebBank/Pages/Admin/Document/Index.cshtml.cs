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
using System.Linq;
using System.Threading.Tasks;

namespace WebBank.Pages.Admin.Document
{
    [Authorize(Roles = "BankUser,SuperAdmin")]

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
        public string uid => User.Identity.Name;
        public List<DocumentsVM> ModelVms { get; set; }
        [BindProperty] public DocumentsDTO ModelDto { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var modelResponse = await _httpClient.GetAsync("Documents/Getdocuments", true, uid).ConfigureAwait(false);

            if (modelResponse == "unauthorized")
            {
                _notyf.Information("Please login");
                return RedirectToPage("/Account/Login");
            }

            // 1. Deserialization
            var rawList = !string.IsNullOrEmpty(modelResponse)
                ? JsonConvert.DeserializeObject<List<DocumentsVM>>(modelResponse)
                : new List<DocumentsVM>();

            // 2. Sort by CreatedDate descending (Latest first)
            if (rawList != null && rawList.Count > 0)
            {
                ModelVms = rawList.OrderByDescending(x => x.CreatedDate).ToList();
            }
            else
            {
                ModelVms = new List<DocumentsVM>();
            }

            // 3. Validation Check
            if (ModelVms.Count <= 0)
            {
                _notyf.Error("Document not found");
                return Page();
            }

            //foreach (var item in ModelVms)
            //{
            //    item.DocumentNameDecrypted = EnDeCryptor.DecryptStringAES(item.DocumentName.Split(".")[0].Replace("B_S", "\"").Replace("F_S", "/"));
            //}

            foreach (var item in ModelVms)
            {
                try
                {
                    // Try decrypting (for old records)
                    item.DocumentNameDecrypted =
                        EnDeCryptor.DecryptStringAES(
                            item.DocumentName.Split(".")[0]
                                .Replace("B_S", "\"")
                                .Replace("F_S", "/")
                        );
                }
                catch
                {
                    // If not encrypted (new records), show directly
                    item.DocumentNameDecrypted = item.DocumentName;
                }
            }

            return Page();
        }
    }
}
