using Application.Dtos;
using Application.Helpers;
using Application.ServiceInterfaces;
using Application.Services;
using Application.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebBank.Pages.Admin.Document
{
    [Authorize(Roles = "BankUser,SuperAdmin")]
    public class AddModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly IFileService _fileService;
        private readonly INotyfService _notyf;

        public AddModel(IHttpClientService httpClient, IFileService fileService, INotyfService notyf  )
        {
            _httpClient = httpClient;
            _fileService = fileService;
            _notyf = notyf;
        }

        [BindProperty] public DocumentsDTO DocumentsDTO { get; set; }
        public List<URLsTimingVM> AvailableUploadWindows { get; set; }
        public URLsTimingVM SelectedUploadWindow { get; set; }

        public string BranchName => User.Claims.FirstOrDefault(x => x.Type == "bnam")?.Value;
        public string BankName => User.Claims.FirstOrDefault(x => x.Type == "nam")?.Value;

        [BindProperty] public bool IsInTime { get; set; }

        #region PAGE LOAD

        public async Task<IActionResult> OnGet()
        {
            await LoadTimingAsync();
            return Page();
        }

        private async Task LoadTimingAsync()
        {
            var request = HttpContext.Request;
            var URL = $"{request.Scheme}://{request.Host}/bank/Admin/Document/Add";

            // Initialize DocumentsDTO if null
            if (DocumentsDTO == null)
            {
                DocumentsDTO = new DocumentsDTO();
            }

            // Get all active upload windows
            var result = await _httpClient.GetAsync("Documents/GetActiveURLsTiming", true);
            var allWindows = !string.IsNullOrEmpty(result)
                ? JsonConvert.DeserializeObject<List<URLsTimingVM>>(result)
                : null;

            // Filter windows for this specific URL
            var urlWindows = allWindows?.Where(x => x.Url == URL).ToList();

            if (urlWindows == null || !urlWindows.Any())
            {
                IsInTime = false;
                AvailableUploadWindows = new List<URLsTimingVM>();
                return;
            }

            // Find currently active windows (BankUser can see ALL OPEN windows in dropdown)
            var now = DateTime.Now;
            AvailableUploadWindows = urlWindows
                .Where(x => x.FromTime <= now && x.ToTime >= now)
                .ToList();

            if (AvailableUploadWindows == null || !AvailableUploadWindows.Any())
            {
                IsInTime = false;
                AvailableUploadWindows = new List<URLsTimingVM>();
                return;
            }

            IsInTime = true;
            
            // Set the first open window as selected by default
            SelectedUploadWindow = AvailableUploadWindows.FirstOrDefault();
            
            // Set the selected upload window ID for the document
            DocumentsDTO.URLsTimingId = SelectedUploadWindow.Id;
            DocumentsDTO.Description = SelectedUploadWindow.Description;
        }

        #endregion

        #region POST

        public async Task<IActionResult> OnPost()
        {
            await LoadTimingAsync();   // IMPORTANT: keep form visible

            try
            {
                // Validate upload window is still active
                if (!IsInTime || SelectedUploadWindow == null)
                {
                    _notyf.Error("Upload window is currently closed. You cannot upload documents at this time.");
                    return Page();
                }

                if (DocumentsDTO.DocumentFile != null)
                {
                    // STEP 1 — Validate securely (does NOT save file)
                    await ValidateFileAsync(DocumentsDTO.DocumentFile);

                    // STEP 2 — Use old encryption + save method (unchanged system)
                    DocumentsDTO.DocumentName = await _fileService.SaveEncryptionAsync(
                        @"\img\UploadedFiles\Documents\",
                        DocumentsDTO.DocumentFile
                    );

                    DocumentsDTO.DocumentFile = null;
                }

                // Set upload window reference
                DocumentsDTO.URLsTimingId = SelectedUploadWindow.Id;
                DocumentsDTO.Description = SelectedUploadWindow.Description;

                DocumentsDTO.BranchName = BranchName;
                DocumentsDTO.BankName = BankName;

                DocumentsDTO = ModelAuditor<DocumentsDTO>.SetAudit(
                    User.Identity.Name,
                    DocumentsDTO.Id == 0 ? "Create" : "Edit",
                    HttpContext.Connection.RemoteIpAddress.ToString(),
                    DocumentsDTO);

                var Result = await _httpClient.PostAsync("Documents/Create", true, DocumentsDTO);

                if (Result == "unauthorized")
                {
                    _notyf.Information("Please login/register");
                    return RedirectToPage("/Account/Login");
                }

                var TempDTO = !string.IsNullOrEmpty(Result)
                    ? JsonConvert.DeserializeObject<DocumentsDTO>(Result)
                    : null;

                if (TempDTO == null)
                    _notyf.Error("Save failed");
                else
                    _notyf.Success("Saved successfully");

                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                _notyf.Error(ex.Message);
                return Page();   // keep form visible
            }
        }

        #endregion


        private async Task ValidateFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("No file uploaded");

            string fileName = file.FileName;

            // ⭐ Block multiple dots (double extension attack)
            if (fileName.Count(c => c == '.') != 1)
                throw new Exception("Invalid file name");

            // ⭐ Block special characters
            if (fileName.Contains(";") || fileName.Contains("%") ||
                fileName.Contains("\\") || fileName.Contains("/"))
                throw new Exception("Invalid file name");

            // ⭐ Extension check
            string ext = Path.GetExtension(fileName).ToLower();

            if (ext != ".pdf")
                throw new Exception("Only PDF files are allowed");

            // ⭐ Size check
            if (file.Length > 2 * 1024 * 1024)
                throw new Exception("File size must be less than 2 MB");

            // ⭐ Validate PDF signature
            using var stream = file.OpenReadStream();

            byte[] header = new byte[5];
            await stream.ReadAsync(header, 0, header.Length);
            stream.Position = 0;

            string pdfHeader = System.Text.Encoding.ASCII.GetString(header);

            if (pdfHeader != "%PDF-")
                throw new Exception("Invalid PDF file");

            // ⭐ Scan for malicious JavaScript
            using var reader = new StreamReader(stream);
            string content = await reader.ReadToEndAsync();

            string[] dangerous =
            {
                "/JavaScript", "/JS", "/OpenAction", "/AA", "/Launch", "/SubmitForm"
            };

            if (dangerous.Any(k => content.Contains(k, StringComparison.OrdinalIgnoreCase)))
                throw new Exception("Malicious PDF detected");

            // ⭐ PASSWORD PROTECTION CHECK
            // Check if PDF is password protected by looking for encryption markers
            stream.Position = 0;
            using var pdfReader = new StreamReader(stream, System.Text.Encoding.ASCII, true, 8192, true);
            string fullContent = await pdfReader.ReadToEndAsync();

            // Check for PDF encryption dictionary
            bool isPasswordProtected = fullContent.Contains("/Encrypt") || 
                                       fullContent.Contains("/Standard") || 
                                       fullContent.Contains("/CFM");

            if (!isPasswordProtected)
            {
                throw new Exception("PDF file must be password protected. Please encrypt your PDF with a password before uploading.");
            }
        }


    }
}
