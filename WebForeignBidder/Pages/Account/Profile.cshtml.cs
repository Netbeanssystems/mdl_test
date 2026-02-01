using Application.Dtos;
using Application.ServiceInterfaces;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebForeignBidder.Helpers;

namespace WebForeignBidder.Pages.Account
{
    [Authorize]
    public class ProfileModel : PageModel
    {
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
            if (User.IsInRole("CommercialExecutive") || User.IsInRole("BidderSuperAdmin"))
            {
               var ids = UserProfileDto.ProjectId;
                //get all the projects for dropdown
                var projectsResult = await _httpClient.GetAsync($"Projects/GetMultiple/{ids}", true).ConfigureAwait(false);
               // var projectsResult = await _httpClient.GetAsync("Projects/Get/{id}", true).ConfigureAwait(false);
                if (projectsResult == "unauthorized") return RedirectToPage("/Account/Login");
                var projects = !string.IsNullOrEmpty(projectsResult) ? JsonConvert.DeserializeObject<List<BidderProjectsDTO>>(projectsResult) : null;
                if (projects == null || projects.Count <= 0)
                {
                    _notyf.Error("Record not found");
                    return Page();
                }

                ViewData["Projects"] = new SelectList(projects, "Id", "ProjectName");

                var id = UserProfileDto.YardId;
                var yardsResult = await _httpClient.GetAsync($"Projects/GetYardsByProjectIds/{id}", true).ConfigureAwait(false);

                if (yardsResult == "unauthorized")
                    return RedirectToPage("/Account/Login");
                var yards = !string.IsNullOrEmpty(yardsResult)? JsonConvert.DeserializeObject<List<BidderYardsDTO>>(yardsResult): null;
                if (yards == null || yards.Count <= 0)
                {
                    _notyf.Error("Record not found");
                    return Page();
                }
                ViewData["Yards"] = new SelectList(yards, "Id", "YardNumber");



            }
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
        public async Task<JsonResult> OnGetGetYardsByProject(int projectId)
        {
            var yardsResult = await _httpClient.GetAsync("Projects/GetYard", true, projectId).ConfigureAwait(false);
            if (yardsResult == "unauthorized") return new JsonResult(new { error = "unauthorized" });

            var yards = !string.IsNullOrEmpty(yardsResult)
                ? JsonConvert.DeserializeObject<List<BidderYardsDTO>>(yardsResult)
                : new List<BidderYardsDTO>();
            if (yards == null || yards.Count <= 0) return new JsonResult(new { error = "Record Not Found" });

            return new JsonResult(yards);
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