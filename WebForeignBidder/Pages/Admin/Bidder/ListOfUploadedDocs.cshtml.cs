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
using System.IO;
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
        public string TenderNoFilter { get; set; }
        public bool OnlyOpenedFilter { get; set; }

        public async Task<IActionResult> OnGetAsync(string tenderNo, bool? onlyOpened)
        {
            TenderNoFilter = tenderNo;
            OnlyOpenedFilter = onlyOpened ?? false;

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

            // Load tenders to check opening date for each document
            var tendersResponse = await _httpClient.GetAsync("BidderTenderUploads/Get", true).ConfigureAwait(false);
            List<BidderTenderUploadsVM> allTenders = null;
            if (!string.IsNullOrEmpty(tendersResponse) && tendersResponse != "unauthorized")
            {
                try
                {
                    allTenders = JsonConvert.DeserializeObject<List<BidderTenderUploadsVM>>(tendersResponse);
                }
                catch { }
            }

            var tenderLookup = allTenders?
                .Where(t => !string.IsNullOrWhiteSpace(t.TenderNo))
                .GroupBy(t => t.TenderNo.Trim(), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase)
                ?? new Dictionary<string, BidderTenderUploadsVM>(StringComparer.OrdinalIgnoreCase);

            var now = DateTime.Now;

            foreach (var item in ModelVms)
            {
                var encrptVal = item.Id.ToString();
                byte[] encryptedBytes = EnDeCryptor.EncryptStringAES(encrptVal);
                item.EncryptedId = Convert.ToBase64String(encryptedBytes);

                try
                {
                    var encPart = item.Doc?.Split(".")[0].Replace("B_S", "\"").Replace("F_S", "/");
                    item.TenderDocDecrypted = EnDeCryptor.DecryptStringAES(encPart);
                }
                catch
                {
                    item.TenderDocDecrypted = item.DocTitle ?? item.Doc;
                }

                if (!string.IsNullOrWhiteSpace(item.TenderNo) && tenderLookup.TryGetValue(item.TenderNo.Trim(), out var tender))
                {
                    item.TenderOpeningDate = (tender.TenderOpeningDate != default) ? tender.TenderOpeningDate : null;
                }

                // Fallback to GetByTenderNo if not in bulk response
                if (!item.TenderOpeningDate.HasValue && !string.IsNullOrWhiteSpace(item.TenderNo))
                {
                    try
                    {
                        var singleTenderRes = await _httpClient.GetAsync($"BidderTenderUploads/GetByTenderNo?tenderNo={item.TenderNo.Trim()}", true).ConfigureAwait(false);
                        if (!string.IsNullOrEmpty(singleTenderRes) && singleTenderRes != "unauthorized" && singleTenderRes.TrimStart().StartsWith("{"))
                        {
                            var singleTender = JsonConvert.DeserializeObject<BidderTenderUploadsVM>(singleTenderRes);
                            if (singleTender != null && singleTender.TenderOpeningDate != default)
                            {
                                item.TenderOpeningDate = singleTender.TenderOpeningDate;
                            }
                        }
                    }
                    catch { }
                }

                item.IsOpen = item.TenderOpeningDate.HasValue && now >= item.TenderOpeningDate.Value;
            }

            if (OnlyOpenedFilter)
            {
                ModelVms = ModelVms.Where(x => x.IsOpen).ToList();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDownloadGenFileAsync(string Id)
        {
            if (string.IsNullOrWhiteSpace(Id))
            {
                return new JsonResult(new { success = false, message = "Invalid document identifier." });
            }

            string decryptedText = EnDeCryptor.DecryptStringAES(Id);
            if (!int.TryParse(decryptedText, out int originalId))
            {
                return new JsonResult(new { success = false, message = "Invalid document ID." });
            }

            var FormsResult = await _httpClient.GetAsync("BidderTenderDocuments/Get", true, originalId).ConfigureAwait(false);
            GenUpload = !string.IsNullOrEmpty(FormsResult) ? JsonConvert.DeserializeObject<BidderTenderDocumentsVM>(FormsResult) : null;
            if (GenUpload == null)
            {
                return new JsonResult(new { success = false, message = "Document not found." });
            }

            // Verify if Tender Opening Date has passed
            if (!string.IsNullOrEmpty(GenUpload.TenderNo))
            {
                BidderTenderUploadsVM tender = null;
                var tenderRes = await _httpClient.GetAsync($"BidderTenderUploads/GetByTenderNo?tenderNo={GenUpload.TenderNo.Trim()}", true).ConfigureAwait(false);
                if (!string.IsNullOrEmpty(tenderRes) && tenderRes != "unauthorized" && tenderRes.TrimStart().StartsWith("{"))
                {
                    try { tender = JsonConvert.DeserializeObject<BidderTenderUploadsVM>(tenderRes); } catch { }
                }

                if (tender == null)
                {
                    var allTendersRes = await _httpClient.GetAsync("BidderTenderUploads/Get", true).ConfigureAwait(false);
                    if (!string.IsNullOrEmpty(allTendersRes) && allTendersRes != "unauthorized")
                    {
                        try
                        {
                            var allTenders = JsonConvert.DeserializeObject<List<BidderTenderUploadsVM>>(allTendersRes);
                            tender = allTenders?.FirstOrDefault(x => string.Equals(x.TenderNo?.Trim(), GenUpload.TenderNo.Trim(), StringComparison.OrdinalIgnoreCase));
                        }
                        catch { }
                    }
                }

                if (tender != null && tender.TenderOpeningDate != default && DateTime.Now < tender.TenderOpeningDate)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = $"Tender opening date ({tender.TenderOpeningDate:dd-MM-yyyy HH:mm:ss}) has not passed yet. Documents cannot be downloaded before the opening date."
                    });
                }
            }

            // Check physical files on disk first
            byte[] fileBytes = null;
            var localPaths = new[]
            {
                Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "BidderTenders", GenUpload.ProjectId ?? "", GenUpload.TenderNo ?? "", GenUpload.Doc ?? ""),
                Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "bidder", "BidderTenders", GenUpload.ProjectId ?? "", GenUpload.TenderNo ?? "", GenUpload.Doc ?? ""),
                Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Bidder", "img", "UploadedFiles", "GeneralUpload", GenUpload.Doc ?? "")
            };

            foreach (var p in localPaths)
            {
                if (System.IO.File.Exists(p))
                {
                    fileBytes = await System.IO.File.ReadAllBytesAsync(p);
                    break;
                }
            }

            if (fileBytes == null || fileBytes.Length == 0)
            {
                string currentDomain = $"{Request.Scheme}://{Request.Host}";
                string fileUrl = $"{currentDomain}/Bidder/BidderTenders/{GenUpload.ProjectId}/{GenUpload.TenderNo}/{GenUpload.Doc}";

                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        fileBytes = await client.GetByteArrayAsync(fileUrl);
                    }
                    catch (Exception ex)
                    {
                        return new JsonResult(new { success = false, message = "File could not be loaded: " + ex.Message });
                    }
                }
            }

            if (fileBytes == null || fileBytes.Length == 0)
            {
                return new JsonResult(new { success = false, message = "File content is empty or not found on server." });
            }

            string filenameWithExtension = $"{GenUpload.Doc}";
            var data = new { success = true, encdata = Convert.ToBase64String(fileBytes), filename = filenameWithExtension };
            return new JsonResult(data);
        }

    }
}

