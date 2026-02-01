using Application.Dtos;
using Application.Helpers;
using Application.ServiceInterfaces;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebForeignBidder.Pages.GeneralUpload
{
    [Authorize(Roles = "GeneralUser,SuperAdmin")]

    public class IndexModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly IFileService _fileService;
        private readonly INotyfService _notyf;
        public IndexModel(IHttpClientService httpClient, IFileService fileService, INotyfService notyf)
        {
            _httpClient = httpClient;
            _fileService = fileService;
            _notyf = notyf;
        }


        [BindProperty] public GenaralUploadDocumentsDTO GenaralUploadDocumentsDTO { get; set; }
        public GeneraluploadURLDTO GeneraluploadURLDTO { get; set; }
        //public string BranchName => User.Claims.FirstOrDefault(x => x.Type == "bnam")?.Value;
        //public string BankName => User.Claims.FirstOrDefault(x => x.Type == "nam")?.Value;
        [BindProperty]
        public bool IsInTime { get; set; } = false;


        public async Task<IActionResult> OnGet()
        {
            var request = HttpContext.Request;
            var URL = $"{request.Scheme}://{request.Host}/bank/Admin/Document/Add";
            var result = await _httpClient.GetAsync("GenaralUploadDocuments/GetURLsTiming", true).ConfigureAwait(false);
            var model = !string.IsNullOrEmpty(result) ? JsonConvert.DeserializeObject<List<GeneraluploadURLDTO>>(result) : null;
            GeneraluploadURLDTO = model.Where(x => x.UploadUrl == URL).OrderByDescending(x => x.Id).Take(1).FirstOrDefault();
            if (GeneraluploadURLDTO == null) return Page();

            var dateTime = DateTime.Now;
            if (dateTime >= GeneraluploadURLDTO.FromTime && dateTime <= GeneraluploadURLDTO.ToTime) IsInTime = true;
            else IsInTime = false;

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {



            if (GenaralUploadDocumentsDTO.DocumentFile != null)
            {//...........Check Valid File ...................
                if (!_fileService.CheckValidFile(GenaralUploadDocumentsDTO.DocumentFile))
                {
                    _notyf.Information("Please Upload Valid File");
                    return RedirectToPage("Index");
                }
                string allowedExtentions = ".pdf";
                if (!_fileService.CheckFiles(GenaralUploadDocumentsDTO.DocumentFile, allowedExtentions))
                {
                    _notyf.Information("Please Upload Valid File");
                    return RedirectToPage("Index");
                }

                GenaralUploadDocumentsDTO.DocumentName = await _fileService.SaveEncryptionAsync(@"\img\UploadedFiles\GeneralUpload\", GenaralUploadDocumentsDTO.DocumentFile);
                GenaralUploadDocumentsDTO.DocumentFile = null;
            }

            GenaralUploadDocumentsDTO.Description = "Description";
           // GenaralUploadDocumentsDTO.BankName = BankName;
            GenaralUploadDocumentsDTO = ModelAuditor<GenaralUploadDocumentsDTO>.SetAudit(User.Identity.Name, GenaralUploadDocumentsDTO.Id == 0 ? "Create" : "Edit", HttpContext.Connection.RemoteIpAddress.ToString(), GenaralUploadDocumentsDTO);
            var Result = await _httpClient.PostAsync("GenaralUploadDocuments/Create", true, GenaralUploadDocumentsDTO);
            if (Result == "unauthorized")
            {
                _notyf.Information("Please login/register");
                return RedirectToPage("/Account/Login");
            }
            var TempDTO = !string.IsNullOrEmpty(Result) ? JsonConvert.DeserializeObject<GenaralUploadDocumentsDTO>(Result) : null;
            if (TempDTO == null)
                _notyf.Error("Save failed");
            else
                _notyf.Success("Saved successfully");
            return RedirectToPage("Index");
        }

       
    }
}

