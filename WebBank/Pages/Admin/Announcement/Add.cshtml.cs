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


namespace WebBank.Pages.Admin.Announcement
{

    [Authorize(Roles = "SuperAdmin,Editors,Admin,ContentCreator,Moderator,Publisher")]
    public class AddModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly IFileService _fileService;
        private readonly INotyfService _notyf;

        public AddModel(IHttpClientService httpClient, IFileService fileService, INotyfService notyf)
        {
            _httpClient = httpClient;
            _fileService = fileService;
            _notyf = notyf;
        }
        [FromRoute] public int? pid { get; set; }
        [BindProperty] public NewsDTO MenuHeading { get; set; }

        public async Task<IActionResult> OnPost()
        {
            if (pid != null)
            {
                MenuHeading.ParentId = pid;
            }
            if (MenuHeading.EnglishFile != null)
            { //...........Check Valid File ...................
                if (!_fileService.CheckValidFile(MenuHeading.EnglishFile))
                {
                    //......Redirect  
                    _notyf.Information("Please Upload Valid File");
                    return RedirectToPage("Index");
                }
                MenuHeading.EnglishAttachment = await _fileService.SaveImageAsync(@"\img\UploadedFiles\News\Images\", MenuHeading.EnglishFile);
                MenuHeading.EnglishFile = null;
            }
            if (MenuHeading.HindiFile != null)
            {//...........Check Valid File ...................
                if (!_fileService.CheckValidFile(MenuHeading.HindiFile))
                {
                    //......Redirect  
                    _notyf.Information("Please Upload Valid File");
                    return RedirectToPage("Index");
                }
                MenuHeading.HindiAttachment = await _fileService.SaveImageAsync(@"\img\UploadedFiles\News\Images\", MenuHeading.HindiFile);
                MenuHeading.HindiFile = null;
            }
            //_notyf.Success("Added Successfully");
            return await CreateAudit(SetAudit("Create", "Pending", MenuHeading));
        }

        private TempNewsDTO SetAudit(string action, string status, NewsDTO model)
        {
            var uname = User.Identity.Name;
            var role = User.Claims.First(c => c.Type == ClaimTypes.Role).Value;
            return new TempNewsDTO
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
                IP = HttpContext.Connection.RemoteIpAddress.ToString(),
                RowId = null
            };
        }

        private async Task<IActionResult> CreateAudit(TempNewsDTO modelDto)
        {
            var Result = await _httpClient.PostAsync("TempNews/CreateAudit", true, modelDto);
            if (Result == "unauthorized")
            {
                _notyf.Information("Please login/register");
                return RedirectToPage("/Account/Login");
            }
            var TempDTO = !string.IsNullOrEmpty(Result) ? JsonConvert.DeserializeObject<TempNewsDTO>(Result) : null;
            if (TempDTO == null)
                _notyf.Error("Save failed");
            else
                _notyf.Success("Saved successfully");
            return RedirectToPage("Index");
        }
    }
}