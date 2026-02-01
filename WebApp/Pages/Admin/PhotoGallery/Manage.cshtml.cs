using Application.Dtos;
using Application.ServiceInterfaces;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApp.Helpers;

namespace WebApp.Pages.Admin.PhotoGallery
{
    [Authorize(Roles = "SuperAdmin,Editors,Admin,ContentCreator,Moderator,Publisher")]
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
        [BindProperty] public PhotoGalleryDTO modelDTO { get; set; }
        public bool IsNew => modelDTO == null;

        public async Task<IActionResult> OnGet()
        {
            //get all the Events for dropdown
            var events = await DataHelper.GetDropdown(_httpClient, "Events", false).ConfigureAwait(false);
            if (events == null || events.Count <= 0)
            {
                _notyf.Error("Events not found");
                return Page();
            }
            ViewData["events"] = new SelectList(events, "Id", "Text");

            if (!id.HasValue) return Page();

            var Result = await _httpClient.GetAsync("PhotoGallery/Get", true, (int)id);
            if (Result == "unauthorized")
            {
                TempData["Message"] = "info^Please login/register";
                return RedirectToPage("/Account/Login");
            }
            modelDTO = !string.IsNullOrEmpty(Result) ? JsonConvert.DeserializeObject<PhotoGalleryDTO>(Result) : null;
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (modelDTO.EnglishAttachmentfile != null)
            {
                //...........Check Valid File ...................
                if (!_common.CheckValidJpgFile(modelDTO.EnglishAttachmentfile))
                {
                    //......Redirect  
                    //TempData["Message"] = "info^Please Upload Valid Attachment File";
                    _notyf.Error("Please Upload Valid Attachment File");
                    return Page();
                }
                modelDTO.EnglishAttachment = await _fileService.SaveImageAsync(@"\img\UploadedFiles\EventPhotos\EnglishPhoto\", modelDTO.EnglishAttachmentfile);
                modelDTO.EnglishAttachmentfile = null;
            }

            if (modelDTO.HindiAttachmentfile != null)
            { //...........Check Valid File ...................
                if (!_common.CheckValidJpgFile(modelDTO.HindiAttachmentfile))
                {
                    //......Redirect  
                    _notyf.Error("Please Upload Valid Attachment File");
                    return Page();
                }
                modelDTO.HindiAttachment = await _fileService.SaveImageAsync(@"\img\UploadedFiles\EventPhotos\HindiPhoto\", modelDTO.HindiAttachmentfile);
                modelDTO.HindiAttachmentfile = null;
            }

            var tempNewProject = !id.HasValue || IsNew ? SetAudit("Create", "Pending", modelDTO) : SetAudit("Edit", "Pending", modelDTO);
            return await CreateAudit(tempNewProject);
        }

        public async Task<IActionResult> OnPostDelete(int Id)
        {
            var newProjectResult = await _httpClient.GetAsync("PhotoGallery/Get", true, Id);
            if (newProjectResult == "unauthorized")
            {
                TempData["Message"] = "info^Please login/register";
                return RedirectToPage("/Account/Login");
            }

            var newProjectDto = !string.IsNullOrEmpty(newProjectResult) ? JsonConvert.DeserializeObject<PhotoGalleryDTO>(newProjectResult) : null;
            if (newProjectDto == null)
            {
                TempData["Message"] = "info^New Project not found";
                return RedirectToPage(new { id = Id });
            }

            var tempNewProject = SetAudit("Delete", "Pending", newProjectDto);

            return await CreateAudit(tempNewProject);
        }

        private TempPhotoGalleryDTO SetAudit(string action, string status, PhotoGalleryDTO model)
        {
            var uname = User.Identity.Name;
            var role = User.Claims.First(c => c.Type == ClaimTypes.Role).Value;
            return new TempPhotoGalleryDTO
            {
                EventId = model.EventId,
                EnglishHeading = model.EnglishHeading,
                EnglishAttachment = model.EnglishAttachment,
                HindiAttachment = model.HindiAttachment,
                HindiHeading = model.HindiHeading,
                ShowEnglishImage = model.ShowEnglishImage,
                ShowHindiImage = model.ShowHindiImage,
                EnglishDescp = model.EnglishDescp,
                HindiDescp = model.HindiDescp,
                Priority = model.Priority,
                Action = action,
                ActionDate = DateTime.UtcNow,
                CreateDate = DateTime.Now,
                UserName = uname,
                RoleName = role,
                Status = status,
                Active = true,
                IP = HttpContext.Connection.RemoteIpAddress.ToString(),
                RowId = !action.Equals("Create", StringComparison.InvariantCultureIgnoreCase) ? id : null
            };
        }

        private async Task<IActionResult> CreateAudit(TempPhotoGalleryDTO modelDto)
        {
            var x = ModelState.IsValid;
            var tempvideoResult = await _httpClient.PostAsync("TempPhotoGallery/CreateAudit", true, modelDto);
            if (tempvideoResult == "unauthorized")
            {
                TempData["Message"] = "info^Please login/register";
                return RedirectToPage("/Account/Login");
            }

            var Tempvideo = !string.IsNullOrEmpty(tempvideoResult) ? JsonConvert.DeserializeObject<TempPhotoGalleryDTO>(tempvideoResult) : null;

            if (Tempvideo == null)
            {
                _notyf.Error("Save failed");
            }
            else
            {
                _notyf.Success("Saved for approval");
            }
            //TempData["Message"] = Tempvideo == null ? "danger^Save failed" : "success^Saved for approval";
            return RedirectToPage("Index");
        }
    }
}
