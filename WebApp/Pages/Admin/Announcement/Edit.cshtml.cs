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


namespace WebApp.Pages.Admin.Announcement
{

    [Authorize(Roles = "SuperAdmin,Editors,Admin,ContentCreator,Moderator,Publisher")]
    public class EditModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly IFileService _fileService;
        private readonly INotyfService _notyf;

        public EditModel(
          IHttpClientService httpClient, IFileService fileService, INotyfService notyf)
        {
            _httpClient = httpClient;
            _fileService = fileService;
            _notyf = notyf;
        }

        [FromRoute] public int? id { get; set; }
        [BindProperty] public NewsDTO News { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var CategoryResult = await _httpClient.GetAsync("News/Get", true, (int)id);
            if (CategoryResult == "unauthorized")
            {
                _notyf.Information("Please login/register");
                return RedirectToPage("/Account/Login");
            }
            News = !string.IsNullOrEmpty(CategoryResult) ? JsonConvert.DeserializeObject<NewsDTO>(CategoryResult) : null;

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (News.EnglishFile != null)
            { //...........Check Valid File ...................
                if (!_fileService.CheckValidFile(News.EnglishFile))
                {
                    //......Redirect  
                    _notyf.Information("Please Upload Valid File");
                    return RedirectToPage("Index");
                }
                News.EnglishAttachment = await _fileService.SaveImageAsync(@"\img\UploadedFiles\News\Images\", News.EnglishFile);
                News.EnglishFile = null;
            }
            if (News.EnglishFile != null)
            {//...........Check Valid File ...................
                if (!_fileService.CheckValidFile(News.EnglishFile))
                {
                    //......Redirect  
                    _notyf.Information("Please Upload Valid File");
                    return RedirectToPage("Index");
                }
                News.HindiAttachment = await _fileService.SaveImageAsync(@"\img\UploadedFiles\News\Images\", News.HindiFile);
                News.EnglishFile = null;
            }
            var tempModelDTO = SetAudit("Edit", "Pending", News);
            _notyf.Success("Saved successfully");
            return await CreateAudit(tempModelDTO);
        }

        public async Task<IActionResult> OnPostDelete(int Id)
        {
            var Result = await _httpClient.GetAsync("News/Get", true, Id);
            if (Result == "unauthorized")
            {
                _notyf.Information("Please login/register");
                return RedirectToPage("/Account/Login");
            }
            var ModelDTO = !string.IsNullOrEmpty(Result) ? JsonConvert.DeserializeObject<NewsDTO>(Result) : null;
            if (ModelDTO == null)
            {
                _notyf.Information("New Project not found");
                return RedirectToPage(new { id = Id });
            }
            var tempModelDTO = SetAudit("Delete", "Pending", ModelDTO);
            _notyf.Success("Deleted successfully");
            return await CreateAudit(tempModelDTO);
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
                RowId = !action.Equals("Create", StringComparison.InvariantCultureIgnoreCase) ? id : null
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