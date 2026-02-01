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


namespace WebBank.Pages.Admin.MenuHeadings
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
        [BindProperty] public MenuHeadingsDTO MenuHeadings { get; set; }
        public async Task<IActionResult> OnGet()
        {
            var CategoryResult = await _httpClient.GetAsync("MenuHeadings/Get", true, (int)id);
            if (CategoryResult == "unauthorized")
            {
                _notyf.Information("Please login/register");
                return RedirectToPage("/Account/Login");
            }
            MenuHeadings = !string.IsNullOrEmpty(CategoryResult) ? JsonConvert.DeserializeObject<MenuHeadingsDTO>(CategoryResult) : null;

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (MenuHeadings.EnglishFile != null)
            {//...........Check Valid File ...................
                if (!_fileService.CheckValidFile(MenuHeadings.EnglishFile))
                {
                    //......Redirect  
                    _notyf.Information("Please Upload Valid File");
                    return RedirectToPage("Index");
                }
                MenuHeadings.EnglishAttachment = await _fileService.SaveImageAsync(@"\img\UploadedFiles\MenuHeadings\Files\English\", MenuHeadings.EnglishFile);
                MenuHeadings.EnglishFile = null;
            }
            if (MenuHeadings.EnglishFile != null)
            {//...........Check Valid File ...................
                if (!_fileService.CheckValidFile(MenuHeadings.HindiFile))
                {
                    //......Redirect  
                    _notyf.Information("Please Upload Valid File");
                    return RedirectToPage("Index");
                }
                MenuHeadings.HindiAttachment = await _fileService.SaveImageAsync(@"\img\UploadedFiles\MenuHeadings\Files\Hindi\", MenuHeadings.HindiFile);
                MenuHeadings.EnglishFile = null;
            }
            var tempModelDTO = SetAudit("Edit", "Pending", MenuHeadings);
            return await CreateAudit(tempModelDTO);
        }

        public async Task<IActionResult> OnPostDelete(int Id)
        {
            var Result = await _httpClient.GetAsync("MenuHeadings/Get", true, Id);
            if (Result == "unauthorized")
            {
                _notyf.Information("Please login/register");
                return RedirectToPage("/Account/Login");
            }
            var ModelDTO = !string.IsNullOrEmpty(Result) ? JsonConvert.DeserializeObject<MenuHeadingsDTO>(Result) : null;
            if (ModelDTO == null)
            {
                _notyf.Information("New Project not found");
                return RedirectToPage(new { id = Id });
            }
            var tempModelDTO = SetAudit("Delete", "Pending", ModelDTO);
            return await CreateAudit(tempModelDTO);
        }
        private TempMenuHeadingsDTO SetAudit(string action, string status, MenuHeadingsDTO model)
        {
            var uname = User.Identity.Name;
            var role = User.Claims.First(c => c.Type == ClaimTypes.Role).Value;
            return new TempMenuHeadingsDTO
            {
                EnglishAttachment = model.EnglishAttachment,
                HindiAttachment = model.HindiAttachment,
                EnglishContentDesc = model.EnglishContentDesc,
                HindiContentDesc = model.HindiContentDesc,
                HindiPageLink = model.HindiPageLink,
                EnglishPageLink = model.EnglishPageLink,
                HindiHeadingName = model.HindiHeadingName,
                EnglishHeadingName = model.EnglishHeadingName,
                Title = model.Title,
                HindiTitle = model.HindiTitle,
                Description = model.Description,
                Keyword = model.Keyword,
                ParentId = model.ParentId,
                Priority = model.Priority,
                Action = action,
                Show = model.Show,
                ActionDate = DateTime.UtcNow,
                UpdateDate = DateTime.Now,
                UserName = uname,
                RoleName = role,
                Status = status,
                IP = HttpContext.Connection.RemoteIpAddress.ToString(),
                RowId = !action.Equals("Create", StringComparison.InvariantCultureIgnoreCase) ? id : null
            };
        }
        private async Task<IActionResult> CreateAudit(TempMenuHeadingsDTO modelDto)
        {
            var Result = await _httpClient.PostAsync("TempMenuHeadings/CreateAudit", true, modelDto);
            if (Result == "unauthorized")
            {
                _notyf.Information("Please login/register");
                return RedirectToPage("/Account/Login");
            }
            var TempDTO = !string.IsNullOrEmpty(Result) ? JsonConvert.DeserializeObject<TempMenuHeadingsDTO>(Result) : null;
            if (TempDTO == null)
                _notyf.Error("Save failed");
            else
                _notyf.Success("Saved for approval");
            return RedirectToPage("Index");
        }
    }
}