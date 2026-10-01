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
    public class ApproveModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly IFileService _fileService;
        private readonly INotyfService _notyf;

        public ApproveModel(IHttpClientService httpClient, IFileService fileService, INotyfService notyf)
        {
            _httpClient = httpClient;
            _fileService = fileService;
            _notyf = notyf;
        }
        [FromRoute] public int id { get; set; }
        [BindProperty] public TempNewsDTO TempDTO { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var tempNewProjectResult = await _httpClient.GetAsync("TempNews/Get", true, id);
            if (tempNewProjectResult == "unauthorized")
            {
                _notyf.Information("Please login/register");
                return RedirectToPage("/Account/Login");
            }
            TempDTO = !string.IsNullOrEmpty(tempNewProjectResult) ? JsonConvert.DeserializeObject<TempNewsDTO>(tempNewProjectResult) : null;
            if (TempDTO == null)
            {
                _notyf.Information("Record not found");
                return RedirectToPage("Pending");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostApprove()
        {
            if (TempDTO.EnglishFile != null)
            { //...........Check Valid File ...................
                if (!_fileService.CheckValidFile(TempDTO.EnglishFile))
                {
                    //......Redirect  
                    _notyf.Information("Please Upload Valid File");
                    return RedirectToPage("Index");
                }
                TempDTO.EnglishAttachment = await _fileService.SaveImageAsync(@"\img\UploadedFiles\News\Images\", TempDTO.EnglishFile);
                TempDTO.EnglishFile = null;
            }
            if (TempDTO.HindiFile != null)
            {//...........Check Valid File ...................
                if (!_fileService.CheckValidFile(TempDTO.HindiFile))
                {
                    //......Redirect  
                    _notyf.Information("Please Upload Valid File");
                    return RedirectToPage("Index");
                }
                TempDTO.HindiAttachment = await _fileService.SaveImageAsync(@"\img\UploadedFiles\News\Images\", TempDTO.HindiFile);
                TempDTO.HindiFile = null;
            }
            var NewProject = new NewsDTO
            {
                Id = TempDTO.RowId ?? 0,
                ParentId = TempDTO.ParentId,
                HindiHeadingName = TempDTO.HindiHeadingName,
                EnglishHeadingName = TempDTO.EnglishHeadingName,
                EnglishPageLink = TempDTO.EnglishPageLink,
                HindiPageLink = TempDTO.HindiPageLink,
                HindiContentDesc = TempDTO.HindiContentDesc,
                EnglishContentDesc = TempDTO.EnglishContentDesc,
                EnglishAttachment = TempDTO.EnglishAttachment,
                HindiAttachment = TempDTO.HindiAttachment,
                Priority = TempDTO.Priority,
                Show = TempDTO.Show,

            };
            if (TempDTO.Action == "Create")
            {
                if (NewProject.ParentId == 0)
                {
                    NewProject.ParentId = null;
                }
                var createResponse = await _httpClient.PostAsync("News/Create", true, NewProject);
                if (createResponse == "unauthorized")
                {
                    _notyf.Information("Please login/register");
                    return RedirectToPage("/Account/Login");
                }
                var createResult = !string.IsNullOrEmpty(createResponse) ? JsonConvert.DeserializeObject<NewsDTO>(createResponse) : null;
                if (createResult == null)
                {
                    _notyf.Error("Create failed");
                    return RedirectToPage("Pending");
                }
                var tempModeldto = SetAudit(TempDTO.Action, "Approved", createResult);
                //_notyf.Success("Saved successfully");
                return await CreateAudit(tempModeldto);
            }

            if (TempDTO.Action == "Edit")
            {
                var editResponse = await _httpClient.PutAsync("News/Edit", true, NewProject.Id, NewProject);
                if (editResponse == "unauthorized")
                {
                    _notyf.Information("Please login/register");
                    return RedirectToPage("/Account/Login");
                }
                var editResult = !string.IsNullOrEmpty(editResponse) ? JsonConvert.DeserializeObject<NewsDTO>(editResponse) : null;
                if (editResult == null)
                {
                    _notyf.Error("Edit failed");
                    return RedirectToPage("Pending");
                }
                var tempNewProject = SetAudit(TempDTO.Action, "Approved", editResult);
                _notyf.Success("Updated successfully");
                return await CreateAudit(tempNewProject);
            }

            if (TempDTO.Action == "Delete")
            {
                var deleteResponse = await _httpClient.DeleteAsync("News/Delete", true, NewProject.Id);
                if (deleteResponse == "unauthorized")
                {
                    _notyf.Information("Please login/register");
                    return RedirectToPage("/Account/Login");
                }
                var RowsChanged = !string.IsNullOrEmpty(deleteResponse) && Convert.ToInt32(deleteResponse) > 0;
                if (!RowsChanged)
                {
                    _notyf.Error("Delete failed");
                    return RedirectToPage("Pending");
                }
                var tempNewProject = SetAudit(TempDTO.Action, "Approved", NewProject);
                _notyf.Success("Deleted successfully");
                return await CreateAudit(tempNewProject);
            }
            _notyf.Error("Action not specified");
            return RedirectToPage("Pending");

        }
        public async Task<IActionResult> OnPostReject()
        {
            var NewProject = new NewsDTO
            {
                Id = TempDTO.RowId ?? 0,
                ParentId = TempDTO.ParentId,
                HindiHeadingName = TempDTO.HindiHeadingName,
                EnglishHeadingName = TempDTO.EnglishHeadingName,
                EnglishPageLink = TempDTO.EnglishPageLink,
                HindiPageLink = TempDTO.HindiPageLink,
                HindiContentDesc = TempDTO.HindiContentDesc,
                EnglishContentDesc = TempDTO.EnglishContentDesc,
                EnglishAttachment = TempDTO.EnglishAttachment,
                HindiAttachment = TempDTO.HindiAttachment,
                Priority = TempDTO.Priority,
            };
            var tempNewProject = SetAudit(TempDTO.Action, "Rejected", NewProject);
            return await CreateAudit(tempNewProject);
        }

        private TempNewsDTO SetAudit(string action, string status, NewsDTO model)
        {
            var uname = User.Identity.Name;
            var role = User.Claims.First(c => c.Type == ClaimTypes.Role).Value;
            return new TempNewsDTO
            {
                HindiHeadingName = TempDTO.HindiHeadingName,
                EnglishHeadingName = TempDTO.EnglishHeadingName,
                EnglishPageLink = TempDTO.EnglishPageLink,
                HindiPageLink = TempDTO.HindiPageLink,
                HindiContentDesc = TempDTO.HindiContentDesc,
                EnglishContentDesc = TempDTO.EnglishContentDesc,
                EnglishAttachment = TempDTO.EnglishAttachment,
                HindiAttachment = TempDTO.HindiAttachment,
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
            };
        }

        private async Task<IActionResult> CreateAudit(TempNewsDTO tempmodelDto)
        {
            if (id != 0)
            {
                tempmodelDto.Id = id;
                tempmodelDto.Show = false;
                var example = await _httpClient.PutAsync("TempNews/Edit", true, id, tempmodelDto);
            }
            tempmodelDto.Show = true;
            tempmodelDto.Id = 0;
            var tempResponse = await _httpClient.PostAsync("TempNews/CreateAudit", true, tempmodelDto);
            if (tempResponse == "unauthorized")
            {
                _notyf.Information("Please login/register");
                return RedirectToPage("/Account/Login");
            }
            var tempResult = !string.IsNullOrEmpty(tempResponse) ? JsonConvert.DeserializeObject<TempNewsDTO>(tempResponse) : null;
            if (tempResult == null)
                _notyf.Error("Save failed");
            else
                _notyf.Success("Saved successfully");
            return RedirectToPage("Pending");
        }
    }
}