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


namespace WebApp.Pages.Admin.Video
{
    [Authorize(Roles = "SuperAdmin,Editors,Admin,ContentCreator,Moderator,Publisher")]

    public class ApproveModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly INotyfService _notyf;
        public ApproveModel(IHttpClientService httpClient, INotyfService notyf)
        {
            _httpClient = httpClient;
            _notyf = notyf;
        }

        [FromRoute] public int id { get; set; }
        [BindProperty] public TempVideoDTO TempDTO { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var tempNewProjectResult = await _httpClient.GetAsync("TempVideo/Get", true, id);
            if (tempNewProjectResult == "unauthorized")
            {
                _notyf.Information("Please login/register");
                return RedirectToPage("/Account/Login");
            }
            TempDTO = !string.IsNullOrEmpty(tempNewProjectResult) ? JsonConvert.DeserializeObject<TempVideoDTO>(tempNewProjectResult) : null;
            if (TempDTO == null)
            {
                _notyf.Information("Record not found");
                return RedirectToPage("Pending");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostApprove()
        {
            var NewProject = new VideoDTO
            {
                Id = TempDTO.RowId ?? 0,
                EnglishHeading = TempDTO.EnglishHeading,
                HindiHeading = TempDTO.HindiHeading,
                EnglishThumbnailLink = TempDTO.EnglishThumbnailLink,
                HindThumbnailLink = TempDTO.HindThumbnailLink,
                EnglishLink = TempDTO.EnglishLink,
                HindiLink = TempDTO.HindiLink,
                EnglishCategory = TempDTO.EnglishCategory,
                HindiCategory = TempDTO.HindiCategory,
                Active = TempDTO.Active
            };
            if (TempDTO.Action == "Create")
            {
                NewProject.Active = true;
                var createResponse = await _httpClient.PostAsync("Video/Create", true, NewProject);
                if (createResponse == "unauthorized")
                {
                    _notyf.Information("Please login/register");
                    return RedirectToPage("/Account/Login");
                }
                var createResult = !string.IsNullOrEmpty(createResponse) ? JsonConvert.DeserializeObject<VideoDTO>(createResponse) : null;
                if (createResult == null)
                {
                    _notyf.Error("Create failed");
                    return RedirectToPage("Pending");
                }
                var tempNewProject = SetAudit(TempDTO.Action, "Approved", createResult);
                return await CreateAudit(tempNewProject);
            }

            if (TempDTO.Action == "Edit")
            {
                var editResponse = await _httpClient.PutAsync("video/Edit", true, NewProject.Id, NewProject);
                if (editResponse == "unauthorized")
                {
                    _notyf.Information("Please login/register");
                    return RedirectToPage("/Account/Login");
                }
                var editResult = !string.IsNullOrEmpty(editResponse) ? JsonConvert.DeserializeObject<VideoDTO>(editResponse) : null;
                if (editResult == null)
                {
                    _notyf.Error("Edit failed");
                    return RedirectToPage("Pending");
                }
                var tempNewProject = SetAudit(TempDTO.Action, "Approved", editResult);
                return await CreateAudit(tempNewProject);
            }

            if (TempDTO.Action == "Delete")
            {
                var deleteResponse = await _httpClient.DeleteAsync("video/Delete", true, NewProject.Id);
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
                return await CreateAudit(tempNewProject);
            }
            _notyf.Error("Action not specified");
            return RedirectToPage("Pending");
        }


        public async Task<IActionResult> OnPostReject()
        {
            var NewProject = new VideoDTO
            {
                Id = TempDTO.RowId ?? 0,
                EnglishHeading = TempDTO.EnglishHeading,
                HindiHeading = TempDTO.HindiHeading,
                EnglishThumbnailLink = TempDTO.EnglishThumbnailLink,
                HindThumbnailLink = TempDTO.HindThumbnailLink,
                EnglishLink = TempDTO.EnglishLink,
                HindiLink = TempDTO.HindiLink,
                EnglishCategory = TempDTO.EnglishCategory,
                HindiCategory = TempDTO.HindiCategory,
                Active = TempDTO.Active
            };
            var tempNewProject = SetAudit(TempDTO.Action, "Rejected", NewProject);
            return await CreateAudit(tempNewProject);
        }

        private TempVideoDTO SetAudit(string action, string status, VideoDTO model)
        {
            var uname = User.Identity.Name;
            var role = User.Claims.First(c => c.Type == ClaimTypes.Role).Value;
            return new TempVideoDTO
            {
                EnglishHeading = model.EnglishHeading,
                HindiHeading = model.HindiHeading,
                EnglishThumbnailLink = TempDTO.EnglishThumbnailLink,
                HindThumbnailLink = TempDTO.HindThumbnailLink,
                HindiLink = model.HindiLink,
                EnglishCategory = model.EnglishCategory,
                HindiCategory = model.HindiCategory,
                Action = action,
                ActionDate = DateTime.UtcNow,
                UserName = uname,
                RoleName = role,
                Status = status,
                Active = model.Active,
                IP = HttpContext.Connection.RemoteIpAddress.ToString(),
                RowId = model.Id == 0 ? (int?)null : model.Id
            };
        }

        private async Task<IActionResult> CreateAudit(TempVideoDTO tempmodelDto)
        {
            if (id != 0)
            {
                tempmodelDto.Id = id;
                tempmodelDto.Active = false;
                var example = await _httpClient.PutAsync("TempVideo/Edit", true, id, tempmodelDto);
            }
            tempmodelDto.Active = true;
            tempmodelDto.Id = 0;
            var tempNewProjectResponse = await _httpClient.PostAsync("TempVideo/CreateAudit", true, tempmodelDto);
            if (tempNewProjectResponse == "unauthorized")
            {
                _notyf.Information("Please login/register");
                return RedirectToPage("/Account/Login");
            }
            var tempNewProjectResult = !string.IsNullOrEmpty(tempNewProjectResponse) ? JsonConvert.DeserializeObject<TempVideoDTO>(tempNewProjectResponse) : null;
            if (tempNewProjectResult == null)
                _notyf.Error("Save failed");
            else
                _notyf.Success("Saved successfully");
            return RedirectToPage("Pending");
        }

    }
}
