using Application.Dtos;
using Application.Helpers;
using Application.ServiceInterfaces;
using Application.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebForeignBidder.Helpers;

namespace WebForeignBidder.Pages.Admin.TenderUpload
{
    [Authorize(Roles = "BidderSuperAdmin,CommercialExecutive")]
    public class ManageModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly INotyfService _notyf;
        private readonly IFileService _fileService;
        public ManageModel(
            IHttpClientService httpClient,
            IEmailService emailService,
            IConfiguration config,
            IHttpContextAccessor httpContextAccessor,
            IFileService fileService,
            INotyfService notyf)
        {
            _httpClient = httpClient;
            _emailService = emailService;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
            _fileService = fileService;
            _notyf = notyf;
        }
        public List<int> ForeignBidderId { get; set; } = new List<int>();
        public string UserProjectIds => User.Claims.FirstOrDefault(x => x.Type == "proj")?.Value;

        // If you specifically want the final comma-separated string
        //public string ForeignBidderId { get; set; }
        [BindProperty] public BidderTenderUploadsDTO tenderUpload { get; set; }
        public string uid => DataHelper.GetUserId(User);
        [BindProperty] public UserProfileDTO UserProfileDto { get; set; }
        // [BindProperty]
        public bool IsNew { get; set; } = false;
        //public async Task<IActionResult> OnGetAsync()
        //{
        //    //get all the projects for dropdown
        //    var projectsResult = await _httpClient.GetAsync("Projects/Get", true).ConfigureAwait(false);
        //    if (projectsResult == "unauthorized") return RedirectToPage("/Account/Login");
        //    var projects = !string.IsNullOrEmpty(projectsResult) ? JsonConvert.DeserializeObject<List<BidderProjectsDTO>>(projectsResult) : null;
        //    if (projects == null || projects.Count <= 0)
        //    {
        //        _notyf.Error("Record not found");
        //        return Page();
        //    }
        //    var userProjectIdsList = UserProjectIds.Split(',').Select(id => int.Parse(id.Trim())).ToList();
        //    projects = projects.Where(p => userProjectIdsList.Contains(p.Id)).ToList();
        //    ViewData["Projects"] = new SelectList(projects, "Id", "ProjectName");

        //    var role = "ForeignBidder";
        //    var ForiegnbidderResult = await _httpClient.GetAsync("Users/GetByRole", true, role).ConfigureAwait(false);
        //    if (ForiegnbidderResult == "unauthorized") return RedirectToPage("/Account/Login");
        //    var ForeignBidder = !string.IsNullOrEmpty(ForiegnbidderResult)
        //        ? JsonConvert.DeserializeObject<List<UserRoleVM>>(ForiegnbidderResult)
        //        : null;
        //    if (ForeignBidder == null || ForeignBidder.Count <= 0)
        //    {
        //        _notyf.Error("Foreign Bidder not found");
        //        return Page();
        //    }
        //    ViewData["ForeignBidder"] = new SelectList(ForeignBidder, "UserName", "UserName");

        //    return Page();
        //}
        public async Task<IActionResult> OnGet()
        {
            var result = await _httpClient.GetAsync("Users/UserById", true, uid).ConfigureAwait(false);
            if (result == "unauthorized") return RedirectToPage("/account/login");
            UserProfileDto = !string.IsNullOrEmpty(result) ? JsonConvert.DeserializeObject<UserProfileDTO>(result) : null;
            if (User.IsInRole("CommercialExecutive") || User.IsInRole("BidderSuperAdmin"))
            {
                var ids = UserProfileDto.ProjectId;
                //get all the projects for dropdown
                var projectsResult = await _httpClient.GetAsync($"Projects/GetMultiple/{ids}", true).ConfigureAwait(false);
                // var projectsResult = await _httpClient.GetAsync("Projects/Get/{id}", true).ConfigureAwait(false);
                if (projectsResult == "unauthorized") return RedirectToPage("/Account/Login");
                var projects = !string.IsNullOrEmpty(projectsResult) ? JsonConvert.DeserializeObject<List<BidderProjectsDTO>>(projectsResult) : null;
                if (projects == null || projects.Count <= 0)
                {
                    _notyf.Error("Record not found");
                    return Page();
                }

                ViewData["Projects"] = new SelectList(projects, "Id", "ProjectName");

                var id = UserProfileDto.YardId;
                var yardsResult = await _httpClient.GetAsync($"Projects/GetYardsByProjectIds/{id}", true).ConfigureAwait(false);

                if (yardsResult == "unauthorized")
                    return RedirectToPage("/Account/Login");
                var yards = !string.IsNullOrEmpty(yardsResult) ? JsonConvert.DeserializeObject<List<BidderYardsDTO>>(yardsResult) : null;
                if (yards == null || yards.Count <= 0)
                {
                    _notyf.Error("Record not found");
                    return Page();
                }
                ViewData["Yards"] = new SelectList(yards, "Id", "YardNumber");

                var role = "ForeignBidder";
                var ForiegnbidderResult = await _httpClient.GetAsync("Users/GetByRole", true, role).ConfigureAwait(false);
                if (ForiegnbidderResult == "unauthorized") return RedirectToPage("/Account/Login");
                var ForeignBidder = new List<UserVM>();
                if (!string.IsNullOrEmpty(ForiegnbidderResult) && ForiegnbidderResult.TrimStart().StartsWith("["))
                {
                    try
                    {
                        ForeignBidder = JsonConvert.DeserializeObject<List<UserVM>>(ForiegnbidderResult) ?? new List<UserVM>();
                    }
                    catch { }
                }

                var BidderResult = await _httpClient.GetAsync("Users/GetByRole", true, "Bidder").ConfigureAwait(false);
                if (BidderResult == "unauthorized") return RedirectToPage("/Account/Login");
                var BidderList = new List<UserVM>();
                if (!string.IsNullOrEmpty(BidderResult) && BidderResult.TrimStart().StartsWith("["))
                {
                    try
                    {
                        BidderList = JsonConvert.DeserializeObject<List<UserVM>>(BidderResult) ?? new List<UserVM>();
                    }
                    catch { }
                }

                if (BidderList != null && BidderList.Count > 0)
                {
                    ForeignBidder.AddRange(BidderList);
                }

                ForeignBidder = ForeignBidder.GroupBy(x => x.UserName).Select(g => g.First()).OrderBy(x => x.UserName).ToList();

                if (ForeignBidder == null || ForeignBidder.Count <= 0)
                {
                    _notyf.Error("Bidder not found");
                    return Page();
                }
                ViewData["ForeignBidder"] = new SelectList(ForeignBidder, "UserName", "UserName");

                //-----------------------
                var bidderOptions = ForeignBidder.Select(x => new
                {
                    Value = x.UserName,
                    Text = $"{(!string.IsNullOrWhiteSpace(x.Name) ? x.Name : x.UserName)} - {(!string.IsNullOrWhiteSpace(x.Country) ? x.Country : "N/A")} - {(!string.IsNullOrWhiteSpace(x.Email) ? x.Email : "N/A")}"
                }).ToList();

                ViewData["ForeignBidder"] = new SelectList(bidderOptions, "Value", "Text");
//---------------

            }
            if (UserProfileDto != null) return Page();
            _notyf.Error("User not found");
            return RedirectToPage("/Index");

        }
        public async Task<IActionResult> OnPost()
        {
            try
            {
                // Validate and save tender documents (dynamic rows)
                tenderUpload.TenderDocuments = new List<BidderTenderUploadDocumentsDTO>();

                if (tenderUpload.UploadDocFiles != null && tenderUpload.UploadDocFiles.Any())
                {
                    if (tenderUpload.UploadDocFiles.Count > 8)
                    {
                        _notyf.Error("Maximum 8 document rows allowed.");
                        return RedirectToPage();
                    }

                    for (int i = 0; i < tenderUpload.UploadDocFiles.Count; i++)
                    {
                        var file = tenderUpload.UploadDocFiles[i];
                        if (file != null && file.Length > 0)
                        {
                            var (isValid, errorMessage) = _fileService.ValidateTenderArchive(file);
                            if (!isValid)
                            {
                                _notyf.Error(errorMessage);
                                return RedirectToPage();
                            }

                            string docTitle = (tenderUpload.UploadDocNames != null && i < tenderUpload.UploadDocNames.Count && !string.IsNullOrWhiteSpace(tenderUpload.UploadDocNames[i]))
                                ? tenderUpload.UploadDocNames[i].Trim()
                                : file.FileName;

                            string encryptedName = "";
                            if (tenderUpload.ProjectIds != null && tenderUpload.ProjectIds.Any())
                            {
                                foreach (var item in tenderUpload.ProjectIds)
                                {
                                    encryptedName = await _fileService.SaveEncryptionAsync(
                                        $@"\BidderTenders\{item}\{tenderUpload.TenderNo}\",
                                        file);
                                }
                            }

                            tenderUpload.TenderDocuments.Add(new BidderTenderUploadDocumentsDTO
                            {
                                OriginalFileName = docTitle,
                                EncryptedFileName = encryptedName,
                                FileSizeInBytes = file.Length,
                                CreatedDate = DateTime.Now,
                                CreatedBy = User.Identity.Name,
                                IsActive = true
                            });
                        }
                    }
                }

                // Backward compatibility for single IFFTenderDoc if present
                if (tenderUpload.IFFTenderDoc != null)
                {
                    var (isValid, errorMessage) = _fileService.ValidateTenderArchive(tenderUpload.IFFTenderDoc);
                    if (!isValid)
                    {
                        _notyf.Error(errorMessage);
                        return RedirectToPage();
                    }

                    if (tenderUpload.ProjectIds != null && tenderUpload.ProjectIds.Any())
                    {
                        foreach (var item in tenderUpload.ProjectIds)
                        {
                            tenderUpload.TenderDoc = await _fileService.SaveEncryptionAsync(
                                $@"\BidderTenders\{item}\{tenderUpload.TenderNo}\",
                                tenderUpload.IFFTenderDoc);
                        }
                    }

                    tenderUpload.TenderDocuments.Add(new BidderTenderUploadDocumentsDTO
                    {
                        OriginalFileName = tenderUpload.IFFTenderDoc.FileName,
                        EncryptedFileName = tenderUpload.TenderDoc,
                        FileSizeInBytes = tenderUpload.IFFTenderDoc.Length,
                        CreatedDate = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        IsActive = true
                    });
                    tenderUpload.IFFTenderDoc = null;
                }
                else if (tenderUpload.TenderDocuments.Any())
                {
                    tenderUpload.TenderDoc = tenderUpload.TenderDocuments.First().EncryptedFileName;
                }

                // Validate and save corrigendum documents (dynamic rows)
                if (tenderUpload.UploadCorrigendumDocFiles != null && tenderUpload.UploadCorrigendumDocFiles.Any())
                {
                    if (tenderUpload.UploadCorrigendumDocFiles.Count > 8)
                    {
                        _notyf.Error("Maximum 8 corrigendum document rows allowed.");
                        return RedirectToPage();
                    }

                    if (tenderUpload.TenderCorrigendums == null)
                    {
                        tenderUpload.TenderCorrigendums = new BidderTenderCorrigendumDto();
                    }

                    List<string> savedCorrigendumDocs = new List<string>();
                    List<string> origCorrigendumNames = new List<string>();
                    long totalSizeBytes = 0;

                    for (int i = 0; i < tenderUpload.UploadCorrigendumDocFiles.Count; i++)
                    {
                        var file = tenderUpload.UploadCorrigendumDocFiles[i];
                        if (file != null && file.Length > 0)
                        {
                            var (isValid, errorMessage) = _fileService.ValidateTenderArchive(file);
                            if (!isValid)
                            {
                                _notyf.Error(errorMessage);
                                return RedirectToPage();
                            }

                            string docTitle = (tenderUpload.UploadCorrigendumDocNames != null && i < tenderUpload.UploadCorrigendumDocNames.Count && !string.IsNullOrWhiteSpace(tenderUpload.UploadCorrigendumDocNames[i]))
                                ? tenderUpload.UploadCorrigendumDocNames[i].Trim()
                                : file.FileName;

                            string encryptedName = "";
                            if (tenderUpload.ProjectIds != null && tenderUpload.ProjectIds.Any())
                            {
                                foreach (var item in tenderUpload.ProjectIds)
                                {
                                    encryptedName = await _fileService.SaveEncryptionAsync(
                                        $@"\BidderTenders\{item}\{tenderUpload.TenderNo}\",
                                        file);
                                }
                            }
                            if (!string.IsNullOrEmpty(encryptedName))
                            {
                                savedCorrigendumDocs.Add(encryptedName);
                                origCorrigendumNames.Add(docTitle);
                                totalSizeBytes += file.Length;
                            }
                        }
                    }

                    if (savedCorrigendumDocs.Any())
                    {
                        tenderUpload.TenderCorrigendums.HashedFileName = string.Join(",", savedCorrigendumDocs);
                        tenderUpload.TenderCorrigendums.OriginalFileName = string.Join(",", origCorrigendumNames);
                        tenderUpload.TenderCorrigendums.FileSizeInBytes = totalSizeBytes;

                        if (string.IsNullOrEmpty(tenderUpload.TenderCorrigendums.CorrigendumDoc))
                        {
                            tenderUpload.TenderCorrigendums.CorrigendumDoc = tenderUpload.TenderCorrigendums.HashedFileName;
                        }
                        else
                        {
                            tenderUpload.TenderCorrigendums.CorrigendumDoc += "," + tenderUpload.TenderCorrigendums.HashedFileName;
                        }
                    }
                }

                // Backward compatibility for single IFFCorrigendumDoc if present
                if (tenderUpload.TenderCorrigendums?.IFFCorrigendumDoc != null)
                {
                    var (isValid, errorMessage) = _fileService.ValidateTenderArchive(tenderUpload.TenderCorrigendums.IFFCorrigendumDoc);
                    if (!isValid)
                    {
                        _notyf.Error(errorMessage);
                        return RedirectToPage();
                    }

                    if (tenderUpload.ProjectIds != null && tenderUpload.ProjectIds.Any())
                    {
                        foreach (var item in tenderUpload.ProjectIds)
                        {
                            tenderUpload.TenderCorrigendums.CorrigendumDoc = await _fileService.SaveEncryptionAsync(
                                $@"\BidderTenders\{item}\{tenderUpload.TenderNo}\",
                                tenderUpload.TenderCorrigendums.IFFCorrigendumDoc
                            );
                        }
                    }

                    tenderUpload.TenderCorrigendums.IFFCorrigendumDoc = null;
                }

                // Convert lists to comma separated strings
                if (tenderUpload.ForeignBidderIds != null)
                    tenderUpload.ForeignBidderId = string.Join(",", tenderUpload.ForeignBidderIds);

                if (tenderUpload.YardIds != null)
                    tenderUpload.YardId = string.Join(",", tenderUpload.YardIds);

                if (tenderUpload.ProjectIds != null)
                    tenderUpload.ProjectId = string.Join(",", tenderUpload.ProjectIds);

                string clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();
                if (!string.IsNullOrEmpty(clientIp) && clientIp.Length > 16)
                {
                    clientIp = clientIp.Substring(0, 16);
                }

                // Audit fields
                if (tenderUpload.Id == 0) // New record
                {
                    tenderUpload.CreatedBy = User.Identity.Name;
                    tenderUpload.CreatedDate = DateTime.Now;
                    tenderUpload.IP = clientIp;
                }
                else // Update existing record
                {
                    if (string.IsNullOrWhiteSpace(tenderUpload.CreatedBy))
                    {
                        tenderUpload.CreatedBy = User.Identity.Name;
                    }
                    tenderUpload.ModifiedBy = User.Identity.Name;
                    tenderUpload.ModifiedDate = DateTime.Now;
                    tenderUpload.IP = clientIp;
                }

                if (tenderUpload.TenderCorrigendums != null)
                {
                    if (string.IsNullOrEmpty(tenderUpload.TenderCorrigendums.CorrigendumDescription) &&
                        string.IsNullOrEmpty(tenderUpload.TenderCorrigendums.CorrigendumDoc) &&
                        string.IsNullOrEmpty(tenderUpload.TenderCorrigendums.HashedFileName))
                    {
                        tenderUpload.TenderCorrigendums = null;
                    }
                    else
                    {
                        if (tenderUpload.TenderCorrigendums.Id == 0)
                        {
                            tenderUpload.TenderCorrigendums.CreatedBy = User.Identity.Name;
                            if (tenderUpload.TenderCorrigendums.CreatedDate < new DateTime(1753, 1, 1))
                                tenderUpload.TenderCorrigendums.CreatedDate = DateTime.Now;
                            tenderUpload.TenderCorrigendums.IP = clientIp;
                        }
                        else
                        {
                            tenderUpload.TenderCorrigendums.ModifiedBy = User.Identity.Name;
                            if (tenderUpload.TenderCorrigendums.CreatedDate < new DateTime(1753, 1, 1))
                                tenderUpload.TenderCorrigendums.CreatedDate = DateTime.Now;
                            tenderUpload.TenderCorrigendums.IP = clientIp;
                        }
                    }
                }

                // Set proper audit action
                string auditAction = tenderUpload.Id == 0 ? "Create" : "Edit";
                tenderUpload = ModelAuditor<BidderTenderUploadsDTO>.SetAudit(User.Identity.Name, auditAction, clientIp, tenderUpload);
                if (tenderUpload.TenderCorrigendums != null)
                {
                    tenderUpload.TenderCorrigendums = ModelAuditor<BidderTenderCorrigendumDto>.SetAudit(User.Identity.Name, auditAction, clientIp, tenderUpload.TenderCorrigendums);
                    tenderUpload.TenderCorrigendums.IFFCorrigendumDoc = null;
                }

                // Ensure file handles are cleared before JSON serialization to WebAPI
                tenderUpload.UploadDocFiles = null;
                tenderUpload.UploadDocNames = null;
                tenderUpload.IFFTenderDoc = null;
                tenderUpload.UploadCorrigendumDocFiles = null;
                tenderUpload.UploadCorrigendumDocNames = null;

                // Call API (POST for new, PUT for update)
                var Result = tenderUpload.Id == 0
                    ? await _httpClient.PostAsync("BidderTenderUploads/Create", true, tenderUpload)
                    : await _httpClient.PutAsync("BidderTenderUploads/Edit", true, tenderUpload.Id, tenderUpload);

                if (Result == "unauthorized")
                {
                    _notyf.Information("Please login/register");
                    return RedirectToPage("/Account/Login");
                }

                BidderTenderUploadsDTO TempDTO = null;
                try
                {
                    if (!string.IsNullOrEmpty(Result))
                    {
                        TempDTO = JsonConvert.DeserializeObject<BidderTenderUploadsDTO>(Result);
                    }
                }
                catch
                {
                    // Result was plain text error rather than JSON
                }

                if (TempDTO == null || TempDTO.Id == 0)
                {
                    string errorMsg = !string.IsNullOrEmpty(Result) ? Result : "Save failed: WebAPI did not return a valid result.";
                    _notyf.Error(errorMsg);
                    return RedirectToPage();
                }
                else
                {
                    _notyf.Success("Saved successfully");
                    return RedirectToPage();
                }
            }
            catch (Exception ex)
            {
                var detailedError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _notyf.Error($"Error: {detailedError}");
                return RedirectToPage();
            }
        }

        //public async Task<IActionResult> OnPost()
        //{
        //    var Result =  await _httpClient.PostAsync("BidderTenderUploads/Create", true, tenderUpload).ConfigureAwait(false);
        //    if (Result == null)
        //    {
        //        return RedirectToPage();
        //    }
        //    return RedirectToPage();

        //}

        //public async Task<IActionResult> OnPostAsync()  
        //{
        //    if (tenderUpload.IFFTenderDoc != null)
        //    {
        //        if (!_fileService.CheckValidFile(tenderUpload.IFFTenderDoc))
        //        {
        //            _notyf.Information("Please Upload Valid File");
        //            return RedirectToPage("Index");
        //        }
        //        tenderUpload.TenderDoc = await _fileService.SaveEncryptionAsync(@"\BidderTenders\" + tenderUpload.ProjectId + @"\" + tenderUpload.TenderNo + @"\", tenderUpload.IFFTenderDoc);
        //        tenderUpload.IFFTenderDoc = null;
        //    }
        //    if (tenderUpload.TenderCorrigendums.IFFCorrigendumDoc != null)
        //    {
        //        if (!_fileService.CheckValidFile(tenderUpload.TenderCorrigendums.IFFCorrigendumDoc))
        //        {
        //            _notyf.Information("Please Upload Valid File");
        //            return RedirectToPage("Index");
        //        }
        //        tenderUpload.TenderCorrigendums.CorrigendumDoc = await _fileService.SaveEncryptionAsync(@"\BidderTenders\" + tenderUpload.ProjectId + @"\" + tenderUpload.TenderNo + @"\", tenderUpload.TenderCorrigendums.IFFCorrigendumDoc);
        //        tenderUpload.TenderCorrigendums.IFFCorrigendumDoc = null;
        //    }
        //    if (tenderUpload.ForeignBidderIds != null)
        //    {
        //        string result = "";
        //        foreach (var item in tenderUpload.ForeignBidderIds) result += item + ",";
        //        if (result.EndsWith(",")) result = result.Substring(0, result.Length - 1);
        //        tenderUpload.ForeignBidderId = result;
        //    }
        //    if (tenderUpload.YardIds != null)
        //    {
        //        string result = "";
        //        foreach (var item in tenderUpload.YardIds) result += item + ",";
        //        if (result.EndsWith(",")) result = result.Substring(0, result.Length - 1);
        //        tenderUpload.YardId = result;
        //    }
        //    if (tenderUpload.YardIds != null)
        //    {
        //        string result = "";
        //        foreach (var item in tenderUpload.ProjectIds) result += item + ",";
        //        if (result.EndsWith(",")) result = result.Substring(0, result.Length - 1);
        //        tenderUpload.ProjectId = result;
        //    }
        //    if (tenderUpload.Id > 0)
        //    {
        //        tenderUpload.CreatedBy = User.Identity.Name;
        //        tenderUpload.CreatedDate = DateTime.Now;
        //        tenderUpload.IP = HttpContext.Connection.RemoteIpAddress.ToString();

        //        if (tenderUpload.TenderCorrigendums != null)
        //        {
        //            tenderUpload.TenderCorrigendums.CreatedBy = User.Identity.Name;
        //            tenderUpload.TenderCorrigendums.CreatedDate = DateTime.Now;
        //            tenderUpload.TenderCorrigendums.IP = HttpContext.Connection.RemoteIpAddress.ToString();
        //        }
        //    }
        //    tenderUpload = ModelAuditor<BidderTenderUploadsDTO>.SetAudit(User.Identity.Name, "Create", HttpContext.Connection.RemoteIpAddress.ToString(), tenderUpload);
        //    tenderUpload.TenderCorrigendums = ModelAuditor<BidderTenderCorrigendumDto>.SetAudit(User.Identity.Name, "Create", HttpContext.Connection.RemoteIpAddress.ToString(), tenderUpload.TenderCorrigendums);
        //    var Result = IsNew == true ? await _httpClient.PostAsync("BidderTenderUploads/Create", true, tenderUpload) : await _httpClient.PutAsync("BidderTenderUploads/Edit", true, tenderUpload.Id, tenderUpload);
        //    if (Result == "unauthorized")
        //    {
        //        _notyf.Information("Please login/register");
        //        return RedirectToPage("/Account/Login");
        //    }
        //    var TempDTO = !string.IsNullOrEmpty(Result) ? JsonConvert.DeserializeObject<BidderTenderUploadsDTO>(Result) : null;
        //    if (TempDTO == null)
        //        _notyf.Error("Save failed");
        //    else
        //        _notyf.Success("Saved successfully");
        //   return RedirectToPage("");
        //}

        //public async Task<JsonResult> OnGetGetYardsByProject(string projectId)
        //{
        //    var yardsResult = await _httpClient.GetAsync("Projects/GetYard", true, projectId).ConfigureAwait(false);
        //    if (yardsResult == "unauthorized") return new JsonResult(new { error = "unauthorized" });

        //    var yards = !string.IsNullOrEmpty(yardsResult)
        //        ? JsonConvert.DeserializeObject<List<BidderYardsDTO>>(yardsResult)
        //        : new List<BidderYardsDTO>();
        //    if (yards == null || yards.Count <= 0) return new JsonResult(new { error = "Record Not Found" });

        //    return new JsonResult(yards);
        //}
        public async Task<JsonResult> OnGetCheckTender(string projectId, string yardId, string tenderNo)
        {
            try
            {
                var modelDto = new BidderTenderUploadsDTO
                {
                    TenderNo = tenderNo,
                    ProjectId = projectId,
                    YardId = yardId
                };
                var yardsResult = await _httpClient.PostAsync("BidderTenderUploads/CheckTender", true, modelDto).ConfigureAwait(false);
                if (string.IsNullOrEmpty(yardsResult) || yardsResult.StartsWith("Failed") || yardsResult.StartsWith("Input"))
                {
                    IsNew = true;
                    return new JsonResult(new { status = "0", data = (object)null });
                }

                BidderTenderUploadsVM yards = null;
                try
                {
                    yards = JsonConvert.DeserializeObject<BidderTenderUploadsVM>(yardsResult);
                }
                catch
                {
                    IsNew = true;
                    return new JsonResult(new { status = "0", data = (object)null });
                }

                if (yards == null || yards.Id == 0)
                {
                    IsNew = true;
                    return new JsonResult(new { status = "0", data = (object)null });
                }

                _notyf.Success("Edit mode Enabled");
                if (yards.TenderCorrigendums != null)
                {
                    foreach (var cor in yards.TenderCorrigendums)
                    {
                        if (!string.IsNullOrEmpty(cor.CorrigendumDoc))
                        {
                            try
                            {
                                cor.CorrigendumDocDecrypted = EnDeCryptor.DecryptStringAES(cor.CorrigendumDoc.Split(".")[0].Replace("B_S", "\"").Replace("F_S", "/"));
                            }
                            catch { }
                        }
                    }
                }
                return new JsonResult(new { status = "1", data = yards });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { status = "0", message = ex.Message });
            }
        }
        public async Task<JsonResult> OnGetUploadQuota(string projectId, string fileSize)
        {
            var modelDto = new UpdateQuotaDTO
            {
                Id = Convert.ToInt32(projectId),
                OccupiedQuota = Convert.ToDecimal(fileSize)
            };
            var Result = await _httpClient.PostAsync("Projects/UploadQuota", true, modelDto).ConfigureAwait(false);
            if (Result == null)
            {
                IsNew = true;
                return new JsonResult(new { status = "0", data = "" });
            }

            var yards = !string.IsNullOrEmpty(Result)
                ? JsonConvert.DeserializeObject<UpdateQuotaDTO>(Result)
                : new UpdateQuotaDTO();

            return new JsonResult(new { status = "1", data = new JsonResult(yards) });
        }

        public async Task<IActionResult> OnPostDeleteCorrigendum(int id)
        {
            if (id <= 0 && int.TryParse(Request.Query["id"], out var queryId)) id = queryId;
            if (id <= 0 && int.TryParse(Request.Form["id"], out var formId)) id = formId;
            if (id <= 0) return new JsonResult(new { success = false, message = "Invalid ID" });
            var result = await _httpClient.PostAsync($"BidderTenderUploads/DeleteCorrigendum/{id}", true, id).ConfigureAwait(false);
            if (result == "unauthorized") return new JsonResult(new { success = false, message = "Unauthorized" });
            if (string.IsNullOrEmpty(result) || result.Contains("failed", StringComparison.OrdinalIgnoreCase) || result.Contains("BadRequest", StringComparison.OrdinalIgnoreCase))
            {
                return new JsonResult(new { success = false, message = result ?? "Failed to delete corrigendum." });
            }
            return new JsonResult(new { success = true, message = "Corrigendum deleted successfully." });
        }
    }
}