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

namespace WebBank.Pages.Admin.PressRelease
{
    [Authorize(Roles = "Administrators,Editors,SuperAdmin")]
    public class ManageModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly IFileService _fileService;
        private readonly ICommon _common;
        private readonly INotyfService _notyf;
        public ManageModel(IHttpClientService httpClient, IFileService fileService, ICommon common, INotyfService notyf)
        {
            _httpClient = httpClient;
            _fileService = fileService;
            _common = common;
            _notyf = notyf;
        }

        [FromRoute] public int? id { get; set; }
        [BindProperty] public MediaDTO modelDTO { get; set; }
        public bool IsNew => modelDTO == null;

        public async Task<IActionResult> OnGet()
        {
            if (!id.HasValue) return Page();
            var Result = await _httpClient.GetAsync("Media/Get", true, (int)id);
            if (Result == "unauthorized")
            {
                TempData["Message"] = "info^Please login/register";
                return RedirectToPage("/Account/Login");
            }
            modelDTO = !string.IsNullOrEmpty(Result) ? JsonConvert.DeserializeObject<MediaDTO>(Result) : null;
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (modelDTO.FileEnglish != null)
            {
                //...........Check Valid File ...................
                if (!_common.CheckValidFile(modelDTO.FileEnglish))
                {
                    //......Redirect  
                    _notyf.Error("Please Upload Valid File");
                    return Page();
                }
                modelDTO.EnglishFile = modelDTO.FileEnglish.FileName;
                modelDTO.EnglishImage = await _fileService.SaveImageAsync(@"\img\UploadedFiles\Media\File\EnglishImage\", modelDTO.FileEnglish);
                modelDTO.FileEnglish = null;
            }

            if (modelDTO.FileHindi != null)
            { //...........Check Valid File ...................
                if (!_common.CheckValidFile(modelDTO.FileHindi))
                {
                    //......Redirect  
                    _notyf.Error("Please Upload Valid File");
                    return Page();
                }
                modelDTO.HindiFile = modelDTO.FileHindi.FileName;
                modelDTO.HindiImage = await _fileService.SaveImageAsync(@"\img\UploadedFiles\Media\File\HindiImage\", modelDTO.FileHindi);
                modelDTO.FileHindi = null;
            }
            var tempNewProject = !id.HasValue || IsNew ? SetAudit("Create", "Pending", modelDTO) : SetAudit("Edit", "Pending", modelDTO);

            _notyf.Success("Saved successfully");

            return await CreateAudit(tempNewProject);
        }

        public async Task<IActionResult> OnPostDelete(int Id)
        {
            var newProjectResult = await _httpClient.GetAsync("Media/Get", true, Id);
            if (newProjectResult == "unauthorized")
            {
                TempData["Message"] = "info^Please login/register";
                return RedirectToPage("/Account/Login");
            }

            var newProjectDto = !string.IsNullOrEmpty(newProjectResult) ? JsonConvert.DeserializeObject<MediaDTO>(newProjectResult) : null;
            if (newProjectDto == null)
            {
                TempData["Message"] = "info^New Project not found";
                return RedirectToPage(new { id = Id });
            }

            var tempNewProject = SetAudit("Delete", "Pending", newProjectDto);

            _notyf.Success("Delete requested successfully");

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
                ActionDate = (DateTime)model.UploadedDate,
                UserName = uname,
                RoleName = role,
                Status = status,
                Show = true,
                IP = HttpContext.Connection.RemoteIpAddress.ToString(),

                RowId = !action.Equals("Create", StringComparison.InvariantCultureIgnoreCase) ? id : null
            };
        }

        private async Task<IActionResult> CreateAudit(TempMediaDTO modelDto)
        {

            var tempNewProjectResult = await _httpClient.PostAsync("TempMedia/CreateAudit", true, modelDto);
            if (tempNewProjectResult == "unauthorized")
            {
                TempData["Message"] = "info^Please login/register";
                return RedirectToPage("/Account/Login");
            }

            var TempNewProject = !string.IsNullOrEmpty(tempNewProjectResult) ? JsonConvert.DeserializeObject<TempMediaDTO>(tempNewProjectResult) : null;
            TempData["Message"] = TempNewProject == null ? "danger^Save failed" : "success^Saved for approval";
            return RedirectToPage("Index");
        }
    }
}
