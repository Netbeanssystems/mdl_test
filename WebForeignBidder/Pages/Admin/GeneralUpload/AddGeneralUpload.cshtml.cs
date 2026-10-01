using Application.Dtos;
using Application.Helpers;
using Application.ServiceInterfaces;
using Application.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebForeignBidder.Pages.Admin.GeneralUpload
{
    [Authorize(Roles = "GeneralUpload,SuperAdmin")]
    public class AddGeneralUploadModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly IFileService _fileService;
        private readonly INotyfService _notyf;

        public AddGeneralUploadModel(IHttpClientService httpClient, IFileService fileService, INotyfService notyf)
        {
            _httpClient = httpClient;
            _fileService = fileService;
            _notyf = notyf;
        }


        [BindProperty] public GeneraluploadURLDTO GeneraluploadURLDTO { get; set; }
        [BindProperty] public List<GeneraluploadURLVM> GeneraluploadURLVM { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var Result = await _httpClient.GetAsync("GenaralUploadDocuments/GetURLsTiming", true).ConfigureAwait(false);
            if (Result == "unauthorized")
            {
                _notyf.Information("Please login/register");
                return RedirectToPage("/Account/Login");
            }
            GeneraluploadURLVM = !string.IsNullOrEmpty(Result) ? JsonConvert.DeserializeObject<List<GeneraluploadURLVM>>(Result) : null;

            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            //GeneraluploadURLDTO.FromTime = DateTime.ParseExact(GeneraluploadURLDTO.FromTime.ToString(), "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            //GeneraluploadURLDTO.ToTime = DateTime.ParseExact(GeneraluploadURLDTO.ToTime.ToString(), "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            var request = HttpContext.Request;
            GeneraluploadURLDTO.UploadUrl = $"{request.Scheme}://{request.Host}/bank/Admin/Document/Add";
            GeneraluploadURLDTO.IsActive = true;
            GeneraluploadURLDTO = ModelAuditor<GeneraluploadURLDTO>.SetAudit(User.Identity.Name, GeneraluploadURLDTO.Id == 0 ? "Create" : "Edit", HttpContext.Connection.RemoteIpAddress.ToString(), GeneraluploadURLDTO);
            var Result = await _httpClient.PostAsync("GenaralUploadDocuments/CreateURLsTiming", true, GeneraluploadURLDTO);
            if (Result == "unauthorized")
            {
                _notyf.Information("Please login/register");
                return RedirectToPage("/Account/Login");
            }
            var TempDTO = !string.IsNullOrEmpty(Result) ? JsonConvert.DeserializeObject<GeneraluploadURLDTO>(Result) : null;
            if (TempDTO == null)
                _notyf.Error("Save failed");
            else
                _notyf.Success("Saved successfully");

            return RedirectToPage("/Admin/GeneralUpload/AddGeneralUpload");
        }

    }
}
