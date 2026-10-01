using Application.Dtos;
using Application.Helpers;
using Application.ServiceInterfaces;
using Application.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebForeignBidder.Pages.Admin.Hod
{
    [Authorize(Roles = "HOD,SuperAdmin")]
    public class GeneralDocumentListModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly INotyfService _notyf;
        public GeneralDocumentListModel(
           IHttpClientService httpClient,
           INotyfService notyf)
        {
            _httpClient = httpClient;
            _notyf = notyf;
        }
        public string uid => User.Identity.Name;
        public List<GenaralUploadDocumentsVM> ModelVms { get; set; }
                public GenaralUploadDocumentsVM GenUpload { get; set; }
        [BindProperty] public GenaralUploadDocumentsDTO ModelDto { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var modelResponse = await _httpClient.GetAsync("GenaralUploadDocuments/Get", true).ConfigureAwait(false);
            if (modelResponse == "unauthorized")
            {
                _notyf.Information("Please login");
                return RedirectToPage("/Account/Login");
            }
            //ModelVms = !string.IsNullOrEmpty(modelResponse) ? JsonConvert.DeserializeObject<List<GenaralUploadDocumentsVM>>(modelResponse) : null;
            ModelVms = !string.IsNullOrEmpty(modelResponse)
    ? JsonConvert.DeserializeObject<List<GenaralUploadDocumentsVM>>(modelResponse)
        ?.OrderByDescending(x => x.Id)
        .ToList()
    : new List<GenaralUploadDocumentsVM>();
            if (ModelVms == null || ModelVms.Count <= 0)
            {
                _notyf.Error("Document not found");
                return Page();
            }

            foreach (var item in ModelVms)
            {
                var encrptVal = item.Id.ToString();
                byte[] encryptedBytes = EnDeCryptor.EncryptStringAES(encrptVal);
                item.EncryptedId = Convert.ToBase64String(encryptedBytes);
                item.DocumentNameDecrypted = EnDeCryptor.DecryptStringAES(item.DocumentName.Split(".")[0].Replace("B_S", "\"").Replace("F_S", "/"));
            }
            return Page();
        }
        public async Task<IActionResult> OnPostDownloadGenFileAsync(string Id)
        {
            string decryptedText = EnDeCryptor.DecryptStringAES(Id);
            int originalId = int.Parse(decryptedText);
            var FormsResult = await _httpClient.GetAsync("GenaralUploadDocuments/Get", true, originalId).ConfigureAwait(false);
            GenUpload = !string.IsNullOrEmpty(FormsResult) ? JsonConvert.DeserializeObject<GenaralUploadDocumentsVM>(FormsResult) : null;
            if (GenUpload != null)
            {

            }
            string currentDomain = $"{Request.Scheme}://{Request.Host}";
            string fileUrl = $"{currentDomain}/Bidder/img/UploadedFiles/GeneralUpload/{GenUpload.DocumentName}";

            byte[] fileBytes;
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    fileBytes = await client.GetByteArrayAsync(fileUrl);
                }
                catch (Exception ex)
                {
                    return new JsonResult(new { success = false, message = ex.Message });
                }
            }
            // Extract the file extension from the file path or name
            // string fileExtension = Path.GetExtension(GenUpload.DocumentName);

            // Create the file name including the extension
            string filenameWithExtension = $"{GenUpload.DocumentName}";
            var data = new { encdata = Convert.ToBase64String(fileBytes), filename = filenameWithExtension };
            return new JsonResult(data);
        }
    }
}
