using Application.Dtos;
using Application.ServiceInterfaces;
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
    public class ApproveModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly IFileService _fileService;
        private readonly ICommon _common;
        public ApproveModel(IHttpClientService httpClient, IFileService fileService, ICommon common)
        {
            _httpClient = httpClient;
            _fileService = fileService;
            _common = common;

        }
        [FromRoute] public int id { get; set; }
        [BindProperty] public TempWhatsNewDTO TempWhatsDTO { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var tempNewProjectResult = await _httpClient.GetAsync("TempWhatsNew/Get", true, id);
            if (tempNewProjectResult == "unauthorized")
            {
                TempData["Message"] = "info^Please login/register";
                return RedirectToPage("/Account/Login");
            }
            TempWhatsDTO = !string.IsNullOrEmpty(tempNewProjectResult) ? JsonConvert.DeserializeObject<TempWhatsNewDTO>(tempNewProjectResult) : null;
            if (TempWhatsDTO == null)
            {
                TempData["Message"] = "info^Record not found";
                return RedirectToPage("Pending");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostApprove()
        {
            if (TempWhatsDTO.EnglishFile != null)
            { //...........Check Valid File ...................
                if (!_common.CheckValidFile(TempWhatsDTO.EnglishFile))
                {
                    //......Redirect  
                    TempData["Message"] = "info^Please Upload Valid File";
                    return RedirectToPage("Index");
                }
                TempWhatsDTO.EnglishAttachment = await _fileService.SaveImageAsync(@"\img\UploadedFiles\WhatsNew\Images\", TempWhatsDTO.EnglishFile);
                TempWhatsDTO.EnglishFile = null;
            }
            if (TempWhatsDTO.HindiFile != null)
            {//...........Check Valid File ...................
                if (!_common.CheckValidFile(TempWhatsDTO.HindiFile))
                {
                    //......Redirect  
                    TempData["Message"] = "info^Please Upload Valid File";
                    return RedirectToPage("Index");
                }
                TempWhatsDTO.HindiAttachment = await _fileService.SaveImageAsync(@"\img\UploadedFiles\WhatsNew\Images\", TempWhatsDTO.HindiFile);
                TempWhatsDTO.HindiFile = null;
            }
            var NewProject = new WhatsNewDTO
            {
                Id = TempWhatsDTO.RowId ?? 0,
                ParentId = TempWhatsDTO.ParentId,
                HindiHeadingName = TempWhatsDTO.HindiHeadingName,
                EnglishHeadingName = TempWhatsDTO.EnglishHeadingName,
                EnglishPageLink = TempWhatsDTO.EnglishPageLink,
                HindiPageLink = TempWhatsDTO.HindiPageLink,
                HindiContentDesc = TempWhatsDTO.HindiContentDesc,
                EnglishContentDesc = TempWhatsDTO.EnglishContentDesc,
                EnglishAttachment = TempWhatsDTO.EnglishAttachment,
                HindiAttachment = TempWhatsDTO.HindiAttachment,
                Priority = TempWhatsDTO.Priority,
                Show = TempWhatsDTO.Show,
                SubmitDate = System.DateTime.Now,
                UpdateDate = TempWhatsDTO.UpdateDate,
                PublishDate = TempWhatsDTO.PublishDate,

            };
            if (TempWhatsDTO.Action == "Create")
            {
                if (NewProject.ParentId == 0)
                {
                    NewProject.ParentId = null;
                }
                var createResponse = await _httpClient.PostAsync("WhatsNew/Create", true, NewProject);
                if (createResponse == "unauthorized")
                {
                    TempData["Message"] = "info^Please login/register";
                    return RedirectToPage("/Account/Login");
                }
                var createResult = !string.IsNullOrEmpty(createResponse) ? JsonConvert.DeserializeObject<WhatsNewDTO>(createResponse) : null;
                if (createResult == null)
                {
                    TempData["Message"] = "danger^Create failed";
                    return RedirectToPage("Pending");
                }
                var tempModeldto = SetAudit(TempWhatsDTO.Action, "Approved", createResult);
                return await CreateAudit(tempModeldto);
            }

            if (TempWhatsDTO.Action == "Edit")
            {
                var editResponse = await _httpClient.PutAsync("WhatsNew/Edit", true, NewProject.Id, NewProject);
                if (editResponse == "unauthorized")
                {
                    TempData["Message"] = "info^Please login/register";
                    return RedirectToPage("/Account/Login");
                }
                var editResult = !string.IsNullOrEmpty(editResponse) ? JsonConvert.DeserializeObject<WhatsNewDTO>(editResponse) : null;
                if (editResult == null)
                {
                    TempData["Message"] = "danger^Edit failed";
                    return RedirectToPage("Pending");
                }
                var tempNewProject = SetAudit(TempWhatsDTO.Action, "Approved", editResult);
                return await CreateAudit(tempNewProject);
            }

            if (TempWhatsDTO.Action == "Delete")
            {
                var deleteResponse = await _httpClient.DeleteAsync("WhatsNew/Delete", true, NewProject.Id);
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
                var tempNewProject = SetAudit(TempWhatsDTO.Action, "Approved", NewProject);
                return await CreateAudit(tempNewProject);
            }
            TempData["Message"] = "danger^Action not specified";
            return RedirectToPage("Pending");

        }
        public async Task<IActionResult> OnPostReject()
        {
            var NewProject = new WhatsNewDTO
            {
                Id = TempWhatsDTO.RowId ?? 0,
                ParentId = TempWhatsDTO.ParentId,
                HindiHeadingName = TempWhatsDTO.HindiHeadingName,
                EnglishHeadingName = TempWhatsDTO.EnglishHeadingName,
                EnglishPageLink = TempWhatsDTO.EnglishPageLink,
                HindiPageLink = TempWhatsDTO.HindiPageLink,
                HindiContentDesc = TempWhatsDTO.HindiContentDesc,
                EnglishContentDesc = TempWhatsDTO.EnglishContentDesc,
                EnglishAttachment = TempWhatsDTO.EnglishAttachment,
                HindiAttachment = TempWhatsDTO.HindiAttachment,
                Priority = TempWhatsDTO.Priority,
            };
            var tempNewProject = SetAudit(TempWhatsDTO.Action, "Rejected", NewProject);
            return await CreateAudit(tempNewProject);
        }

        private TempWhatsNewDTO SetAudit(string action, string status, WhatsNewDTO model)
        {
            var uname = User.Identity.Name;
            var role = User.Claims.First(c => c.Type == ClaimTypes.Role).Value;
            return new TempWhatsNewDTO
            {
                HindiHeadingName = TempWhatsDTO.HindiHeadingName,
                EnglishHeadingName = TempWhatsDTO.EnglishHeadingName,
                EnglishPageLink = TempWhatsDTO.EnglishPageLink,
                HindiPageLink = TempWhatsDTO.HindiPageLink,
                HindiContentDesc = TempWhatsDTO.HindiContentDesc,
                EnglishContentDesc = TempWhatsDTO.EnglishContentDesc,
                EnglishAttachment = TempWhatsDTO.EnglishAttachment,
                HindiAttachment = TempWhatsDTO.HindiAttachment,
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

        private async Task<IActionResult> CreateAudit(TempWhatsNewDTO tempmodelDto)
        {
            if (id != 0)
            {
                tempmodelDto.Id = id;
                tempmodelDto.Show = false;
                var example = await _httpClient.PutAsync("TempWhatsNew/Edit", true, id, tempmodelDto);
            }
            tempmodelDto.Show = true;
            tempmodelDto.Id = 0;
            var tempResponse = await _httpClient.PostAsync("TempWhatsNew/CreateAudit", true, tempmodelDto);
            if (tempResponse == "unauthorized")
            {
                TempData["Message"] = "info^Please login/register";
                return RedirectToPage("/Account/Login");
            }
            var tempResult = !string.IsNullOrEmpty(tempResponse) ? JsonConvert.DeserializeObject<TempWhatsNewDTO>(tempResponse) : null;
            TempData["Message"] = tempResult == null ? "danger^Operation failed" : "success^Operation successfull";
            return RedirectToPage("Pending");
        }
    }
}