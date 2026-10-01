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


namespace WebApp.Pages.Admin.OtherLinkHeading
{

    [Authorize(Roles = "SuperAdmin")]
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
        [BindProperty] public OtherLinkHeadingDTO otherlinkdto { get; set; }

        public async Task<IActionResult> OnPost()
        {
            if (pid != null)
            {
                otherlinkdto.ParentId = pid;
            }
            if (otherlinkdto.EnglishFile != null)
            {//...........Check Valid File ...................
                if (!_fileService.CheckValidFile(otherlinkdto.EnglishFile))
                {
                    //......Redirect  
                    _notyf.Information("Please Upload Valid File");
                    return RedirectToPage("Index");
                }
                otherlinkdto.EnglishAttachment = await _fileService.SaveImageAsync(@"\img\UploadedFiles\OtherLinkHeading\Files\English\", otherlinkdto.EnglishFile);
                otherlinkdto.EnglishFile = null;
            }
            if (otherlinkdto.HindiFile != null)
            {//...........Check Valid File ...................
                if (!_fileService.CheckValidFile(otherlinkdto.HindiFile))
                {
                    //......Redirect  
                    _notyf.Information("Please Upload Valid File");
                    return RedirectToPage("Index");
                }
                otherlinkdto.HindiAttachment = await _fileService.SaveImageAsync(@"\img\UploadedFiles\OtherLinkHeading\Files\Hindi\", otherlinkdto.HindiFile);
                otherlinkdto.HindiFile = null;
            }
            return await CreateAudit(SetAudit("Create", "Pending", otherlinkdto));
        }

        private TempOtherLinkHeadingDTO SetAudit(string action, string status, OtherLinkHeadingDTO model)
        {
            var uname = User.Identity.Name;
            var role = User.Claims.First(c => c.Type == ClaimTypes.Role).Value;
            return new TempOtherLinkHeadingDTO
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
                Title = model.Title,
                HindiTitle = model.HindiTitle,
                Description = model.Description,
                Keyword = model.Keyword,
                KeywordHindi = model.KeywordHindi,
                DescriptionHindi = model.KeywordHindi,
                Action = action,
                Show = model.Show,
                ActionDate = DateTime.UtcNow,
                UpdateDate = DateTime.Now,
                UserName = uname,
                RoleName = role,
                Status = status,
                IP = HttpContext.Connection.RemoteIpAddress.ToString(),
                RowId = null
            };
        }

        private async Task<IActionResult> CreateAudit(TempOtherLinkHeadingDTO modelDto)
        {
            var x = ModelState.IsValid;
            //var Result = await _httpClient.PostAsync("TempOtherLinkHeading/CreateAudit", true, modelDto);
            var Result = await _httpClient.PostAsync("TempOtherLink/CreateAudit", true, modelDto);
            if (Result == "unauthorized")
            {
                _notyf.Information("Please login/register");
                return RedirectToPage("/Account/Login");
            }
            var TempDTO = !string.IsNullOrEmpty(Result) ? JsonConvert.DeserializeObject<TempOtherLinkHeadingDTO>(Result) : null;
            if (TempDTO == null)
                _notyf.Error("Save failed");
            else
                _notyf.Success("Saved successfully");
            return RedirectToPage("Index");
        }
    }
}