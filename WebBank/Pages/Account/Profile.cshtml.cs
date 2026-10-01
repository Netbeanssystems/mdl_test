using Application.Dtos;
using Application.ServiceInterfaces;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Threading.Tasks;
using WebBank.Extensions;
using WebBank.Helpers;

namespace WebBank.Pages.Account
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        Validate objbal = new Validate();
        private readonly IHttpClientService _httpClient;
        private readonly INotyfService _notyf;
        public ProfileModel(
            IHttpClientService httpClient,
            INotyfService notyf)
        {
            _httpClient = httpClient;
            _notyf = notyf;
        }

        public string uid => DataHelper.GetUserId(User);
        [BindProperty] public UserProfileDTO UserProfileDto { get; set; }
        public async Task<IActionResult> OnGet()
        {
            var result = await _httpClient.GetAsync("Users/UserById", true, uid).ConfigureAwait(false);
            if (result == "unauthorized") return RedirectToPage("/account/login");
            UserProfileDto = !string.IsNullOrEmpty(result) ? JsonConvert.DeserializeObject<UserProfileDTO>(result) : null;
            if (UserProfileDto != null) return Page();
            _notyf.Error("User not found");
            return RedirectToPage("/Index");
        }
        public async Task<IActionResult> OnPost()
        {
            UserProfileDto.Id = uid;
            var UserUpdationResult = await _httpClient.PostMultipartAsync("Users/UpdateUser", true, UserProfileDto).ConfigureAwait(false);
            if (UserUpdationResult == "unauthorized") return RedirectToPage("/account/login");
            UserProfileDto = !string.IsNullOrEmpty(UserUpdationResult) ? JsonConvert.DeserializeObject<UserProfileDTO>(UserUpdationResult) : null;
            if (UserProfileDto == null)
                _notyf.Error("Update failed");
            else
                _notyf.Success("Updated successfully.");
            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostUploadFile()
        {
            var fileoldname = string.IsNullOrEmpty(Request.Form["FileOldName"].ToString()) ||
                                    Request.Form["FileOldName"].ToString() == "default_user100.png"
                                    ? string.Empty
                                    : Request.Form["FileOldName"].ToString();
            var FileUploadDto = new FileUploadDTO
            {
                ChangeName = true,
                ReturnValue = "name",
                UploadedFile = Request.Form.Files.GetFile("UserImage"),
                FilePath = "\\img\\users",
                FileOldName = fileoldname,
                ChangeDimensions = true,
                Width = 128,
                Height = 128
            };
            var FileUploadResult = await _httpClient.PostMultipartAsync("Files/UploadFile", true, FileUploadDto).ConfigureAwait(false);
            if (FileUploadResult == "unauthorized") return new JsonResult("unauthorized");
            var SavedFileName = !string.IsNullOrEmpty(FileUploadResult) ? JsonConvert.DeserializeObject<string>(FileUploadResult) : null;
            if (!string.IsNullOrEmpty(SavedFileName))
                HttpContext.Session.SetString("ProfileImage", SavedFileName);
            return new JsonResult(SavedFileName);
        }
        public async Task<IActionResult> OnPostDeleteFile(string fileoldname)
        {
            var FileUploadDto = new FileUploadDTO
            {
                FileOldName = string.IsNullOrEmpty(fileoldname) || fileoldname == "default_user100.png" ? string.Empty : fileoldname,
                FilePath = string.IsNullOrEmpty(fileoldname) || fileoldname == "default_user100.png" ? string.Empty : $"\\img\\users\\{fileoldname}"
            };
            var FileUploadResult = await _httpClient.PostAsync("Files/DeleteFile", true, FileUploadDto).ConfigureAwait(false);
            if (FileUploadResult == "unauthorized") return new JsonResult("unauthorized");
            HttpContext.Session.SetString("ProfileImage", "default_user100.png");
            return new JsonResult(FileUploadResult);
        }
    }
}