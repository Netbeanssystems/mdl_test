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
                var ForeignBidder = !string.IsNullOrEmpty(ForiegnbidderResult)
                    ? JsonConvert.DeserializeObject<List<UserRoleVM>>(ForiegnbidderResult)
                    : new List<UserRoleVM>();

                var BidderResult = await _httpClient.GetAsync("Users/GetByRole", true, "Bidder").ConfigureAwait(false);
                if (BidderResult == "unauthorized") return RedirectToPage("/Account/Login");
                var BidderList = !string.IsNullOrEmpty(BidderResult)
                    ? JsonConvert.DeserializeObject<List<UserRoleVM>>(BidderResult)
                    : new List<UserRoleVM>();

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

            }
            if (UserProfileDto != null) return Page();
            _notyf.Error("User not found");
            return RedirectToPage("/Index");

        }
        public async Task<IActionResult> OnPost()
        {
            // Validate and save tender document
            if (tenderUpload.IFFTenderDoc != null)
            {
                if (!_fileService.CheckValidFile(tenderUpload.IFFTenderDoc))
                {
                    _notyf.Information("Please Upload    Valid File");
                    return RedirectToPage("Index");
                }

                foreach (var item in tenderUpload.ProjectIds)
                {
                    tenderUpload.TenderDoc = await _fileService.SaveEncryptionAsync(
                        $@"\BidderTenders\{item}\{tenderUpload.TenderNo}\",
                        tenderUpload.IFFTenderDoc);
                }
                tenderUpload.IFFTenderDoc = null;
            }

            // Validate and save corrigendum document
            if (tenderUpload.TenderCorrigendums?.IFFCorrigendumDoc != null)
            {
                if (!_fileService.CheckValidFile(tenderUpload.TenderCorrigendums.IFFCorrigendumDoc))
                {
                    _notyf.Information("Please Upload Valid File");
                    return RedirectToPage("Index");
                }

                foreach (var item in tenderUpload.ProjectIds)
                {
                    tenderUpload.TenderCorrigendums.CorrigendumDoc = await _fileService.SaveEncryptionAsync(
                    $@"\BidderTenders\{item}\{tenderUpload.TenderNo}\",
                    tenderUpload.TenderCorrigendums.IFFCorrigendumDoc
                );
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

            // Audit fields
            if (tenderUpload.Id == 0) // New record
            {
                tenderUpload.CreatedBy = User.Identity.Name;
                tenderUpload.CreatedDate = DateTime.Now;
                tenderUpload.IP = HttpContext.Connection.RemoteIpAddress.ToString();
            }
            else // Update existing record
            {
                tenderUpload.ModifiedBy = User.Identity.Name;
                tenderUpload.ModifiedDate = DateTime.Now;
                tenderUpload.IP = HttpContext.Connection.RemoteIpAddress.ToString();
            }

            if (tenderUpload.TenderCorrigendums != null)
            {
                if (tenderUpload.TenderCorrigendums.Id == 0)
                {
                    tenderUpload.TenderCorrigendums.CreatedBy = User.Identity.Name;
                    //tenderUpload.TenderCorrigendums.CreatedDate = DateTime.Now;
                    if (tenderUpload.TenderCorrigendums.CreatedDate < new DateTime(1753, 1, 1))
                        tenderUpload.TenderCorrigendums.CreatedDate = DateTime.Now;
                    tenderUpload.TenderCorrigendums.IP = HttpContext.Connection.RemoteIpAddress.ToString();
                }
                else
                {
                    tenderUpload.TenderCorrigendums.ModifiedBy = User.Identity.Name;
                    //tenderUpload.TenderCorrigendums.ModifiedDate = DateTime.Now;
                    if (tenderUpload.TenderCorrigendums.CreatedDate < new DateTime(1753, 1, 1))
                        tenderUpload.TenderCorrigendums.CreatedDate = DateTime.Now;
                    tenderUpload.TenderCorrigendums.IP = HttpContext.Connection.RemoteIpAddress.ToString();
                }
            }
            tenderUpload.TenderCorrigendums.CreatedBy = User.Identity.Name;
            // Set proper audit action
            string auditAction = tenderUpload.Id == 0 ? "Create" : "Edit";
            tenderUpload = ModelAuditor<BidderTenderUploadsDTO>.SetAudit(User.Identity.Name, auditAction, HttpContext.Connection.RemoteIpAddress.ToString(), tenderUpload);
            tenderUpload.TenderCorrigendums = ModelAuditor<BidderTenderCorrigendumDto>.SetAudit(User.Identity.Name, auditAction, HttpContext.Connection.RemoteIpAddress.ToString(), tenderUpload.TenderCorrigendums);

            // Call API (POST for new, PUT for update)
            var Result = tenderUpload.Id == 0
                ? await _httpClient.PostAsync("BidderTenderUploads/Create", true, tenderUpload)
                : await _httpClient.PutAsync("BidderTenderUploads/Edit", true, tenderUpload.Id, tenderUpload);

            if (Result == "unauthorized")
            {
                _notyf.Information("Please login/register");
                return RedirectToPage("/Account/Login");
            }

            var TempDTO = !string.IsNullOrEmpty(Result)
                ? JsonConvert.DeserializeObject<BidderTenderUploadsDTO>(Result)
                : null;

            if (TempDTO == null)
            {
                _notyf.Error("Save failed");
                return RedirectToPage();
            }
            else
            {
                _notyf.Success("Saved successfully");
                return RedirectToPage();
            }
            return RedirectToPage();

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
            var modelDto = new BidderTenderUploadsDTO
            {
                TenderNo = tenderNo,
                ProjectId = projectId,
                YardId = yardId
            };
            var yardsResult = await _httpClient.PostAsync("BidderTenderUploads/CheckTender", true, modelDto).ConfigureAwait(false);
            if (yardsResult == null)
            {
                IsNew = true;
                return new JsonResult(new { status = "0", data = "" });
            }

            var yards = !string.IsNullOrEmpty(yardsResult)
                ? JsonConvert.DeserializeObject<BidderTenderUploadsVM>(yardsResult)
                : new BidderTenderUploadsVM();
            _notyf.Success("Edit mode Enabled");
            foreach (var cor in yards.TenderCorrigendums) {
                if (cor.CorrigendumDoc != null) cor.CorrigendumDoc = EnDeCryptor.DecryptStringAES(cor.CorrigendumDoc.Split(".")[0].Replace("B_S", "\"").Replace("F_S", "/"));
            };
            return new JsonResult(new { status = "1", data = new JsonResult(yards) });
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
    }
}