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
    public class AddModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly IFileService _fileService;
        private readonly ICommon _common;
        public AddModel(IHttpClientService httpClient, IFileService fileService, ICommon common)
        {
            _httpClient = httpClient;
            _fileService = fileService;
            _common = common;
        }

        [FromRoute] public int? pid { get; set; }
        [BindProperty] public BannerDTO MenuHeading { get; set; }
        public async Task<IActionResult> OnPost()
        {
            if (pid != null)
            {
                MenuHeading.ParentId = pid;
            }
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
                MenuHeading.HindiFile = null;
            }
            MenuHeading.ParentId = 1;

            return await CreateAudit(SetAudit("Create", "Pending", MenuHeading));
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
                RowId = null
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
            var TempDTO = !string.IsNullOrEmpty(Result) ? JsonConvert.DeserializeObject<TempBannerDTO>(Result) : null;
            TempData["Message"] = TempDTO == null ? "danger^Save failed" : "success^Saved for approval";
            return RedirectToPage("Index");
        }
    }
}
