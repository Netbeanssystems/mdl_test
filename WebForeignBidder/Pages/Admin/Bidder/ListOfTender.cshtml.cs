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
    [Authorize(Roles = "Bidder,ForeignBidder,SuperAdmin,CommercialExecutive,HOD")]


    public class ListOfTenderModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly INotyfService _notyf;
        public ListOfTenderModel(
           IHttpClientService httpClient,
           INotyfService notyf)
        {
            _httpClient = httpClient;
            _notyf = notyf;
        }
        public string uid => User.Identity.Name;
        public List<BidderTenderUploadsVM> ModelVms { get; set; }
        public BidderTenderUploadsVM GenUpload { get; set; }
        [BindProperty] public BidderTenderUploadsDTO ModelDto { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var modelResponse = await _httpClient.GetAsync("BidderTenderUploads/Get", true).ConfigureAwait(false);
            if (modelResponse == "unauthorized")
            {
                _notyf.Information("Please login");
                return RedirectToPage("/Account/Login");
            }
            var allRecords = !string.IsNullOrEmpty(modelResponse)
    ? JsonConvert.DeserializeObject<List<BidderTenderUploadsVM>>(modelResponse)
    : null;

            if (allRecords == null || allRecords.Count <= 0)
            {
                _notyf.Error("Document not found");
                return Page();
            }
            if (!string.IsNullOrEmpty(uid) && uid.Contains("CommercialExecutive", StringComparison.OrdinalIgnoreCase))
            {
                ModelVms = !string.IsNullOrEmpty(modelResponse)
                    ? JsonConvert.DeserializeObject<List<BidderTenderUploadsVM>>(modelResponse)
                    : null;
            }
            else if (!string.IsNullOrEmpty(uid) && uid.Contains("HOD", StringComparison.OrdinalIgnoreCase))
            {
                ModelVms = !string.IsNullOrEmpty(modelResponse)
                    ? JsonConvert.DeserializeObject<List<BidderTenderUploadsVM>>(modelResponse)
                    : null;
            }
            else
            {
                ModelVms = allRecords
                    .Where(x => !string.IsNullOrEmpty(x.ForeignBidderId) &&
                                x.ForeignBidderId.Split(',')
                                    .Any(f => f.Trim().Equals(uid, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }
            // 🔑 Filter records to only those matching current user
            //     ModelVms = allRecords
            //.Where(x => !string.IsNullOrEmpty(x.ForeignBidderId) &&
            //            x.ForeignBidderId.Split(',').Any(f => f.Trim().Equals(uid, StringComparison.OrdinalIgnoreCase)))
            //.ToList();

            //     ModelVms = !string.IsNullOrEmpty(modelResponse) ? JsonConvert.DeserializeObject<List<BidderTenderUploadsVM>>(modelResponse) : null;

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
                item.TenderDocDecrypted = EnDeCryptor.DecryptStringAES(item.TenderDoc.Split(".")[0].Replace("B_S", "\"").Replace("F_S", "/"));
                DateTime? latestExtendedDate = null;
                foreach (var cor in item.TenderCorrigendums)
                {
                    if(cor.CorrigendumDoc != null) cor.CorrigendumDocDecrypted = EnDeCryptor.DecryptStringAES(cor.CorrigendumDoc.Split(".")[0].Replace("B_S", "\"").Replace("F_S", "/"));
                    if (cor.ExtendedDate.HasValue)
                    {
                        if (!latestExtendedDate.HasValue || cor.ExtendedDate > latestExtendedDate)
                        {
                            latestExtendedDate = cor.ExtendedDate;
                        }
                    }
                }
                item.LatestExtendedDate = latestExtendedDate;
            }
            return Page();
        }
        public async Task<IActionResult> OnPostDownloadGenFileAsync(string Id)
        {
            string decryptedText = EnDeCryptor.DecryptStringAES(Id);
            int originalId = int.Parse(decryptedText);
            var FormsResult = await _httpClient.GetAsync("BidderTenderUploads/Get", true, originalId).ConfigureAwait(false);
            GenUpload = !string.IsNullOrEmpty(FormsResult) ? JsonConvert.DeserializeObject<BidderTenderUploadsVM>(FormsResult) : null;
            if (GenUpload != null)
            {

            }
            string currentDomain = $"{Request.Scheme}://{Request.Host}";
            string fileUrl = $"{currentDomain}/bidder/BidderTenders/{GenUpload.ProjectId}/{GenUpload.TenderNo}/{GenUpload.TenderDoc}";

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
            string filenameWithExtension = $"{GenUpload.TenderDoc}";
            var data = new { encdata = Convert.ToBase64String(fileBytes), filename = filenameWithExtension };
            return new JsonResult(data);
        }

    }
}
