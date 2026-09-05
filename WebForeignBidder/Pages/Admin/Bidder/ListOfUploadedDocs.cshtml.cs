using Application.Dtos;
using Application.Helpers;
using Application.ServiceInterfaces;
using Application.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebForeignBidder.Pages.Admin.Bidder
{
    [Authorize(Roles = "Bidder,ForeignBidder,SuperAdmin,HOD,CommercialExecutive")]
    public class ListOfUploadedDocsModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly INotyfService _notyf;
        public ListOfUploadedDocsModel(
           IHttpClientService httpClient,
           INotyfService notyf)
        {
            _httpClient = httpClient;
            _notyf = notyf;
        }
        public string uid => User.Identity.Name;
        public List<BidderTenderDocumentsVM> ModelVms { get; set; }

        public BidderTenderDocumentsVM GenUpload { get; set; }
        [BindProperty] public BidderTenderDocumentsDTO ModelDto { get; set; }
        public bool IsAdminOrExecutive { get; set; }
        public async Task<IActionResult> OnGetAsync(string tenderNo)
        {
            var modelResponse = await _httpClient.GetAsync("BidderTenderDocuments/Get", true).ConfigureAwait(false);
            if (modelResponse == "unauthorized")
            {
                _notyf.Information("Please login");
                return RedirectToPage("/Account/Login");
            }

            ModelVms = !string.IsNullOrEmpty(modelResponse)
                ? JsonConvert.DeserializeObject<List<BidderTenderDocumentsVM>>(modelResponse)
                    ?.OrderByDescending(x => x.Id)
                    .ToList()
                : null;

            bool isSuperAdmin = User.IsInRole("SuperAdmin") || User.IsInRole("BidderSuperAdmin") || (!string.IsNullOrEmpty(uid) && uid.Contains("SuperAdmin", StringComparison.OrdinalIgnoreCase));
            bool isCommExec = User.IsInRole("CommercialExecutive") || User.IsInRole("HOD") || (!string.IsNullOrEmpty(uid) && (uid.Contains("CommercialExecutive", StringComparison.OrdinalIgnoreCase) || uid.Contains("HOD", StringComparison.OrdinalIgnoreCase)));
            bool isBidder = User.IsInRole("Bidder") || User.IsInRole("ForeignBidder") || (!isSuperAdmin && !isCommExec);

            IsAdminOrExecutive = isSuperAdmin || isCommExec;

            if (ModelVms != null && ModelVms.Count > 0)
            {
                if (!isSuperAdmin)
                {
                    if (isCommExec)
                    {
                        if (!string.IsNullOrEmpty(tenderNo))
                        {
                            ModelVms = ModelVms.Where(x => string.Equals(x.TenderNo?.Trim(), tenderNo.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
                        }
                    }
                    else if (isBidder)
                    {
                        ModelVms = ModelVms.Where(x => !string.IsNullOrEmpty(x.CreatedBy) && string.Equals(x.CreatedBy.Trim(), uid?.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
                    }
                }
            }

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
                item.TenderDocDecrypted = EnDeCryptor.DecryptStringAES(item.Doc.Split(".")[0].Replace("B_S", "\"").Replace("F_S", "/"));
            }
            return Page();
        }


        public async Task<IActionResult> OnPostDownloadGenFileAsync(string Id)
        {
            string decryptedText = EnDeCryptor.DecryptStringAES(Id);
            int originalId = int.Parse(decryptedText);
            var FormsResult = await _httpClient.GetAsync("BidderTenderDocuments/Get", true, originalId).ConfigureAwait(false);
            GenUpload = !string.IsNullOrEmpty(FormsResult) ? JsonConvert.DeserializeObject<BidderTenderDocumentsVM>(FormsResult) : null;
            if (GenUpload != null)
            {

            }
            string currentDomain = $"{Request.Scheme}://{Request.Host}";
            //string fileUrl = $"{currentDomain}/Bidder/img/UploadedFiles/GeneralUpload/{GenUpload.Doc}";
            string fileUrl = $"{currentDomain}/Bidder/BidderTenders/{GenUpload.ProjectId}/{GenUpload.TenderNo}/{GenUpload.Doc}";
            //string fileUrl = $"{currentDomain}/bidderfiles/1/1333/{GenUpload.Doc}";

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
            string filenameWithExtension = $"{GenUpload.Doc}";
                var data = new { encdata = Convert.ToBase64String(fileBytes), filename = filenameWithExtension };
            return new JsonResult(data);
        }

    }
}
