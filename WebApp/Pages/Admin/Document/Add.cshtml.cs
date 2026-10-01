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
namespace WebApp.Pages.Admin.Document
{
    [Authorize(Roles = "BankUser,SuperAdmin")]
    public class AddModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly IFileService _fileService;
        private readonly INotyfService _notyf;

        public AddModel(IHttpClientService httpClient, IFileService fileService, INotyfService notyf)
        {
            _httpClient = httpClient;
            _fileService = fileService;
            _notyf = notyf;
        }


        [BindProperty] public DocumentsDTO DocumentsDTO { get; set; }
        public URLsTimingDTO URLsTimingDTO { get; set; }
        public string BranchName => User.Claims.FirstOrDefault(x => x.Type == "bnam")?.Value;
        public string BankName => User.Claims.FirstOrDefault(x => x.Type == "nam")?.Value;
        [BindProperty]
        public bool IsInTime { get; set; } = false;


        public async Task<IActionResult> OnGet()
        {
            var request = HttpContext.Request;
            var URL = $"{request.Scheme}://{request.Host}/admin/Admin/Document/Add";
            var result = await _httpClient.GetAsync("Documents/GetURLsTiming", true).ConfigureAwait(false);
            var model = !string.IsNullOrEmpty(result) ? JsonConvert.DeserializeObject<List<URLsTimingDTO>>(result) : null;
            URLsTimingDTO = model.Where(x => x.Url == URL).OrderByDescending(x => x.Id).Take(1).FirstOrDefault();
            if (URLsTimingDTO == null) return Page();

            var dateTime = DateTime.Now;
            if (dateTime >= URLsTimingDTO.FromTime && dateTime <= URLsTimingDTO.ToTime) IsInTime = true;
            else IsInTime = false;

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {

            if (DocumentsDTO.DocumentFile != null)
            {//...........Check Valid File ...................
                string allowedExtentions = ".pdf";
                if (!_fileService.CheckFiles(DocumentsDTO.DocumentFile, allowedExtentions))
                {
                    _notyf.Information("Please Upload Valid File");
                    return RedirectToPage("Index");
                }
                DocumentsDTO.DocumentName = await _fileService.SaveEncryptionAsync(@"\img\UploadedFiles\Documents\", DocumentsDTO.DocumentFile);
                DocumentsDTO.DocumentFile = null;
            }

            DocumentsDTO.BranchName = BranchName;
            DocumentsDTO.BankName = BankName;
            DocumentsDTO = ModelAuditor<DocumentsDTO>.SetAudit(User.Identity.Name, DocumentsDTO.Id == 0 ? "Create" : "Edit", HttpContext.Connection.RemoteIpAddress.ToString(), DocumentsDTO);
            var Result = await _httpClient.PostAsync("Documents/Create", true, DocumentsDTO);
            if (Result == "unauthorized")
            {
                _notyf.Information("Please login/register");
                return RedirectToPage("/Account/Login");
            }
            var TempDTO = !string.IsNullOrEmpty(Result) ? JsonConvert.DeserializeObject<DocumentsDTO>(Result) : null;
            if (TempDTO == null)
                _notyf.Error("Save failed");
            else
                _notyf.Success("Saved successfully");
            return RedirectToPage("Index");
        }

        //private DocumentsDTO SetAudit(string action, string status, DocumentsDTO model)
        //{
        //    var uname = User.Identity.Name;
        //    var role = User.Claims.First(c => c.Type == ClaimTypes.Role).Value;
        //    return new DocumentsDTO
        //    {
        //        DocumentName=model.DocumentName,
        //        //ActionDate = DateTime.UtcNow,
        //        //UpdateDate = DateTime.Now,
        //        //UserName = uname,
        //        //RoleName = role,
        //        //Status = status,
        //        //IP = HttpContext.Connection.RemoteIpAddress.ToString(),
        //        //RowId = null
        //    };
        //}

        //private async Task<IActionResult> CreateAudit(DocumentsDTO modelDto)
        //{
        //    var x = ModelState.IsValid;
        //    //var Result = await _httpClient.PostAsync("TempOtherLinkHeading/CreateAudit", true, modelDto);
        //    var Result = await _httpClient.PostAsync("Document/Create", true, modelDto);
        //    if (Result == "unauthorized")
        //    {
        //        _notyf.Information("Please login/register");
        //        return RedirectToPage("/Account/Login");
        //    }
        //    var TempDTO = !string.IsNullOrEmpty(Result) ? JsonConvert.DeserializeObject<DocumentsDTO>(Result) : null;
        //    if (TempDTO == null)
        //        _notyf.Error("Save failed");
        //    else
        //        _notyf.Success("Saved successfully");
        //    return RedirectToPage("Index");
        //}
    }
}
