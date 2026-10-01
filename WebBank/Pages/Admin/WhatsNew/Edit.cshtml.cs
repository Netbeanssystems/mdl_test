using Application.Dtos;
using Application.ServiceInterfaces;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebBank.Pages.Admin.WhatsNew
{
    [Authorize(Roles = "SuperAdmin,Editors,Admin,ContentCreator,Moderator,Publisher")]
    public class EditModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly IFileService _fileService;
        private readonly ICommon _common;
        private readonly INotyfService _notyf;

        public EditModel(IHttpClientService httpClient, IFileService fileService, ICommon common, INotyfService notyf)
        {
            _httpClient = httpClient;
            _fileService = fileService;
            _common = common;
            _notyf = notyf;
        }

        [FromRoute] public int? id { get; set; }
        [BindProperty] public WhatsNewDTO MenuHeading { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var CategoryResult = await _httpClient.GetAsync("WhatsNew/Get", true, (int)id);
            if (CategoryResult == "unauthorized")
            {
                TempData["Message"] = "info^Please login/register";
                return RedirectToPage("/Account/Login");
            }
            MenuHeading = !string.IsNullOrEmpty(CategoryResult) ? JsonConvert.DeserializeObject<WhatsNewDTO>(CategoryResult) : null;
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (MenuHeading.EnglishFile != null)
            { //...........Check Valid File ...................
                if (!_common.CheckValidFile(MenuHeading.EnglishFile))
                {
                    //......Redirect  
                    TempData["Message"] = "info^Please Upload Valid File";
                    return RedirectToPage("Index");
                }
                MenuHeading.EnglishAttachment = await _fileService.SaveImageAsync(@"\img\UploadedFiles\WhatsNew\Images\", MenuHeading.EnglishFile);
                MenuHeading.EnglishFile = null;
            }
            if (MenuHeading.EnglishFile != null)
            {//...........Check Valid File ...................
                if (!_common.CheckValidFile(MenuHeading.EnglishFile))
                {
                    //......Redirect  
                    TempData["Message"] = "info^Please Upload Valid File";
                    return RedirectToPage("Index");
                }
                MenuHeading.HindiAttachment = await _fileService.SaveImageAsync(@"\img\UploadedFiles\WhatsNew\Images\", MenuHeading.HindiFile);
                MenuHeading.EnglishFile = null;
            }
            var tempModelDTO = SetAudit("Edit", "Pending", MenuHeading);
            _notyf.Success("Updated Successfully");
            return await CreateAudit(tempModelDTO);
        }

        public async Task<IActionResult> OnPostDelete(int Id)
        {
            var Result = await _httpClient.GetAsync("WhatsNew/Get", true, Id);
            if (Result == "unauthorized")
            {
                TempData["Message"] = "info^Please login/register";
                return RedirectToPage("/Account/Login");
            }
            var ModelDTO = !string.IsNullOrEmpty(Result) ? JsonConvert.DeserializeObject<WhatsNewDTO>(Result) : null;
            if (ModelDTO == null)
            {
                TempData["Message"] = "info^New Project not found";
                return RedirectToPage(new { id = Id });
            }
            var tempModelDTO = SetAudit("Delete", "Pending", ModelDTO);
            _notyf.Success("Deleted Successfully");
            return await CreateAudit(tempModelDTO);
        }

        private TempWhatsNewDTO SetAudit(string action, string status, WhatsNewDTO model)
        {
            var uname = User.Identity.Name;
            var role = User.Claims.First(c => c.Type == ClaimTypes.Role).Value;
            return new TempWhatsNewDTO
            {
                EnglishAttachment = model.EnglishAttachment,
                HindiAttachment = model.HindiAttachment,
                EnglishContentDesc = model.EnglishContentDesc,
                HindiContentDesc = model.HindiContentDesc,
                HindiPageLink = model.HindiPageLink,
                EnglishPageLink = model.EnglishPageLink,
                HindiHeadingName = model.HindiHeadingName,
                EnglishHeadingName = model.EnglishHeadingName,
                ParentId = model.ParentId,
                Priority = model.Priority,
                Action = action,
                Show = model.Show,
                ActionDate = DateTime.UtcNow,
                UserName = uname,
                RoleName = role,
                Status = status,
                PublishDate = model.PublishDate,
                IP = HttpContext.Connection.RemoteIpAddress.ToString(),
                RowId = !action.Equals("Create", StringComparison.InvariantCultureIgnoreCase) ? id : null
            };
        }

        private async Task<IActionResult> CreateAudit(TempWhatsNewDTO modelDto)
        {
            var Result = await _httpClient.PostAsync("TempWhatsNew/CreateAudit", true, modelDto);
            if (Result == "unauthorized")
            {
                TempData["Message"] = "info^Please login/register";
                return RedirectToPage("/Account/Login");
            }
            var TempDTO = !string.IsNullOrEmpty(Result) ? JsonConvert.DeserializeObject<TempWhatsNewDTO>(Result) : null;
            TempData["Message"] = TempDTO == null ? "danger^Save failed" : "success^Saved for approval";
            return RedirectToPage("Index");
        }
    }
}
