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
    public class ApproveModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly IFileService _fileService;
        private readonly ICommon _common;
        public ApproveModel(IHttpClientService httpClient, IFileService fileService, ICommon common)
        {
            _httpClient = httpClient;
            _fileService = fileService;
            _common = common;
        }
        [FromRoute] public int id { get; set; }
        [BindProperty] public TempBannerDTO TempDTO { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var tempResult = await _httpClient.GetAsync("TempBanner/Get", true, id);
            if (tempResult == "unauthorized")
            {
                TempData["Message"] = "info^Please login/register";
                return RedirectToPage("/Account/Login");
            }
            TempDTO = !string.IsNullOrEmpty(tempResult) ? JsonConvert.DeserializeObject<TempBannerDTO>(tempResult) : null;
            if (TempDTO == null)
            {
                TempData["Message"] = "info^Record not found";
                return RedirectToPage("Pending");
            }
            return Page();
        }
        public async Task<IActionResult> OnPostApprove()
        {
            if (TempDTO.EnglishFile != null)
            {
                //...........Check Valid File ...................
                if (!_common.CheckValidFile(TempDTO.EnglishFile))
                {
                    //......Redirect  
                    TempData["Message"] = "info^Please Upload Valid File";
                    return RedirectToPage("Index");
                }
                TempDTO.EnglishBanner = await _fileService.SaveImageAsync(@"\img\UploadedFiles\HomeBanner\EnglishBanners\", TempDTO.EnglishFile);
                TempDTO.EnglishFile = null;
            }

            if (TempDTO.HindiFile != null)
            {
                //...........Check Valid File ...................
                if (!_common.CheckValidFile(TempDTO.HindiFile))
                {
                    //......Redirect  
                    TempData["Message"] = "info^Please Upload Valid File";
                    return RedirectToPage("Index");
                }
                TempDTO.HindiBanner = await _fileService.SaveImageAsync(@"\img\UploadedFiles\HomeBanner\HindiBanners\", TempDTO.HindiFile);
                TempDTO.HindiFile = null;
            }
            var modelDto = new BannerDTO
            {
                Id = TempDTO.RowId ?? 0,
                ParentId = TempDTO.ParentId,
                HindiHeadingName = TempDTO.HindiHeadingName,
                EnglishHeadingName = TempDTO.EnglishHeadingName,
                EnglishBanner = TempDTO.EnglishBanner,
                HindiBanner = TempDTO.HindiBanner,
                Priority = TempDTO.Priority,
                Show = TempDTO.Show,
                Linkopen = TempDTO.Linkopen,
                EnglishLink = TempDTO.EnglishLink,
                HindiLink = TempDTO.HindiLink,
            };

            if (TempDTO.Action == "Create")
            {

                var createResponse = await _httpClient.PostAsync("Banner/Create", true, modelDto);
                if (createResponse == "unauthorized")
                {
                    TempData["Message"] = "info^Please login/register";
                    return RedirectToPage("/Account/Login");
                }
                var createResult = !string.IsNullOrEmpty(createResponse) ? JsonConvert.DeserializeObject<BannerDTO>(createResponse) : null;
                if (createResult == null)
                {
                    TempData["Message"] = "danger^Create failed";
                    return RedirectToPage("Pending");
                }
                var tempModeldto = SetAudit(TempDTO.Action, "Approved", createResult);
                return await CreateAudit(tempModeldto);
            }

            if (TempDTO.Action == "Edit")
            {
                var editResponse = await _httpClient.PutAsync("Banner/Edit", true, modelDto.Id, modelDto);
                if (editResponse == "unauthorized")
                {
                    TempData["Message"] = "info^Please login/register";
                    return RedirectToPage("/Account/Login");
                }
                var editResult = !string.IsNullOrEmpty(editResponse) ? JsonConvert.DeserializeObject<BannerDTO>(editResponse) : null;
                if (editResult == null)
                {
                    TempData["Message"] = "danger^Edit failed";
                    return RedirectToPage("Pending");
                }
                var tempModelDto = SetAudit(TempDTO.Action, "Approved", editResult);
                return await CreateAudit(tempModelDto);
            }

            if (TempDTO.Action == "Delete")
            {
                var deleteResponse = await _httpClient.DeleteAsync("Banner/Delete", true, modelDto.Id);
                if (deleteResponse == "unauthorized")
                {
                    TempData["Message"] = "info^Please login/register";
                    return RedirectToPage("/Account/Login");
                }
                var RowsChanged = !string.IsNullOrEmpty(deleteResponse) && Convert.ToInt32(deleteResponse) > 0;
                if (!RowsChanged)
                {
                    TempData["Message"] = "danger^Delete failed";
                    return RedirectToPage("Pending");
                }
                var tempModelDto = SetAudit(TempDTO.Action, "Approved", modelDto);
                return await CreateAudit(tempModelDto);
            }
            TempData["Message"] = "danger^Action not specified";
            return RedirectToPage("Pending");
        }

        public async Task<IActionResult> OnPostReject()
        {
            var ModelDTO = new BannerDTO
            {
                Id = TempDTO.RowId ?? 0,
                ParentId = TempDTO.ParentId,
                HindiHeadingName = TempDTO.HindiHeadingName,
                EnglishHeadingName = TempDTO.EnglishHeadingName,
                EnglishBanner = TempDTO.EnglishBanner,
                HindiBanner = TempDTO.HindiBanner,
                Priority = TempDTO.Priority,
                Linkopen = TempDTO.Linkopen,
                EnglishLink = TempDTO.EnglishLink,
                HindiLink = TempDTO.HindiLink,
            };
            var tempModelDTO = SetAudit(TempDTO.Action, "Rejected", ModelDTO);
            return await CreateAudit(tempModelDTO);
        }

        private TempBannerDTO SetAudit(string action, string status, BannerDTO model)
        {
            var uname = User.Identity.Name;
            var role = User.Claims.First(c => c.Type == ClaimTypes.Role).Value;
            return new TempBannerDTO
            {
                HindiHeadingName = TempDTO.HindiHeadingName,
                EnglishHeadingName = TempDTO.EnglishHeadingName,
                EnglishBanner = TempDTO.EnglishBanner,
                HindiBanner = TempDTO.HindiBanner,
                Priority = model.Priority,
                ParentId = model.ParentId,
                Action = action,
                ActionDate = DateTime.UtcNow,
                UserName = uname,
                RoleName = role,
                Status = status,
                IP = HttpContext.Connection.RemoteIpAddress.ToString(),
                RowId = model.Id == 0 ? (int?)null : model.Id,
                Show = model.Show,
                Linkopen = model.Linkopen,
                EnglishLink = model.EnglishLink,
                HindiLink = model.HindiLink,
            };
        }
        private async Task<IActionResult> CreateAudit(TempBannerDTO modelDto)
        {
            if (id != 0)
            {
                modelDto.Id = id;
                modelDto.Show = false;
                var example = await _httpClient.PutAsync("TempBanner/Edit", true, id, modelDto);
            }
            modelDto.Show = true;
            modelDto.Id = 0;
            var tempResponse = await _httpClient.PostAsync("TempBanner/CreateAudit", true, modelDto);
            if (tempResponse == "unauthorized")
            {
                TempData["Message"] = "info^Please login/register";
                return RedirectToPage("/Account/Login");
            }
            var tempResult = !string.IsNullOrEmpty(tempResponse) ? JsonConvert.DeserializeObject<TempBannerDTO>(tempResponse) : null;
            TempData["Message"] = tempResult == null ? "danger^Operation failed" : "success^Operation successfull";
            return RedirectToPage("Pending");
        }
    }
}
