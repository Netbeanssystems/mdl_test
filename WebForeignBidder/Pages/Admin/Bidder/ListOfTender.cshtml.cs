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
                                    .Any(f => f.Trim().Equals(uid, StringComparison.OrdinalIgnoreCase)) &&
                                (!x.TenderStartDate.HasValue || x.TenderStartDate.Value <= DateTime.Now))
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
                            try
                            {
                                var encPart = cor.CorrigendumDoc.Split(".")[0].Replace("B_S", "\"").Replace("F_S", "/");
                                var decrypted = EnDeCryptor.DecryptStringAES(encPart);
                                cor.CorrigendumDocDecrypted = (decrypted == "keyError") ? cor.CorrigendumDoc : decrypted;
                            }
                            catch
                            {
                                cor.CorrigendumDocDecrypted = cor.CorrigendumDoc;
                            }
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
                item.LatestExtendedDate = latestExtendedDate;
            }
            return Page();
        }
        public async Task<IActionResult> OnPostDownloadGenFileAsync(string Id)
        {
            try
            {
                string decryptedText = EnDeCryptor.DecryptStringAES(Id);
                int originalId = int.Parse(decryptedText);
                var FormsResult = await _httpClient.GetAsync("BidderTenderUploads/Get", true, originalId).ConfigureAwait(false);
                GenUpload = !string.IsNullOrEmpty(FormsResult) ? JsonConvert.DeserializeObject<BidderTenderUploadsVM>(FormsResult) : null;
                if (GenUpload == null)
                {
                    return new JsonResult(new { success = false, message = "Tender record not found." });
                }

                // Check if multiple documents exist in TenderDocuments
                if (GenUpload.TenderDocuments != null && GenUpload.TenderDocuments.Count > 0)
                {
                    using (var zipStream = new MemoryStream())
                    {
                        using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
                        {
                            int docIndex = 1;
                            foreach (var doc in GenUpload.TenderDocuments)
                            {
                                var docFileName = !string.IsNullOrEmpty(doc.EncryptedFileName) ? doc.EncryptedFileName : doc.OriginalFileName;
                                var entryName = !string.IsNullOrEmpty(doc.OriginalFileName) ? doc.OriginalFileName : docFileName;
                                if (string.IsNullOrEmpty(entryName)) entryName = $"Document_{docIndex}";

                                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "bidder", "BidderTenders", GenUpload.ProjectId ?? "", GenUpload.TenderNo ?? "", docFileName ?? "");

                                byte[] fileBytes = null;
                                if (System.IO.File.Exists(filePath))
                                {
                                    fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                                }
                                else
                                {
                                    // Try fetching via HTTP client if physical file path doesn't exist
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
                                }
                                docIndex++;
                            }
                        }
                        zipStream.Position = 0;
                        string zipFileName = $"Tender_Documents_{GenUpload.TenderNo.Replace("/", "_")}.zip";
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
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "bidder", "BidderTenders", GenUpload.ProjectId ?? "", GenUpload.TenderNo ?? "", docFileName ?? "");

                    byte[] fileBytes = null;
                    if (System.IO.File.Exists(filePath))
                    {
                        fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                    }
                    else
                    {
                        string currentDomain = $"{Request.Scheme}://{Request.Host}";
                        string fileUrl = $"{currentDomain}/bidder/BidderTenders/{GenUpload.ProjectId}/{GenUpload.TenderNo}/{docFileName}";
                        using (HttpClient client = new HttpClient())
                        {
                            fileBytes = await client.GetByteArrayAsync(fileUrl);
                        }
                    }

                    string filenameWithExtension = $"{GenUpload.TenderDoc}";
                    return new JsonResult(new { success = true, encdata = Convert.ToBase64String(fileBytes), filename = filenameWithExtension });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> OnPostDownloadBiddersCsvAsync(string Id)
        {
            try
            {
                string decryptedText = EnDeCryptor.DecryptStringAES(Id);
                int originalId = int.Parse(decryptedText);
                var tenderResult = await _httpClient.GetAsync("BidderTenderUploads/Get", true, originalId).ConfigureAwait(false);
                var tender = !string.IsNullOrEmpty(tenderResult) ? JsonConvert.DeserializeObject<BidderTenderUploadsVM>(tenderResult) : null;
                if (tender == null) return new JsonResult(new { success = false, message = "Tender not found" });

                var bidderUsernames = (tender.ForeignBidderId ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();

                var roleResult = await _httpClient.GetAsync("Users/GetByRole", true, "ForeignBidder").ConfigureAwait(false);
                var foreignBidders = !string.IsNullOrEmpty(roleResult) && roleResult.TrimStart().StartsWith("[")
                    ? JsonConvert.DeserializeObject<List<UserVM>>(roleResult) ?? new List<UserVM>()
                    : new List<UserVM>();

                var bidderRoleResult = await _httpClient.GetAsync("Users/GetByRole", true, "Bidder").ConfigureAwait(false);
                var normalBidders = !string.IsNullOrEmpty(bidderRoleResult) && bidderRoleResult.TrimStart().StartsWith("[")
                    ? JsonConvert.DeserializeObject<List<UserVM>>(bidderRoleResult) ?? new List<UserVM>()
                    : new List<UserVM>();

                foreignBidders.AddRange(normalBidders);
                var allUsers = foreignBidders.GroupBy(x => x.UserName).Select(g => g.First()).ToList();

                var selectedBidders = allUsers.Where(u => bidderUsernames.Contains(u.UserName, StringComparer.OrdinalIgnoreCase)).ToList();

                var sb = new System.Text.StringBuilder();
                sb.AppendLine($"\"Tender/Ref No :\",\"{tender.TenderNo}\"");
                sb.AppendLine($"\"Tender Description :\",\"{tender.TenderDescription}\"");
                sb.AppendLine($"\"Downloaded Time Stamp :\",\"{DateTime.Now:yyyy-MM-dd HH:mm:ss}\"");
                sb.AppendLine();
                sb.AppendLine("\"Sr No.\",\"Bidder Name\",\"Country\",\"Bidder Email ID\"");

                int srNo = 1;
                if (selectedBidders.Count > 0)
                {
                    foreach (var b in selectedBidders)
                    {
                        string name = !string.IsNullOrWhiteSpace(b.Name) ? b.Name : b.UserName;
                        string country = !string.IsNullOrWhiteSpace(b.Country) ? b.Country : "N/A";
                        string email = !string.IsNullOrWhiteSpace(b.Email) ? b.Email : "N/A";
                        sb.AppendLine($"\"{srNo++}\",\"{name.Replace("\"", "\"\"")}\",\"{country.Replace("\"", "\"\"")}\",\"{email.Replace("\"", "\"\"")}\"");
                    }
                }
                else
                {
                    foreach (var username in bidderUsernames)
                    {
                        sb.AppendLine($"\"{srNo++}\",\"{username.Replace("\"", "\"\"")}\",\"N/A\",\"N/A\"");
                    }
                }

                byte[] csvBytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
                string fileName = $"Selected_Bidders_{tender.TenderNo.Replace("/", "_")}.csv";

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
