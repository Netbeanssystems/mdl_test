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


namespace WebBank.Pages.Admin.Video
{
    [Authorize(Roles = "SuperAdmin,Editors,Admin,ContentCreator,Moderator,Publisher")]

    public class ManageModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly IFileService _fileService;
        private readonly INotyfService _notyf;

        public ManageModel(IHttpClientService httpClient, IFileService fileService, INotyfService notyf)
        {
            _httpClient = httpClient;
            _fileService = fileService;
            _notyf = notyf;
        }

        [FromRoute] public int? id { get; set; }
        [BindProperty] public VideoDTO modelDTO { get; set; }
        public bool IsNew => modelDTO == null;

        public async Task<IActionResult> OnGet()
        {
            if (!id.HasValue) return Page();

            var Result = await _httpClient.GetAsync("Video/Get", true, (int)id);
            if (Result == "unauthorized")
            {
                _notyf.Information("Please login/register");
                return RedirectToPage("/Account/Login");
            }
            modelDTO = !string.IsNullOrEmpty(Result) ? JsonConvert.DeserializeObject<VideoDTO>(Result) : null;
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            var tempNewProject = !id.HasValue || IsNew ? SetAudit("Create", "Pending", modelDTO) : SetAudit("Edit", "Pending", modelDTO);
            return await CreateAudit(tempNewProject);
        }

        public async Task<IActionResult> OnPostDelete(int Id)
        {
            var newProjectResult = await _httpClient.GetAsync("Video/Get", true, Id);
            if (newProjectResult == "unauthorized")
            {
                _notyf.Information("Please login/register");
                return RedirectToPage("/Account/Login");
            }

            var newProjectDto = !string.IsNullOrEmpty(newProjectResult) ? JsonConvert.DeserializeObject<VideoDTO>(newProjectResult) : null;
            if (newProjectDto == null)
            {
                _notyf.Information("New Project not found");
                return RedirectToPage(new { id = Id });
            }

            var tempNewProject = SetAudit("Delete", "Pending", modelDTO);

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
                EnglishLink = model.EnglishLink,
                HindiLink = model.HindiLink,
                EnglishThumbnailLink = model.EnglishThumbnailLink,
                HindThumbnailLink = model.HindThumbnailLink,
                EnglishCategory = model.EnglishCategory,
                HindiCategory = model.HindiCategory,
                Action = action,
                ActionDate = DateTime.UtcNow,
                UserName = uname,
                RoleName = role,
                Status = status,
                Active = true,
                IP = HttpContext.Connection.RemoteIpAddress.ToString(),
                RowId = !action.Equals("Create", StringComparison.InvariantCultureIgnoreCase) ? id : null
            };
        }

        private async Task<IActionResult> CreateAudit(TempVideoDTO modelDto)
        {
            var x = ModelState.IsValid;
            var tempvideoResult = await _httpClient.PostAsync("TempVideo/CreateAudit", true, modelDto);
            if (tempvideoResult == "unauthorized")
            {
                _notyf.Information("Please login/register");
                return RedirectToPage("/Account/Login");
            }

            var Tempvideo = !string.IsNullOrEmpty(tempvideoResult) ? JsonConvert.DeserializeObject<TempVideoDTO>(tempvideoResult) : null;
            if (Tempvideo == null)
                _notyf.Error("Save failed");
            else
                _notyf.Success("Saved successfully");
            return RedirectToPage("Index");
        }
    }
}
