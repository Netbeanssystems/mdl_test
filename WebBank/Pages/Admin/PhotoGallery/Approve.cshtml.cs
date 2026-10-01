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
using WebBank.Helpers;

namespace WebBank.Pages.Admin.PhotoGallery
{
    [Authorize(Roles = "SuperAdmin,Editors,Admin,ContentCreator,Moderator,Publisher")]
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
        [BindProperty] public TempPhotoGalleryDTO Tempvideo { get; set; }

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

            var tempNewProjectResult = await _httpClient.GetAsync("TempPhotoGallery/Get", true, id);
            if (tempNewProjectResult == "unauthorized")
            {
                TempData["Message"] = "info^Please login/register";
                return RedirectToPage("/Account/Login");
            }
            Tempvideo = !string.IsNullOrEmpty(tempNewProjectResult) ? JsonConvert.DeserializeObject<TempPhotoGalleryDTO>(tempNewProjectResult) : null;
            if (Tempvideo == null)
            {
                TempData["Message"] = "info^Record not found";
                return RedirectToPage("Pending");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostApprove()
        {
            //if (Tempvideo.EnglishfileVideo != null)
            //{ //...........Check Valid File ...................
            //    if (!_common.CheckValidFile(Tempvideo.EnglishfileVideo))
            //    {
            //        //......Redirect  
            //        TempData["Message"] = "info^Please Upload Valid File";
            //        return RedirectToPage("Index");
            //    }
            //    Tempvideo.ShowEnglishImage = Tempvideo.EnglishfileVideo.FileName;
            //    Tempvideo.EnglishImage = await _fileService.SaveImageAsync(@"\img\UploadedFiles\Videos\PhotoGalleryImage\", Tempvideo.EnglishfileVideo);
            //    Tempvideo.EnglishfileVideo = null;
            //}
            //if (Tempvideo.HindiFileVideo != null)
            //{//...........Check Valid File ...................
            //    if (!_common.CheckValidFile(Tempvideo.HindiFileVideo))
            //    {
            //        //......Redirect  
            //        TempData["Message"] = "info^Please Upload Valid File";
            //        return RedirectToPage("Index");
            //    }
            //    Tempvideo.ShowHindiImage = Tempvideo.HindiFileVideo.FileName;
            //    Tempvideo.HindiImage = await _fileService.SaveImageAsync(@"\img\UploadedFiles\Videos\PhotoGalleryImage\", Tempvideo.HindiFileVideo);
            //    Tempvideo.HindiFileVideo = null;
            //}
            //if (Tempvideo.EnglishFilethumbnail != null)
            //{ //...........Check Valid File ...................
            //    if (!_common.CheckValidFile(Tempvideo.EnglishFilethumbnail))
            //    {
            //        //......Redirect  
            //        TempData["Message"] = "info^Please Upload Valid File";
            //        return RedirectToPage("Index");
            //    }
            //    Tempvideo.EnglishThumbnail = await _fileService.SaveImageAsync(@"\img\UploadedFiles\Videos\thumbnail\", Tempvideo.EnglishFilethumbnail);
            //    Tempvideo.EnglishFilethumbnail = null;
            //}
            //if (Tempvideo.Hindifilethumbnail != null)
            //{//...........Check Valid File ...................
            //    if (!_common.CheckValidFile(Tempvideo.Hindifilethumbnail))
            //    {
            //        //......Redirect  
            //        TempData["Message"] = "info^Please Upload Valid File";
            //        return RedirectToPage("Index");
            //    }
            //    Tempvideo.HindThumbnail = await _fileService.SaveImageAsync(@"\img\UploadedFiles\Videos\thumbnail\", Tempvideo.Hindifilethumbnail);
            //    Tempvideo.Hindifilethumbnail = null;
            //}

            if (Tempvideo.EnglishAttachmentfile != null)
            {
                //...........Check Valid File ...................
                if (!_common.CheckValidFile(Tempvideo.EnglishAttachmentfile))
                {
                    //......Redirect  
                    TempData["Message"] = "info^Please Upload Valid File";
                    return RedirectToPage("Index");
                }
                Tempvideo.EnglishAttachment = await _fileService.SaveImageAsync(@"\img\UploadedFiles\Videos\PhotoGalleryImage\", Tempvideo.EnglishAttachmentfile);
                Tempvideo.EnglishAttachmentfile = null;
            }

            if (Tempvideo.HindiAttachmentfile != null)
            {
                //...........Check Valid File ...................
                if (!_common.CheckValidFile(Tempvideo.HindiAttachmentfile))
                {
                    //......Redirect  
                    TempData["Message"] = "info^Please Upload Valid File";
                    return RedirectToPage("Index");
                }
                Tempvideo.HindiAttachment = await _fileService.SaveImageAsync(@"\img\UploadedFiles\Videos\PhotoGalleryImage\", Tempvideo.HindiAttachmentfile);
                Tempvideo.HindiAttachmentfile = null;
            }

            var NewProject = new PhotoGalleryDTO
            {
                Id = Tempvideo.RowId ?? 0,
                EventId = Tempvideo.EventId,
                EnglishHeading = Tempvideo.EnglishHeading,
                HindiHeading = Tempvideo.HindiHeading,
                ShowEnglishImage = Tempvideo.ShowEnglishImage,
                ShowHindiImage = Tempvideo.ShowHindiImage,
                EnglishDescp = Tempvideo.EnglishDescp,
                HindiDescp = Tempvideo.HindiDescp,
                EnglishAttachment = Tempvideo.EnglishAttachment,
                HindiAttachment = Tempvideo.HindiAttachment,
                Priority = Tempvideo.Priority,
                Active = Tempvideo.Active,
                CreateDate = DateTime.Now
            };
            if (Tempvideo.Action == "Create")
            {
                NewProject.Active = true;
                var createResponse = await _httpClient.PostAsync("PhotoGallery/Create", true, NewProject);
                if (createResponse == "unauthorized")
                {
                    TempData["Message"] = "info^Please login/register";
                    return RedirectToPage("/Account/Login");
                }
                var createResult = !string.IsNullOrEmpty(createResponse) ? JsonConvert.DeserializeObject<PhotoGalleryDTO>(createResponse) : null;
                if (createResult == null)
                {
                    TempData["Message"] = "danger^Create failed";
                    return RedirectToPage("Pending");
                }
                var tempNewProject = SetAudit(Tempvideo.Action, "Approved", createResult);
                return await CreateAudit(tempNewProject);
            }

            if (Tempvideo.Action == "Edit")
            {
                var editResponse = await _httpClient.PutAsync("PhotoGallery/Edit", true, NewProject.Id, NewProject);
                if (editResponse == "unauthorized")
                {
                    TempData["Message"] = "info^Please login/register";
                    return RedirectToPage("/Account/Login");
                }
                var editResult = !string.IsNullOrEmpty(editResponse) ? JsonConvert.DeserializeObject<PhotoGalleryDTO>(editResponse) : null;
                if (editResult == null)
                {
                    TempData["Message"] = "danger^Edit failed";
                    return RedirectToPage("Pending");
                }
                var tempNewProject = SetAudit(Tempvideo.Action, "Approved", editResult);
                return await CreateAudit(tempNewProject);
            }

            if (Tempvideo.Action == "Delete")
            {
                var deleteResponse = await _httpClient.DeleteAsync("PhotoGallery/Delete", true, NewProject.Id);
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
                var tempNewProject = SetAudit(Tempvideo.Action, "Approved", NewProject);
                return await CreateAudit(tempNewProject);
            }
            TempData["Message"] = "danger^Action not specified";
            return RedirectToPage("Pending");
        }

        public async Task<IActionResult> OnPostReject()
        {
            var NewProject = new PhotoGalleryDTO
            {
                Id = Tempvideo.RowId ?? 0,
                EnglishHeading = Tempvideo.EnglishHeading,
                HindiHeading = Tempvideo.HindiHeading,
                ShowEnglishImage = Tempvideo.ShowEnglishImage,
                ShowHindiImage = Tempvideo.ShowHindiImage,
                EnglishDescp = Tempvideo.EnglishDescp,
                HindiDescp = Tempvideo.HindiDescp,
                EnglishAttachment = Tempvideo.EnglishAttachment,
                HindiAttachment = Tempvideo.HindiAttachment,
                Priority = Tempvideo.Priority,
                Active = Tempvideo.Active,
                CreateDate = DateTime.Now
            };
            var tempNewProject = SetAudit(Tempvideo.Action, "Rejected", NewProject);
            return await CreateAudit(tempNewProject);
        }

        private TempPhotoGalleryDTO SetAudit(string action, string status, PhotoGalleryDTO model)
        {
            var uname = User.Identity.Name;
            var role = User.Claims.First(c => c.Type == ClaimTypes.Role).Value;
            return new TempPhotoGalleryDTO
            {
                EnglishHeading = Tempvideo.EnglishHeading,
                HindiHeading = Tempvideo.HindiHeading,
                EnglishImage = Tempvideo.EnglishImage,
                HindiImage = Tempvideo.HindiImage,
                ShowEnglishImage = Tempvideo.ShowEnglishImage,
                ShowHindiImage = Tempvideo.ShowHindiImage,
                EnglishDescp = Tempvideo.EnglishDescp,
                HindiDescp = Tempvideo.HindiDescp,
                EnglishText = Tempvideo.EnglishText,
                HindiText = Tempvideo.HindiText,
                EnglishAttachment = Tempvideo.EnglishAttachment,
                HindiAttachment = Tempvideo.HindiAttachment,
                Priority = Tempvideo.Priority,
                Action = action,
                ActionDate = DateTime.UtcNow,
                CreateDate = DateTime.Now,
                UserName = uname,
                RoleName = role,
                Status = status,
                Active = model.Active,
                IP = HttpContext.Connection.RemoteIpAddress.ToString(),
                RowId = model.Id == 0 ? (int?)null : model.Id
            };
        }

        private async Task<IActionResult> CreateAudit(TempPhotoGalleryDTO tempmodelDto)
        {
            if (id != 0)
            {
                tempmodelDto.Id = id;
                tempmodelDto.Active = false;
                var example = await _httpClient.PutAsync("TempPhotoGallery/Edit", true, id, tempmodelDto);
            }
            tempmodelDto.Active = true;
            tempmodelDto.Id = 0;
            var tempNewProjectResponse = await _httpClient.PostAsync("TempPhotoGallery/CreateAudit", true, tempmodelDto);
            if (tempNewProjectResponse == "unauthorized")
            {
                TempData["Message"] = "info^Please login/register";
                return RedirectToPage("/Account/Login");
            }
            var tempNewProjectResult = !string.IsNullOrEmpty(tempNewProjectResponse) ? JsonConvert.DeserializeObject<TempPhotoGalleryDTO>(tempNewProjectResponse) : null;
            TempData["Message"] = tempNewProjectResult == null ? "danger^Operation failed" : "success^Operation successfull";
            return RedirectToPage("Pending");
        }
    }
}
