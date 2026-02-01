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

namespace WebApp.Pages.Admin.WhatsNew
{
    [Authorize(Roles = "SuperAdmin,Editors,Admin,ContentCreator,Moderator,Publisher")]
    public class AddModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly IFileService _fileService;
        private readonly ICommon _common;
        private readonly INotyfService _notyf;
        public AddModel(IHttpClientService httpClient, IFileService fileService, ICommon common, INotyfService notyf)
        {
            _httpClient = httpClient;
            _fileService = fileService;
            _common = common;
            _notyf = notyf;
        }
        [FromRoute] public int? pid { get; set; }
        [BindProperty] public WhatsNewDTO Whats { get; set; }

        public async Task<IActionResult> OnPost()
        {
            if (pid != null)
            {
                Whats.ParentId = pid;
            }
            if (Whats.EnglishFile != null)
            { //...........Check Valid File ...................
                if (!_common.CheckValidFile(Whats.EnglishFile))
                {
                    //......Redirect  
                    TempData["Message"] = "info^Please Upload Valid File";
                    return RedirectToPage("Index");
                }
                Whats.EnglishAttachment = await _fileService.SaveImageAsync(@"\img\UploadedFiles\WhatsNew\Images\", Whats.EnglishFile);
                Whats.EnglishFile = null;
            }
            if (Whats.HindiFile != null)
            {//...........Check Valid File ...................
                if (!_common.CheckValidFile(Whats.HindiFile))
                {
                    //......Redirect  
                    TempData["Message"] = "info^Please Upload Valid File";
                    return RedirectToPage("Index");
                }
                Whats.HindiAttachment = await _fileService.SaveImageAsync(@"\img\UploadedFiles\WhatsNew\Images\", Whats.HindiFile);
                Whats.HindiFile = null;
            }

            if (Whats.PublishDate == null)
            {
                Whats.PublishDate = System.DateTime.Now;
            }
            //_notyf.Success("Added Successfully");
            return await CreateAudit(SetAudit("Create", "Pending", Whats));
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
                RowId = null
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
            //TempData["Message"] = TempDTO == null ? "danger^Save failed" : "success^Saved for approval";

            if (TempDTO == null)
            {

                _notyf.Error("Save failed");
            }
            else
            {
                _notyf.Success("Saved for approval");
            }
            return RedirectToPage("Index");
        }
    }
}