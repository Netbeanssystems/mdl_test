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
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.IO.Compression;
using Microsoft.AspNetCore.Hosting;

namespace WebForeignBidder.Pages.Admin.Bidder
{
    [Authorize(Roles = "Bidder,ForeignBidder,SuperAdmin,CommercialExecutive,HOD")]


    public class ListOfTenderModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly INotyfService _notyf;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public ListOfTenderModel(
           IHttpClientService httpClient,
           INotyfService notyf,
           IWebHostEnvironment hostingEnvironment = null)
        {
            _httpClient = httpClient;
            _notyf = notyf;
            _hostingEnvironment = hostingEnvironment;
        }
        public string uid => User.Identity.Name;
        public bool IsAdminOrExecutive { get; set; }
        public List<BidderTenderUploadsVM> ModelVms { get; set; }
        public BidderTenderUploadsVM GenUpload { get; set; }
        [BindProperty] public BidderTenderUploadsDTO ModelDto { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            IsAdminOrExecutive = User.IsInRole("SuperAdmin") || User.IsInRole("BidderSuperAdmin") || User.IsInRole("CommercialExecutive") || User.IsInRole("HOD")
                || (!string.IsNullOrEmpty(uid) && (uid.Contains("CommercialExecutive", StringComparison.OrdinalIgnoreCase) || uid.Contains("SuperAdmin", StringComparison.OrdinalIgnoreCase) || uid.Contains("HOD", StringComparison.OrdinalIgnoreCase)));
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
            if (IsAdminOrExecutive)
            {
                ModelVms = allRecords;
            }
            else
            {
                // Always show tenders allowed for this bidder, even after passing closing date or extended date
                ModelVms = allRecords
                    .Where(x => !string.IsNullOrEmpty(x.ForeignBidderId) &&
                                x.ForeignBidderId.Split(',', StringSplitOptions.RemoveEmptyEntries)
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
                if (!string.IsNullOrEmpty(item.TenderDoc))
                {
                    try
                    {
                        var encPart = item.TenderDoc.Split(".")[0].Replace("B_S", "\"").Replace("F_S", "/");
                        var decrypted = EnDeCryptor.DecryptStringAES(encPart);
                        item.TenderDocDecrypted = (decrypted == "keyError") ? item.TenderDoc : decrypted;
                    }
                    catch
                    {
                        item.TenderDocDecrypted = item.TenderDoc;
                    }
                }

                DateTime? latestExtendedDate = null;
                if (item.TenderCorrigendums != null)
                {
                    foreach (var cor in item.TenderCorrigendums)
                    {
                        if (!string.IsNullOrEmpty(cor.CorrigendumDoc))
                        {
                            if (!string.IsNullOrEmpty(cor.OriginalFileName))
                            {
                                cor.CorrigendumDocDecrypted = cor.OriginalFileName;
                            }
                            else
                            {
                                var parts = cor.CorrigendumDoc.Split(',', StringSplitOptions.RemoveEmptyEntries);
                                var decryptedParts = new List<string>();
                                foreach (var p in parts)
                                {
                                    try
                                    {
                                        var encPart = p.Trim().Split(".")[0].Replace("B_S", "\"").Replace("F_S", "/");
                                        var decrypted = EnDeCryptor.DecryptStringAES(encPart);
                                        decryptedParts.Add((decrypted == "keyError") ? p.Trim() : decrypted);
                                    }
                                    catch
                                    {
                                        decryptedParts.Add(p.Trim());
                                    }
                                }
                                cor.CorrigendumDocDecrypted = string.Join(",", decryptedParts);
                            }
                        }
                        if (cor.ExtendedDate.HasValue)
                        {
                            if (!latestExtendedDate.HasValue || cor.ExtendedDate > latestExtendedDate)
                            {
                                latestExtendedDate = cor.ExtendedDate;
                            }
                        }
                    }
                }
                item.LatestExtendedDate = latestExtendedDate;
            }
            return Page();
        }

        private static bool IsTenderNotExpired(BidderTenderUploadsVM x, DateTime now)
        {
            if (x == null) return false;

            DateTime? latestExtendedDate = x.TenderCorrigendums?
                .Where(c => c.ExtendedDate.HasValue)
                .Select(c => c.ExtendedDate.Value)
                .DefaultIfEmpty()
                .Max();

            DateTime effectiveClosingDate = (latestExtendedDate.HasValue && latestExtendedDate.Value != default)
                ? latestExtendedDate.Value
                : x.TenderClosingDate;

            return effectiveClosingDate >= now;
        }

        public IActionResult OnGetDownloadDocument(string tenderNo, string projectId, string fileName, string displayName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                _notyf.Error("File name is missing.");
                return RedirectToPage();
            }

            string cleanFileName = Path.GetFileName(fileName);
            string cleanDisplayName = !string.IsNullOrWhiteSpace(displayName) ? Path.GetFileName(displayName) : cleanFileName;

            string filePath = FindPhysicalFile(projectId, tenderNo, cleanFileName);

            if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
            {
                var ext = Path.GetExtension(cleanFileName).ToLowerInvariant();
                var mimeType = ext switch
                {
                    ".pdf" => "application/pdf",
                    ".zip" => "application/zip",
                    ".rar" => "application/x-rar-compressed",
                    ".doc" or ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    ".xls" or ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    _ => "application/octet-stream"
                };

                return PhysicalFile(filePath, mimeType, cleanDisplayName);
            }

            _notyf.Error("File not found on server.");
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetDownloadTenderZipAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    _notyf.Error("Invalid tender reference.");
                    return RedirectToPage();
                }

                string decryptedText = EnDeCryptor.DecryptStringAES(id);
                if (!int.TryParse(decryptedText, out int originalId))
                {
                    _notyf.Error("Invalid tender ID.");
                    return RedirectToPage();
                }

                var formsResult = await _httpClient.GetAsync("BidderTenderUploads/Get", true, originalId).ConfigureAwait(false);
                if (string.IsNullOrEmpty(formsResult) || !formsResult.TrimStart().StartsWith("{"))
                {
                    formsResult = await _httpClient.GetAsync("BidderTenderUploads/Get", false, originalId).ConfigureAwait(false);
                }

                var tender = (!string.IsNullOrEmpty(formsResult) && formsResult.TrimStart().StartsWith("{"))
                    ? JsonConvert.DeserializeObject<BidderTenderUploadsVM>(formsResult)
                    : null;

                if (tender == null)
                {
                    _notyf.Error("Tender details not found.");
                    return RedirectToPage();
                }

                using (var zipStream = new MemoryStream())
                {
                    using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
                    {
                        if (tender.TenderDocuments != null && tender.TenderDocuments.Count > 0)
                        {
                            int docIndex = 1;
                            foreach (var doc in tender.TenderDocuments)
                            {
                                var docFileName = !string.IsNullOrEmpty(doc.EncryptedFileName) ? doc.EncryptedFileName : doc.OriginalFileName;
                                var entryName = !string.IsNullOrEmpty(doc.OriginalFileName) ? doc.OriginalFileName : docFileName;
                                if (string.IsNullOrEmpty(entryName)) entryName = $"Document_{docIndex}";

                                string filePath = FindPhysicalFile(tender.ProjectId, tender.TenderNo, docFileName);
                                byte[] fileBytes = null;

                                if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
                                {
                                    fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                                }

                                if (fileBytes != null && fileBytes.Length > 0)
                                {
                                    var zipEntry = archive.CreateEntry(entryName, CompressionLevel.Optimal);
                                    using (var entryStream = zipEntry.Open())
                                    {
                                        await entryStream.WriteAsync(fileBytes, 0, fileBytes.Length);
                                    }
                                }
                                docIndex++;
                            }
                        }
                        else if (!string.IsNullOrEmpty(tender.TenderDoc))
                        {
                            string docFileName = tender.TenderDoc;
                            string entryName = docFileName;
                            string filePath = FindPhysicalFile(tender.ProjectId, tender.TenderNo, docFileName);
                            byte[] fileBytes = null;

                            if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
                            {
                                fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                            }

                            if (fileBytes != null && fileBytes.Length > 0)
                            {
                                var zipEntry = archive.CreateEntry(entryName, CompressionLevel.Optimal);
                                using (var entryStream = zipEntry.Open())
                                {
                                    await entryStream.WriteAsync(fileBytes, 0, fileBytes.Length);
                                }
                            }
                        }
                    }

                    zipStream.Position = 0;
                    string safeTenderNo = (tender.TenderNo ?? "Tender").Replace("/", "_").Replace("\\", "_");
                    string zipFileName = $"Tender_Documents_{safeTenderNo}.zip";
                    return File(zipStream.ToArray(), "application/zip", zipFileName);
                }
            }
            catch (Exception ex)
            {
                _notyf.Error("Error generating zip: " + ex.Message);
                return RedirectToPage();
            }
        }

        private string FindPhysicalFile(string projectId, string tenderNo, string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return null;

            string cleanFileName = Path.GetFileName(fileName);
            var candidateFileNames = new List<string> { cleanFileName };
            if (cleanFileName.Contains(" "))
            {
                candidateFileNames.Add(cleanFileName.Replace(" ", "+"));
            }
            if (cleanFileName.Contains("+"))
            {
                candidateFileNames.Add(cleanFileName.Replace("+", " "));
            }

            var baseDirs = new List<string>();
            if (!string.IsNullOrEmpty(_hostingEnvironment?.WebRootPath) && Directory.Exists(_hostingEnvironment.WebRootPath))
            {
                baseDirs.Add(_hostingEnvironment.WebRootPath);
            }
            var curWwwroot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            if (Directory.Exists(curWwwroot) && !baseDirs.Contains(curWwwroot))
            {
                baseDirs.Add(curWwwroot);
            }
            var subWwwroot = Path.Combine(Directory.GetCurrentDirectory(), "WebForeignBidder", "wwwroot");
            if (Directory.Exists(subWwwroot) && !baseDirs.Contains(subWwwroot))
            {
                baseDirs.Add(subWwwroot);
            }
            var appBaseWwwroot = Path.Combine(AppContext.BaseDirectory, "wwwroot");
            if (Directory.Exists(appBaseWwwroot) && !baseDirs.Contains(appBaseWwwroot))
            {
                baseDirs.Add(appBaseWwwroot);
            }

            var projectList = (projectId ?? "").Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToList();
            if (!string.IsNullOrWhiteSpace(projectId) && !projectList.Contains(projectId.Trim()))
            {
                projectList.Add(projectId.Trim());
            }
            if (!projectList.Any())
            {
                projectList.Add("");
            }

            foreach (var baseDir in baseDirs)
            {
                var candidateDirs = new List<string>();
                foreach (var proj in projectList)
                {
                    if (!string.IsNullOrEmpty(proj))
                    {
                        candidateDirs.Add(Path.Combine(baseDir, "BidderTenders", proj, tenderNo ?? ""));
                        candidateDirs.Add(Path.Combine(baseDir, "bidder", "BidderTenders", proj, tenderNo ?? ""));
                    }
                }
                candidateDirs.Add(Path.Combine(baseDir, "BidderTenders", tenderNo ?? ""));
                candidateDirs.Add(Path.Combine(baseDir, "bidder", "BidderTenders", tenderNo ?? ""));
                candidateDirs.Add(Path.Combine(baseDir, "Bidder", "img", "UploadedFiles", "GeneralUpload"));

                foreach (var fn in candidateFileNames)
                {
                    foreach (var dir in candidateDirs)
                    {
                        var testPath = Path.Combine(dir, fn);
                        if (System.IO.File.Exists(testPath))
                        {
                            return testPath;
                        }
                    }

                    // Fallback search in wwwroot/BidderTenders recursively
                    try
                    {
                        var tendersDir = Path.Combine(baseDir, "BidderTenders");
                        if (Directory.Exists(tendersDir))
                        {
                            var foundFiles = Directory.GetFiles(tendersDir, fn, SearchOption.AllDirectories);
                            if (foundFiles.Length > 0)
                            {
                                return foundFiles[0];
                            }
                        }
                    }
                    catch { }

                    try
                    {
                        var bidderTendersDir = Path.Combine(baseDir, "bidder", "BidderTenders");
                        if (Directory.Exists(bidderTendersDir))
                        {
                            var foundFiles = Directory.GetFiles(bidderTendersDir, fn, SearchOption.AllDirectories);
                            if (foundFiles.Length > 0)
                            {
                                return foundFiles[0];
                            }
                        }
                    }
                    catch { }
                }
            }

            return null;
        }

        public async Task<IActionResult> OnPostDownloadFileAjaxAsync(string tenderNo, string projectId, string fileName, string displayName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    return new JsonResult(new { success = false, message = "File name is missing." });
                }

                string cleanFileName = Path.GetFileName(fileName);
                string cleanDisplayName = !string.IsNullOrWhiteSpace(displayName) ? Path.GetFileName(displayName) : cleanFileName;

                // Ensure file extension is preserved on cleanDisplayName
                string fileExt = Path.GetExtension(cleanFileName);
                if (!string.IsNullOrEmpty(fileExt) && !cleanDisplayName.EndsWith(fileExt, StringComparison.OrdinalIgnoreCase))
                {
                    cleanDisplayName = $"{cleanDisplayName}{fileExt}";
                }

                byte[] fileBytes = null;
                string filePath = FindPhysicalFile(projectId, tenderNo, cleanFileName);

                if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
                {
                    fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                }

                // Fallback to HTTP download from current domain if physical file not found directly on disk
                if (fileBytes == null || fileBytes.Length == 0)
                {
                    var projectList = (projectId ?? "").Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToList();
                    if (!projectList.Any()) projectList.Add(projectId ?? "");

                    string currentDomain = $"{Request.Scheme}://{Request.Host}";
                    using (HttpClient client = new HttpClient())
                    {
                        foreach (var proj in projectList)
                        {
                            var candidateUrls = new[]
                            {
                                $"{currentDomain}/BidderTenders/{proj}/{tenderNo}/{cleanFileName}",
                                $"{currentDomain}/bidder/BidderTenders/{proj}/{tenderNo}/{cleanFileName}",
                                $"{currentDomain}/BidderTenders/{tenderNo}/{cleanFileName}",
                                $"{currentDomain}/bidder/BidderTenders/{tenderNo}/{cleanFileName}",
                                $"{currentDomain}/Bidder/img/UploadedFiles/GeneralUpload/{cleanFileName}"
                            };

                            foreach (var url in candidateUrls)
                            {
                                try
                                {
                                    fileBytes = await client.GetByteArrayAsync(url);
                                    if (fileBytes != null && fileBytes.Length > 0)
                                    {
                                        break;
                                    }
                                }
                                catch { }
                            }
                            if (fileBytes != null && fileBytes.Length > 0) break;
                        }
                    }
                }

                if (fileBytes == null || fileBytes.Length == 0)
                {
                    return new JsonResult(new { success = false, message = $"Document '{cleanDisplayName}' could not be found on server." });
                }

                return new JsonResult(new
                {
                    success = true,
                    encdata = Convert.ToBase64String(fileBytes),
                    filename = cleanDisplayName
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "Error loading document: " + ex.Message });
            }
        }

        public async Task<IActionResult> OnPostDownloadGenFileAsync(string Id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Id))
                {
                    return new JsonResult(new { success = false, message = "Invalid tender reference." });
                }

                string decryptedText = EnDeCryptor.DecryptStringAES(Id);
                if (!int.TryParse(decryptedText, out int originalId))
                {
                    return new JsonResult(new { success = false, message = "Invalid tender ID." });
                }

                var FormsResult = await _httpClient.GetAsync("BidderTenderUploads/Get", true, originalId).ConfigureAwait(false);
                if (string.IsNullOrEmpty(FormsResult) || !FormsResult.TrimStart().StartsWith("{"))
                {
                    FormsResult = await _httpClient.GetAsync("BidderTenderUploads/Get", false, originalId).ConfigureAwait(false);
                }
                GenUpload = (!string.IsNullOrEmpty(FormsResult) && FormsResult.TrimStart().StartsWith("{"))
                    ? JsonConvert.DeserializeObject<BidderTenderUploadsVM>(FormsResult)
                    : null;
                if (GenUpload == null)
                {
                    return new JsonResult(new { success = false, message = "Tender record not found." });
                }

                // Check if multiple documents exist in TenderDocuments
                if (GenUpload.TenderDocuments != null && GenUpload.TenderDocuments.Count > 0)
                {
                    using (var zipStream = new MemoryStream())
                    {
                        int addedCount = 0;
                        using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
                        {
                            int docIndex = 1;
                            foreach (var doc in GenUpload.TenderDocuments)
                            {
                                var docFileName = !string.IsNullOrEmpty(doc.EncryptedFileName) ? doc.EncryptedFileName : doc.OriginalFileName;
                                var entryName = !string.IsNullOrEmpty(doc.OriginalFileName) ? doc.OriginalFileName : docFileName;
                                if (string.IsNullOrEmpty(entryName)) entryName = $"Document_{docIndex}";

                                string filePath = FindPhysicalFile(GenUpload.ProjectId, GenUpload.TenderNo, docFileName);

                                byte[] fileBytes = null;
                                if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
                                {
                                    fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                                }
                                else
                                {
                                    string currentDomain = $"{Request.Scheme}://{Request.Host}";
                                    string fileUrl = $"{currentDomain}/bidder/BidderTenders/{GenUpload.ProjectId}/{GenUpload.TenderNo}/{docFileName}";
                                    using (HttpClient client = new HttpClient())
                                    {
                                        try
                                        {
                                            fileBytes = await client.GetByteArrayAsync(fileUrl);
                                        }
                                        catch { }
                                    }
                                }

                                if (fileBytes != null && fileBytes.Length > 0)
                                {
                                    var zipEntry = archive.CreateEntry(entryName, CompressionLevel.Optimal);
                                    using (var entryStream = zipEntry.Open())
                                    {
                                        await entryStream.WriteAsync(fileBytes, 0, fileBytes.Length);
                                    }
                                    addedCount++;
                                }
                                docIndex++;
                            }
                        }

                        if (addedCount == 0)
                        {
                            return new JsonResult(new { success = false, message = "No tender documents could be found on the server to create ZIP." });
                        }

                        zipStream.Position = 0;
                        string safeTenderNo = (GenUpload.TenderNo ?? "Tender").Replace("/", "_").Replace("\\", "_");
                        string zipFileName = $"Tender_Documents_{safeTenderNo}.zip";
                        return new JsonResult(new
                        {
                            success = true,
                            encdata = Convert.ToBase64String(zipStream.ToArray()),
                            filename = zipFileName
                        });
                    }
                }
                else
                {
                    // Fallback to legacy single TenderDoc
                    string docFileName = GenUpload.TenderDoc;
                    if (string.IsNullOrEmpty(docFileName))
                    {
                        return new JsonResult(new { success = false, message = "No document attached to this tender." });
                    }

                    string filePath = FindPhysicalFile(GenUpload.ProjectId, GenUpload.TenderNo, docFileName);

                    byte[] fileBytes = null;
                    if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
                    {
                        fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                    }
                    else
                    {
                        string currentDomain = $"{Request.Scheme}://{Request.Host}";
                        string fileUrl = $"{currentDomain}/bidder/BidderTenders/{GenUpload.ProjectId}/{GenUpload.TenderNo}/{docFileName}";
                        using (HttpClient client = new HttpClient())
                        {
                            try
                            {
                                fileBytes = await client.GetByteArrayAsync(fileUrl);
                            }
                            catch { }
                        }
                    }

                    if (fileBytes == null || fileBytes.Length == 0)
                    {
                        return new JsonResult(new { success = false, message = "File could not be found on server." });
                    }

                    string friendlyName = docFileName;
                    try
                    {
                        var encPart = docFileName.Split(".")[0].Replace("B_S", "\"").Replace("F_S", "/");
                        var dec = EnDeCryptor.DecryptStringAES(encPart);
                        if (dec != "keyError" && !string.IsNullOrWhiteSpace(dec))
                        {
                            friendlyName = dec;
                            string ext = Path.GetExtension(docFileName);
                            if (!string.IsNullOrEmpty(ext) && !friendlyName.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
                            {
                                friendlyName = $"{friendlyName}{ext}";
                            }
                        }
                    }
                    catch { }

                    return new JsonResult(new { success = true, encdata = Convert.ToBase64String(fileBytes), filename = friendlyName });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> OnPostDownloadBiddersCsvAsync(string Id, int? rawId, string tenderNo, string tenderDesc, string bidders)
        {
            try
            {
                int originalId = rawId ?? 0;
                if (originalId <= 0 && !string.IsNullOrWhiteSpace(Id))
                {
                    try
                    {
                        string cleanId = Id.Trim().Replace(" ", "+");
                        string decryptedText = EnDeCryptor.DecryptStringAES(cleanId);
                        int.TryParse(decryptedText, out originalId);
                    }
                    catch { }
                }

                BidderTenderUploadsVM tender = null;

                // 1. Try get tender by originalId directly from API
                if (originalId > 0)
                {
                    try
                    {
                        var tenderResult = await _httpClient.GetAsync("BidderTenderUploads/Get", true, originalId).ConfigureAwait(false);
                        if (!string.IsNullOrEmpty(tenderResult) && tenderResult.TrimStart().StartsWith("{"))
                        {
                            tender = JsonConvert.DeserializeObject<BidderTenderUploadsVM>(tenderResult);
                        }
                    }
                    catch { }
                }

                // 2. If not found by single ID, load all tenders list (the exact call that loaded the page) and find match
                if (tender == null)
                {
                    try
                    {
                        var allTendersRes = await _httpClient.GetAsync("BidderTenderUploads/Get", true).ConfigureAwait(false);
                        if (!string.IsNullOrEmpty(allTendersRes) && allTendersRes.TrimStart().StartsWith("["))
                        {
                            var allTenders = JsonConvert.DeserializeObject<List<BidderTenderUploadsVM>>(allTendersRes);
                            if (originalId > 0)
                            {
                                tender = allTenders?.FirstOrDefault(x => x.Id == originalId);
                            }
                            if (tender == null && !string.IsNullOrWhiteSpace(tenderNo))
                            {
                                tender = allTenders?.FirstOrDefault(x => string.Equals(x.TenderNo?.Trim(), tenderNo.Trim(), StringComparison.OrdinalIgnoreCase));
                            }
                        }
                    }
                    catch { }
                }

                // 3. If still null, try GetByTenderNo endpoint if available
                if (tender == null && !string.IsNullOrWhiteSpace(tenderNo))
                {
                    try
                    {
                        var singleRes = await _httpClient.GetAsync($"BidderTenderUploads/GetByTenderNo?tenderNo={tenderNo.Trim()}", true).ConfigureAwait(false);
                        if (!string.IsNullOrEmpty(singleRes) && singleRes.TrimStart().StartsWith("{"))
                        {
                            tender = JsonConvert.DeserializeObject<BidderTenderUploadsVM>(singleRes);
                        }
                    }
                    catch { }
                }

                // 4. Fallback using data passed from the row itself
                if (tender == null)
                {
                    if (!string.IsNullOrWhiteSpace(tenderNo) || !string.IsNullOrWhiteSpace(bidders))
                    {
                        tender = new BidderTenderUploadsVM
                        {
                            Id = originalId,
                            TenderNo = !string.IsNullOrWhiteSpace(tenderNo) ? tenderNo.Trim() : "Tender",
                            TenderDescription = tenderDesc ?? "",
                            ForeignBidderId = bidders ?? ""
                        };
                    }
                }

                if (tender == null)
                {
                    return new JsonResult(new { success = false, message = "Tender details not found." });
                }

                if (string.IsNullOrWhiteSpace(tender.ForeignBidderId) && !string.IsNullOrWhiteSpace(bidders))
                {
                    tender.ForeignBidderId = bidders;
                }

                var bidderUsernames = (tender.ForeignBidderId ?? "")
                    .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                var allUsers = new List<UserVM>();
                try
                {
                    var roleResult = await _httpClient.GetAsync("Users/GetByRole", true, "ForeignBidder").ConfigureAwait(false);
                    if (!string.IsNullOrEmpty(roleResult) && roleResult.TrimStart().StartsWith("["))
                    {
                        var list = JsonConvert.DeserializeObject<List<UserVM>>(roleResult);
                        if (list != null) allUsers.AddRange(list);
                    }
                }
                catch { }

                try
                {
                    var bidderRoleResult = await _httpClient.GetAsync("Users/GetByRole", true, "Bidder").ConfigureAwait(false);
                    if (!string.IsNullOrEmpty(bidderRoleResult) && bidderRoleResult.TrimStart().StartsWith("["))
                    {
                        var list = JsonConvert.DeserializeObject<List<UserVM>>(bidderRoleResult);
                        if (list != null) allUsers.AddRange(list);
                    }
                }
                catch { }

                if (!allUsers.Any())
                {
                    try
                    {
                        var usersRes = await _httpClient.GetAsync("Users/Get", true).ConfigureAwait(false);
                        if (!string.IsNullOrEmpty(usersRes) && usersRes.TrimStart().StartsWith("["))
                        {
                            var list = JsonConvert.DeserializeObject<List<UserVM>>(usersRes);
                            if (list != null) allUsers.AddRange(list);
                        }
                    }
                    catch { }
                }

                var userLookup = allUsers
                    .Where(u => !string.IsNullOrWhiteSpace(u.UserName))
                    .GroupBy(u => u.UserName.Trim(), StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

                var sb = new System.Text.StringBuilder();
                sb.AppendLine($"\"Tender/Ref No :\",\"{tender.TenderNo}\"");
                sb.AppendLine($"\"Tender Description :\",\"{tender.TenderDescription}\"");
                sb.AppendLine($"\"Downloaded Time Stamp :\",\"{DateTime.Now:yyyy-MM-dd HH:mm:ss}\"");
                sb.AppendLine();
                sb.AppendLine("\"Sr No.\",\"Bidder Name\",\"Country\",\"Bidder Email ID\"");

                int srNo = 1;
                if (bidderUsernames.Count > 0)
                {
                    foreach (var username in bidderUsernames)
                    {
                        userLookup.TryGetValue(username, out var b);
                        string name = (b != null && !string.IsNullOrWhiteSpace(b.Name)) ? b.Name : username;
                        string country = (b != null && !string.IsNullOrWhiteSpace(b.Country)) ? b.Country : "N/A";
                        string email = (b != null && !string.IsNullOrWhiteSpace(b.Email)) ? b.Email : "N/A";
                        sb.AppendLine($"\"{srNo++}\",\"{name.Replace("\"", "\"\"")}\",\"{country.Replace("\"", "\"\"")}\",\"{email.Replace("\"", "\"\"")}\"");
                    }
                }
                else
                {
                    sb.AppendLine("\"No bidders assigned to this tender\",\"\",\"\",\"\"");
                }

                byte[] csvBytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
                string safeTenderNo = string.Join("_", (tender.TenderNo ?? "Tender").Split(Path.GetInvalidFileNameChars())).Replace("/", "_");
                string fileName = $"Selected_Bidders_{safeTenderNo}.csv";

                return new JsonResult(new
                {
                    success = true,
                    encdata = Convert.ToBase64String(csvBytes),
                    filename = fileName
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}
