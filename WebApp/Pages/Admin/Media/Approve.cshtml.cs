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

namespace WebApp.Pages.Admin.Media
{
    [Authorize(Roles = "Administrators,Editors,SuperAdmin")]
    public class ApproveModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly IFileService _fileService;
        private readonly ICommon _common;
        private readonly INotyfService _notyf;

        public ApproveModel(IHttpClientService httpClient, IFileService fileService, ICommon common, INotyfService notyf)
        {
            _httpClient = httpClient;
            _fileService = fileService;
            _common = common;
            _notyf = notyf;
        }

        [FromRoute] public int id { get; set; }
        [BindProperty] public TempMediaDTO tempmedia { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var tempNewProjectResult = await _httpClient.GetAsync("TempMedia/Get", true, id);
            if (tempNewProjectResult == "unauthorized")
            {
                TempData["Message"] = "info^Please login/register";
                return RedirectToPage("/Account/Login");
            }
            tempmedia = !string.IsNullOrEmpty(tempNewProjectResult) ? JsonConvert.DeserializeObject<TempMediaDTO>(tempNewProjectResult) : null;
            if (tempmedia == null)
            {
                TempData["Message"] = "info^Record not found";
                return RedirectToPage("Pending");
            }
            return Page();
        }
        public async Task<IActionResult> OnPostApprove()
        {
            if (tempmedia.FileEnglish != null)
            { //...........Check Valid File ...................
                if (!_common.CheckValidFile(tempmedia.FileEnglish))
                {
                    //......Redirect  
                    TempData["Message"] = "info^Please Upload Valid File";
                    return RedirectToPage("Index");
                }
                tempmedia.EnglishFile = tempmedia.FileEnglish.FileName;
                tempmedia.EnglishImage = await _fileService.SaveImageAsync(@"\img\UploadedFiles\Media\File\EnglishImage\", tempmedia.FileEnglish);
                tempmedia.FileEnglish = null;
            }
            if (tempmedia.FileHindi != null)
            { //...........Check Valid File ...................
                if (!_common.CheckValidFile(tempmedia.FileHindi))
                {
                    //......Redirect  
                    TempData["Message"] = "info^Please Upload Valid File";
                    return RedirectToPage("Index");
                }
                tempmedia.HindiFile = tempmedia.FileHindi.FileName;
                tempmedia.HindiImage = await _fileService.SaveImageAsync(@"\img\UploadedFiles\Media\File\HindiImage\", tempmedia.FileHindi);
                tempmedia.FileHindi = null;
            }
            var NewProject = new MediaDTO
            {
                Id = tempmedia.RowId ?? 0,
                EnglishHeading = tempmedia.EnglishHeading,
                HindiHeading = tempmedia.HindiHeading,
                EnglishContent = tempmedia.EnglishContent,
                HindiContent = tempmedia.HindiContent,
                EnglishImage = tempmedia.EnglishImage,
                HindiImage = tempmedia.HindiImage,
                EnglishFile = tempmedia.EnglishFile,
                HindiFile = tempmedia.HindiFile,

                UploadedDate = tempmedia.ActionDate,
                Tag = tempmedia.Tag,
                HindiTag = tempmedia.HindiTag,

                Show = tempmedia.Show
            };
            if (tempmedia.Action == "Create")
            {
                NewProject.Show = true;
                var createResponse = await _httpClient.PostAsync("Media/Create", true, NewProject);
                if (createResponse == "unauthorized")
                {
                    TempData["Message"] = "info^Please login/register";
                    return RedirectToPage("/Account/Login");
                }
                var createResult = !string.IsNullOrEmpty(createResponse) ? JsonConvert.DeserializeObject<MediaDTO>(createResponse) : null;
                if (createResult == null)
                {
                    TempData["Message"] = "danger^Create failed";
                    return RedirectToPage("Pending");
                }
                var tempNewProject = SetAudit(tempmedia.Action, "Approved", createResult);
                return await CreateAudit(tempNewProject);
            }

            if (tempmedia.Action == "Edit")
            {
                var editResponse = await _httpClient.PutAsync("Media/Edit", true, NewProject.Id, NewProject);
                if (editResponse == "unauthorized")
                {
                    TempData["Message"] = "info^Please login/register";
                    return RedirectToPage("/Account/Login");
                }
                var editResult = !string.IsNullOrEmpty(editResponse) ? JsonConvert.DeserializeObject<MediaDTO>(editResponse) : null;
                if (editResult == null)
                {
                    TempData["Message"] = "danger^Edit failed";
                    return RedirectToPage("Pending");
                }
                var tempNewProject = SetAudit(tempmedia.Action, "Approved", editResult);
                return await CreateAudit(tempNewProject);
            }

            if (tempmedia.Action == "Delete")
            {
                var deleteResponse = await _httpClient.DeleteAsync("Media/Delete", true, NewProject.Id);
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
                var tempNewProject = SetAudit(tempmedia.Action, "Approved", NewProject);
                return await CreateAudit(tempNewProject);
            }
            TempData["Message"] = "danger^Action not specified";
            return RedirectToPage("Pending");
        }

        public async Task<IActionResult> OnPostReject()
        {
            var NewProject = new MediaDTO
            {
                Id = tempmedia.RowId ?? 0,
                EnglishHeading = tempmedia.EnglishHeading,
                HindiHeading = tempmedia.HindiHeading,
                EnglishContent = tempmedia.EnglishContent,
                HindiContent = tempmedia.HindiContent,
                EnglishImage = tempmedia.EnglishImage,
                HindiImage = tempmedia.HindiImage,
                EnglishFile = tempmedia.EnglishFile,
                HindiFile = tempmedia.HindiFile,

                UploadedDate = tempmedia.ActionDate,
                Tag = tempmedia.Tag,
                HindiTag = tempmedia.HindiTag,

                Show = tempmedia.Show
            };
            var tempNewProject = SetAudit(tempmedia.Action, "Rejected", NewProject);
            return await CreateAudit(tempNewProject);
        }

        private TempMediaDTO SetAudit(string action, string status, MediaDTO model)
        {
            var uname = User.Identity.Name;
            var role = User.Claims.First(c => c.Type == ClaimTypes.Role).Value;
            return new TempMediaDTO
            {
                EnglishHeading = model.EnglishHeading,
                HindiHeading = model.HindiHeading,
                EnglishContent = model.EnglishContent,
                HindiContent = model.HindiContent,
                EnglishImage = model.EnglishImage,
                HindiImage = model.HindiImage,
                EnglishFile = model.EnglishFile,
                HindiFile = model.HindiFile,
                Tag = model.Tag,
                HindiTag = model.HindiTag,

                Action = action,
                ActionDate = DateTime.UtcNow,
                UserName = uname,
                RoleName = role,
                Status = status,
                Show = model.Show,
                IP = HttpContext.Connection.RemoteIpAddress.ToString(),
                RowId = model.Id == 0 ? (int?)null : model.Id
            };
        }

        private async Task<IActionResult> CreateAudit(TempMediaDTO tempmodelDto)
        {
            if (id != 0)
            {
                tempmodelDto.Id = id;
                tempmodelDto.Show = false;
                var example = await _httpClient.PutAsync("TempMedia/Edit", true, id, tempmodelDto);
            }
            tempmodelDto.Show = true;
            tempmodelDto.Id = 0;
            var tempNewProjectResponse = await _httpClient.PostAsync("TempMedia/CreateAudit", true, tempmodelDto);
            if (tempNewProjectResponse == "unauthorized")
            {
                TempData["Message"] = "info^Please login/register";
                return RedirectToPage("/Account/Login");
            }
            var tempNewProjectResult = !string.IsNullOrEmpty(tempNewProjectResponse) ? JsonConvert.DeserializeObject<TempMediaDTO>(tempNewProjectResponse) : null;
            TempData["Message"] = tempNewProjectResult == null ? "danger^Operation failed" : "success^Operation successfull";

            _notyf.Success("Approved successfully");

            return RedirectToPage("Pending");
        }

    }
}
