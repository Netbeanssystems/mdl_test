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


namespace WebBank.Pages.Admin.OtherLinkHeading
{
    [Authorize(Roles = "SuperAdmin,Editors,Admin,ContentCreator,Moderator,Publisher")]

    public class EditModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly IFileService _fileService;
        private readonly INotyfService _notyf;

        public EditModel(IHttpClientService httpClient, IFileService fileService, INotyfService notyf)
        {
            _httpClient = httpClient;
            _fileService = fileService;
            _notyf = notyf;
        }

        [FromRoute] public int id { get; set; }
        [BindProperty] public OtherLinkHeadingDTO OtherLinkHeading { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var CategoryResult = await _httpClient.GetAsync("OtherLinkHeading/Get", true, (int)id);
            if (CategoryResult == "unauthorized")
            {
                _notyf.Information("Please login/register");
                return RedirectToPage("/Account/Login");
            }
            OtherLinkHeading = !string.IsNullOrEmpty(CategoryResult) ? JsonConvert.DeserializeObject<OtherLinkHeadingDTO>(CategoryResult) : null;
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (OtherLinkHeading.EnglishFile != null)
            {//...........Check Valid File ...................
                if (!_fileService.CheckValidFile(OtherLinkHeading.EnglishFile))
                {
                    //......Redirect  
                    _notyf.Information("Please Upload Valid File");
                    return RedirectToPage("Index");
                }
                OtherLinkHeading.EnglishAttachment = await _fileService.SaveImageAsync(@"\img\UploadedFiles\OtherLinkHeading\Files\English\", OtherLinkHeading.EnglishFile);
                OtherLinkHeading.EnglishFile = null;
            }
            if (OtherLinkHeading.HindiFile != null)
            {//...........Check Valid File ...................
                if (!_fileService.CheckValidFile(OtherLinkHeading.HindiFile))
                {
                    //......Redirect  
                    _notyf.Information("Please Upload Valid File");
                    return RedirectToPage("Index");
                }
                OtherLinkHeading.HindiAttachment = await _fileService.SaveImageAsync(@"\img\UploadedFiles\OtherLinkHeading\Files\Hindi\", OtherLinkHeading.HindiFile);
                OtherLinkHeading.HindiFile = null;
            }
            var tempModelDTO = SetAudit("Edit", "Pending", OtherLinkHeading);
            return await CreateAudit(tempModelDTO);
        }


        public async Task<IActionResult> OnPostDelete(int Id)
        {
            var Result = await _httpClient.GetAsync("OtherLinkHeading/Get", true, Id);
            if (Result == "unauthorized")
            {
                _notyf.Information("Please login/register");
                return RedirectToPage("/Account/Login");
            }
            var ModelDTO = !string.IsNullOrEmpty(Result) ? JsonConvert.DeserializeObject<OtherLinkHeadingDTO>(Result) : null;
            if (ModelDTO == null)
            {
                _notyf.Information("New Project not found");
                return RedirectToPage(new { id = Id });
            }
            var tempModelDTO = SetAudit("Delete", "Pending", ModelDTO);
            return await CreateAudit(tempModelDTO);
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
                Action = action,
                Show = model.Show,
                ActionDate = DateTime.UtcNow,
                UpdateDate = DateTime.Now,
                UserName = uname,
                RoleName = role,
                Status = status,
                IP = HttpContext.Connection.RemoteIpAddress.ToString(),
                RowId = !action.Equals("Create", StringComparison.InvariantCultureIgnoreCase) ? id : 0
            };
        }

        private async Task<IActionResult> CreateAudit(TempOtherLinkHeadingDTO modelDto)
        {
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