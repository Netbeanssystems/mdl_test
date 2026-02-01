using Application.Dtos;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebApp.Pages.Admin.HomePage.Banner
{
    [Authorize(Roles = "SuperAdmin,Editors,Admin,ContentCreator,Moderator,Publisher")]
    //[SessionManage]
    public class EditModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly IFileService _fileService;
        private readonly ICommon _common;
        public EditModel(IHttpClientService httpClient, IFileService fileService, ICommon common)
        {
            _httpClient = httpClient;
            _fileService = fileService;
            _common = common;
        }

        [FromRoute] public int? id { get; set; }
        [BindProperty] public BannerDTO MenuHeading { get; set; }
        public async Task<IActionResult> OnGet()
        {
            var CategoryResult = await _httpClient.GetAsync("Banner/Get", true, (int)id);
            if (CategoryResult == "unauthorized")
            {
                TempData["Message"] = "info^Please login/register";
                return RedirectToPage("/Account/Login");
            }
            MenuHeading = !string.IsNullOrEmpty(CategoryResult) ? JsonConvert.DeserializeObject<BannerDTO>(CategoryResult) : null;

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (MenuHeading.EnglishFile != null)
            {
                //...........Check Valid File ...................
                if (!_common.CheckValidFile(MenuHeading.EnglishFile))
                {
                    //......Redirect  
                    TempData["Message"] = "info^Please Upload Valid File";
                    return RedirectToPage("Index");
                }
                MenuHeading.EnglishBanner = await _fileService.SaveImageAsync(@"\img\UploadedFiles\HomeBanner\EnglishBanners\", MenuHeading.EnglishFile);
                MenuHeading.EnglishFile = null;
            }
            if (MenuHeading.HindiFile != null)
            {
                //...........Check Valid File ...................
                if (!_common.CheckValidFile(MenuHeading.HindiFile))
                {
                    //......Redirect  
                    TempData["Message"] = "info^Please Upload Valid File";
                    return RedirectToPage("Index");
                }
                MenuHeading.HindiBanner = await _fileService.SaveImageAsync(@"\img\UploadedFiles\HomeBanner\HindiBanners\", MenuHeading.HindiFile);
                MenuHeading.EnglishFile = null;
            }
            var tempModelDTO = SetAudit("Edit", "Pending", MenuHeading);
            return await CreateAudit(tempModelDTO);
        }

        public async Task<IActionResult> OnPostDelete(int Id)
        {
            var Result = await _httpClient.GetAsync("Banner/Get", true, Id);
            if (Result == "unauthorized")
            {
                TempData["Message"] = "info^Please login/register";
                return RedirectToPage("/Account/Login");
            }
            var ModelDTO = !string.IsNullOrEmpty(Result) ? JsonConvert.DeserializeObject<BannerDTO>(Result) : null;
            if (ModelDTO == null)
            {
                TempData["Message"] = "info^New Project not found";
                return RedirectToPage(new { id = Id });
            }
            var tempModelDTO = SetAudit("Delete", "Pending", ModelDTO);
            return await CreateAudit(tempModelDTO);
        }
        private TempBannerDTO SetAudit(string action, string status, BannerDTO model)
        {
            var uname = User.Identity.Name;
            var role = User.Claims.First(c => c.Type == ClaimTypes.Role).Value;
            return new TempBannerDTO
            {
                EnglishBanner = model.EnglishBanner,
                HindiBanner = model.HindiBanner,
                HindiHeadingName = model.HindiHeadingName,
                EnglishHeadingName = model.EnglishHeadingName,
                ParentId = model.ParentId,
                Priority = model.Priority,
                Action = action,
                Show = model.Show,
                Linkopen = model.Linkopen,
                EnglishLink = model.EnglishLink,
                HindiLink = model.HindiLink,
                ActionDate = DateTime.UtcNow,
                UserName = uname,
                RoleName = role,
                Status = status,
                IP = HttpContext.Connection.RemoteIpAddress.ToString(),
                RowId = !action.Equals("Create", StringComparison.InvariantCultureIgnoreCase) ? id : null
            };
        }
        private async Task<IActionResult> CreateAudit(TempBannerDTO modelDto)
        {
            var Result = await _httpClient.PostAsync("TempBanner/CreateAudit", true, modelDto);
            if (Result == "unauthorized")
            {
                TempData["Message"] = "info^Please login/register";
                return RedirectToPage("/Account/Login");
            }
            var TempDTO = !string.IsNullOrEmpty(Result) ? JsonConvert.DeserializeObject<BannerDTO>(Result) : null;
            TempData["Message"] = TempDTO == null ? "danger^Save failed" : "success^Saved for approval";
            return RedirectToPage("Index");
        }
    }
}