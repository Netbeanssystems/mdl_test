using Application.Dtos;
using Application.Helpers;
using Application.ServiceInterfaces;
using Application.Services;
using Application.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebForeignBidder.Pages.Admin.Bidder
{
    [Authorize(Roles = "Bidder,ForeignBidder,SuperAdmin")]


    public class AddTenderDocModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly INotyfService _notyf;
        private readonly IFileService _fileService;
        public AddTenderDocModel(
           IHttpClientService httpClient,
           INotyfService notyf,
           IFileService fileService)
        {
            _httpClient = httpClient;
            _notyf = notyf;
            _fileService = fileService;
        }
        public string uid => User.Identity.Name;
        public List<BidderTenderUploadsVM> ModelVms { get; set; }
        [BindProperty] public BidderTenderUploadsDTO ModelDto { get; set; }
        public BidderTenderDocumentsVM _TenderDocVM { get; set; }
        //public async Task<IActionResult> OnGetAsync()
        //{
        //    //get all the projects for dropdown
        //    var tendersResult = await _httpClient.GetAsync("BidderTenderUploads/Get", true).ConfigureAwait(false);
        //    if (tendersResult == "unauthorized") return RedirectToPage("/Account/Login");
        //    var tenders = !string.IsNullOrEmpty(tendersResult) ? JsonConvert.DeserializeObject<List<BidderTenderUploadsVM>>(tendersResult) : null;
        //    if (tenders == null || tenders.Count <= 0)
        //    {
        //        _notyf.Error("Record not found");
        //        return Page();
        //    }

        //    ViewData["TenderNos"] = new SelectList(tenders, "TenderNo", "TenderNo");
        //    return Page();
        //}
        public async Task<IActionResult> OnGetAsync()
        {
            // Get current user id (string)
            var uid = User?.Identity?.Name;
            if (string.IsNullOrEmpty(uid))
            {
                _notyf.Error("User not found");
                return RedirectToPage("/Account/Login");
            }

            // Fetch tender data from API
            var tendersResult = await _httpClient.GetAsync("BidderTenderUploads/Get", true)
                                                 .ConfigureAwait(false);

            if (tendersResult == "unauthorized")
                return RedirectToPage("/Account/Login");

            var tenders = !string.IsNullOrEmpty(tendersResult)
                ? JsonConvert.DeserializeObject<List<BidderTenderUploadsVM>>(tendersResult)
                : new List<BidderTenderUploadsVM>();

            // Filter tenders by UID and TenderStartDate
            var filteredTenders = tenders
     .Where(t => !string.IsNullOrEmpty(t.ForeignBidderId) &&
                 t.ForeignBidderId.Split(',').Any(u => u.Trim().Equals(uid, StringComparison.OrdinalIgnoreCase)) &&
                 (!t.TenderStartDate.HasValue || t.TenderStartDate.Value <= DateTime.Now))
     .ToList();

            if (filteredTenders.Count == 0)
            {
                _notyf.Error("No tenders found for this user");
                return Page();
            }

            // Bind filtered tenders
            ViewData["TenderNos"] = new SelectList(filteredTenders, "TenderNo", "TenderNo");

            return Page();
        }

        public async Task<IActionResult> OnGetGetUploadedDocsAsync(string tenderNo)
        {
            if (string.IsNullOrEmpty(tenderNo)) return new JsonResult(new List<object>());

            var docsResult = await _httpClient.GetAsync("BidderTenderDocuments/Get", true).ConfigureAwait(false);
            if (docsResult == "unauthorized" || string.IsNullOrEmpty(docsResult)) return new JsonResult(new List<object>());

            var docs = JsonConvert.DeserializeObject<List<BidderTenderDocumentsVM>>(docsResult);
            if (docs == null) return new JsonResult(new List<object>());

            var currentUid = User?.Identity?.Name;
            var filteredDocs = docs
                .Where(d => string.Equals(d.TenderNo?.Trim(), tenderNo.Trim(), StringComparison.OrdinalIgnoreCase) &&
                            (string.IsNullOrEmpty(currentUid) || string.Equals(d.CreatedBy?.Trim(), currentUid.Trim(), StringComparison.OrdinalIgnoreCase)))
                .OrderByDescending(d => d.Id)
                .Select(d => new
                {
                    d.Id,
                    d.DocTitle,
                    d.DocType,
                    DocName = d.Doc,
                    d.Remarks,
                    CreatedDate = d.CreatedDate.ToString("dd/MM/yyyy HH:mm")
                })
                .ToList();

            return new JsonResult(filteredDocs);
        }

        public async Task<IActionResult> OnPostDeleteDocumentAsync(int id)
        {
            if (id <= 0) return new JsonResult(new { success = false, message = "Invalid Document ID." });

            var result = await _httpClient.DeleteAsync("BidderTenderDocuments/Delete", true, id).ConfigureAwait(false);
            if (result != null && result != "unauthorized")
            {
                return new JsonResult(new { success = true, message = "Document deleted successfully!" });
            }

            return new JsonResult(new { success = false, message = "Failed to delete document." });
        }

        public async Task<IActionResult> OnPostUploadDocuments()
        {
            try
            {
                var tenderNo = Request.Form["TenderNo"].ToString();
                if (string.IsNullOrEmpty(tenderNo)) return new JsonResult(new { success = false, message = "Please select a Tender/Reference No." });

                var projectsResult = await _httpClient.GetAsync("BidderTenderUploads/GetByTenderNo", true, tenderNo).ConfigureAwait(false);
                var projectsVM = !string.IsNullOrEmpty(projectsResult) ? JsonConvert.DeserializeObject<BidderTenderUploadsVM>(projectsResult) : null;
                if (projectsVM == null) return new JsonResult(new { success = false, message = "Tender project details not found." });

                var docTitles = Request.Form["DocTitles"].ToList();
                var docTypes = Request.Form["DocTypes"].ToList();
                var remarksList = Request.Form["Remarks"].ToList();
                var files = Request.Form.Files;

                if (files == null || files.Count == 0) return new JsonResult(new { success = false, message = "Please upload at least one document file." });
                if (files.Count > 12) return new JsonResult(new { success = false, message = "Maximum 12 dynamic document rows allowed." });

                int savedCount = 0;
                for (int i = 0; i < files.Count; i++)
                {
                    var file = files[i];
                    string docTitle = (docTitles != null && i < docTitles.Count) ? docTitles[i] : file.FileName;
                    string docType = (docTypes != null && i < docTypes.Count) ? docTypes[i] : "Technical";
                    string remark = (remarksList != null && i < remarksList.Count && !string.IsNullOrWhiteSpace(remarksList[i])) ? remarksList[i].Trim() : "NA";

                    bool isPriceBid = string.Equals(docType, "Price Bid", StringComparison.OrdinalIgnoreCase);

                    var (isValid, errorMessage) = _fileService.ValidateTenderArchive(file, isPriceBid);
                    if (!isValid)
                    {
                        return new JsonResult(new { success = false, message = $"Row {i + 1} ({file.FileName}): {errorMessage}" });
                    }

                    string encryptedFileName = await _fileService.SaveEncryptionAsync(@"\BidderTenders\" + projectsVM.ProjectId + @"\" + projectsVM.TenderNo + @"\", file);

                    var docDto = new BidderTenderDocumentsDTO
                    {
                        TenderNo = tenderNo,
                        DocType = docType,
                        DocTitle = docTitle,
                        Remarks = remark,
                        Doc = encryptedFileName,
                        ProjectId = projectsVM.ProjectId
                    };

                    docDto = ModelAuditor<BidderTenderDocumentsDTO>.SetAudit(User.Identity.Name, "Create", HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "", docDto);
                    var result = await _httpClient.PostAsync("BidderTenderDocuments/Create", true, docDto).ConfigureAwait(false);
                    if (!string.IsNullOrEmpty(result)) savedCount++;
                }

                return new JsonResult(new { success = true, message = $"{savedCount} Document(s) Uploaded Successfully!" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "Error processing upload: " + ex.Message });
            }
        }
    }
}
