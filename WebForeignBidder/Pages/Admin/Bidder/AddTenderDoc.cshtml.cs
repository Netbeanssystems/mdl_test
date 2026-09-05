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

        public async Task<IActionResult> OnPostUploadDocuments()
        {
            BidderTenderDocumentsDTO BidderTenderDocumentsDTOs = JsonConvert.DeserializeObject<BidderTenderDocumentsDTO>(Request.Form["model"].ToString());

            var Projects = await _httpClient.GetAsync("BidderTenderUploads/GetByTenderNo", true, BidderTenderDocumentsDTOs.TenderNo).ConfigureAwait(false);
            var ProjectsVM = !string.IsNullOrEmpty(Projects) ? JsonConvert.DeserializeObject<BidderTenderUploadsVM>(Projects) : null;
            if(ProjectsVM == null) return new JsonResult("0");

            var files = Request.Form.Files;
            //...........Check Valid File ...................
            foreach (var file in files)
            {
                if (!_fileService.CheckValidFile(file))
                {
                    return new JsonResult("Please upload a valid file.");
                }

                if (string.Equals(BidderTenderDocumentsDTOs.DocType, "Price Bid", StringComparison.OrdinalIgnoreCase))
                {
                    var ext = System.IO.Path.GetExtension(file.FileName)?.ToLowerInvariant();
                    if (ext != ".pdf")
                    {
                        return new JsonResult("Price Bid document must be a PDF file.");
                    }

                    byte[] buffer = new byte[Math.Min(file.Length, 100 * 1024)];
                    using (var stream = file.OpenReadStream())
                    {
                        stream.Read(buffer, 0, buffer.Length);
                    }
                    string headerContent = System.Text.Encoding.UTF8.GetString(buffer);
                    if (!headerContent.Contains("/Encrypt"))
                    {
                        return new JsonResult("Price Bid PDF file must be password protected.");
                    }
                }

                BidderTenderDocumentsDTOs.Doc = await _fileService.SaveEncryptionAsync(@"\BidderTenders\" + ProjectsVM.ProjectId + @"\" + ProjectsVM.TenderNo + @"\", file);
                BidderTenderDocumentsDTOs.ProjectId = ProjectsVM.ProjectId;
            }
            BidderTenderDocumentsDTOs = ModelAuditor<BidderTenderDocumentsDTO>.SetAudit(User.Identity.Name, "Create", HttpContext.Connection.RemoteIpAddress.ToString(), BidderTenderDocumentsDTOs);
            var result = await _httpClient.PostAsync("BidderTenderDocuments/Create", true, BidderTenderDocumentsDTOs).ConfigureAwait(false);
            if (result != null)
            {
                _TenderDocVM = !string.IsNullOrEmpty(result) ? JsonConvert.DeserializeObject<BidderTenderDocumentsVM>(result) : null;
                return new JsonResult("1");
            }

            var uid = User.Claims.FirstOrDefault(x => x.Type == "uid")?.Value;

            return new JsonResult("0");
        }
    }
}
