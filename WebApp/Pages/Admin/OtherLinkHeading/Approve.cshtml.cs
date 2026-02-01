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
        [BindProperty] public TempOtherLinkHeadingDTO TempDTO { get; set; }

        public async Task<IActionResult> OnGet()
        {
            //var tempResult = await _httpClient.GetAsync("TempOtherLinkHeading/Get", true, id);
            var tempResult = await _httpClient.GetAsync("TempOtherLink/Get", true, id);
            if (tempResult == "unauthorized")
            {
                _notyf.Information("Please login/register");
                return RedirectToPage("/Account/Login");
            }
            TempDTO = !string.IsNullOrEmpty(tempResult) ? JsonConvert.DeserializeObject<TempOtherLinkHeadingDTO>(tempResult) : null;
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
            {
                //...........Check Valid File ...................
                if (!_fileService.CheckValidFile(TempDTO.EnglishFile))
                {
                    //......Redirect  
                    _notyf.Information("Please Upload Valid File");
                    return RedirectToPage("Index");
                }
                TempDTO.EnglishAttachment = await _fileService.SaveImageAsync(@"\img\UploadedFiles\OtherLinkHeading\Files\English\", TempDTO.EnglishFile);
                TempDTO.EnglishFile = null;
            }
            if (TempDTO.HindiFile != null)
            {
                //...........Check Valid File ...................
                if (!_fileService.CheckValidFile(TempDTO.HindiFile))
                {
                    //......Redirect  
                    _notyf.Information("Please Upload Valid File");
                    return RedirectToPage("Index");
                }
                TempDTO.HindiAttachment = await _fileService.SaveImageAsync(@"\img\UploadedFiles\OtherLinkHeading\Files\Hindi\", TempDTO.HindiFile);
                TempDTO.HindiFile = null;
            }
            var modelDto = new OtherLinkHeadingDTO
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
                Title = TempDTO.Title,
                HindiTitle = TempDTO.HindiTitle,
                Description = TempDTO.Description,
                Keyword = TempDTO.Keyword,
                KeywordHindi = TempDTO.KeywordHindi,
                DescriptionHindi = TempDTO.KeywordHindi,
                UpdateDate = DateTime.Now,
            };

            if (TempDTO.Action == "Create")
            {
                var createResponse = await _httpClient.PostAsync("OtherLinkHeading/Create", true, modelDto);
                if (createResponse == "unauthorized")
                {
                    _notyf.Information("Please login/register");
                    return RedirectToPage("/Account/Login");
                }
                var createResult = !string.IsNullOrEmpty(createResponse) ? JsonConvert.DeserializeObject<OtherLinkHeadingDTO>(createResponse) : null;
                if (createResult == null)
                {
                    _notyf.Error("Create failed");
                    return RedirectToPage("Pending");
                }
                var tempModeldto = SetAudit(TempDTO.Action, "Approved", createResult);
                return await CreateAudit(tempModeldto);
            }

            if (TempDTO.Action == "Edit")
            {
                var editResponse = await _httpClient.PutAsync("OtherLinkHeading/Edit", true, modelDto.Id, modelDto);
                if (editResponse == "unauthorized")
                {
                    _notyf.Information("Please login/register");
                    return RedirectToPage("/Account/Login");
                }
                var editResult = !string.IsNullOrEmpty(editResponse) ? JsonConvert.DeserializeObject<OtherLinkHeadingDTO>(editResponse) : null;
                if (editResult == null)
                {
                    _notyf.Error("Edit failed");
                    return RedirectToPage("Pending");
                }
                var tempModelDto = SetAudit(TempDTO.Action, "Approved", editResult);
                return await CreateAudit(tempModelDto);
            }

            if (TempDTO.Action == "Delete")
            {
                var deleteResponse = await _httpClient.DeleteAsync("OtherLinkHeading/Delete", true, modelDto.Id);
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
                var tempModelDto = SetAudit(TempDTO.Action, "Approved", modelDto);
                return await CreateAudit(tempModelDto);
            }
            _notyf.Error("Action not specified");
            return RedirectToPage("Pending");
        }

        public async Task<IActionResult> OnPostReject()
        {
            var ModelDTO = new OtherLinkHeadingDTO
            {
                Id = TempDTO.RowId ?? 0,
                HindiHeadingName = TempDTO.HindiHeadingName,
                EnglishHeadingName = TempDTO.EnglishHeadingName,
                EnglishPageLink = TempDTO.EnglishPageLink,
                HindiPageLink = TempDTO.HindiPageLink,
                HindiContentDesc = TempDTO.HindiContentDesc,
                EnglishContentDesc = TempDTO.EnglishContentDesc,
                EnglishAttachment = TempDTO.EnglishAttachment,
                HindiAttachment = TempDTO.HindiAttachment,
                Priority = TempDTO.Priority,
                Title = TempDTO.Title,
                HindiTitle = TempDTO.HindiTitle,
                Description = TempDTO.Description,
                Keyword = TempDTO.Keyword,
                KeywordHindi = TempDTO.KeywordHindi,
                DescriptionHindi = TempDTO.KeywordHindi,
                UpdateDate = DateTime.Now,
            };
            var tempModelDTO = SetAudit(TempDTO.Action, "Rejected", ModelDTO);
            return await CreateAudit(tempModelDTO);
        }

        private TempOtherLinkHeadingDTO SetAudit(string action, string status, OtherLinkHeadingDTO model)
        {
            var uname = User.Identity.Name;
            var role = User.Claims.First(c => c.Type == ClaimTypes.Role).Value;
            return new TempOtherLinkHeadingDTO
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
                Title = TempDTO.Title,
                HindiTitle = TempDTO.HindiTitle,
                Description = TempDTO.Description,
                Keyword = TempDTO.Keyword,
                KeywordHindi = TempDTO.KeywordHindi,
                DescriptionHindi = TempDTO.DescriptionHindi,
                UpdateDate = DateTime.Now,
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

        private async Task<IActionResult> CreateAudit(TempOtherLinkHeadingDTO modelDto)
        {
            if (id != 0)
            {
                modelDto.Id = id;
                modelDto.Show = false;
                //var example = await _httpClient.PutAsync("TempOtherLinkHeading/Edit", true, id, modelDto);
                var example = await _httpClient.PutAsync("TempOtherLink/Edit", true, id, modelDto);
            }
            modelDto.Show = true;
            modelDto.Id = 0;
            //var tempResponse = await _httpClient.PostAsync("TempOtherLinkHeading/CreateAudit", true, modelDto);
            var tempResponse = await _httpClient.PostAsync("TempOtherLink/CreateAudit", true, modelDto);
            if (tempResponse == "unauthorized")
            {
                _notyf.Information("Please login/register");
                return RedirectToPage("/Account/Login");
            }
            var tempResult = !string.IsNullOrEmpty(tempResponse) ? JsonConvert.DeserializeObject<TempOtherLinkHeadingDTO>(tempResponse) : null;
            if (tempResult == null)
                _notyf.Error("Save failed");
            else
                _notyf.Success("Saved successfully");
            return RedirectToPage("Pending");
        }
    }
}