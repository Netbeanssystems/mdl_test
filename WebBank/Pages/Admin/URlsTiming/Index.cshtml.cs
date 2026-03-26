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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace WebBank.Pages.Admin.URlsTiming
{
    [Authorize(Roles = "BankAdmin,SuperAdmin")]
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


        [BindProperty] public URLsTimingDTO URLsTimingDTO { get; set; }
        [BindProperty] public List<URLsTimingVM> URLsTimingVM { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var Result = await _httpClient.GetAsync("Documents/GetURLsTiming", true).ConfigureAwait(false);
            if (Result == "unauthorized")
            {
                _notyf.Information("Please login/register");
                return RedirectToPage("/Account/Login");
            }
            URLsTimingVM = !string.IsNullOrEmpty(Result) ? JsonConvert.DeserializeObject<List<URLsTimingVM>>(Result) : null;

            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            var request = HttpContext.Request;
            URLsTimingDTO.Url = $"{request.Scheme}://{request.Host}/bank/Admin/Document/Add";
            URLsTimingDTO.IsActive = true;
            
            // Parse date strings from form (MM/dd/yyyy HH:mm format)
            if (!DateTime.TryParseExact(URLsTimingDTO.FromTimeStr, "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fromTime))
            {
                _notyf.Error("Invalid From Date format. Please use MM/dd/yyyy HH:mm");
                return Page();
            }
            if (!DateTime.TryParseExact(URLsTimingDTO.ToTimeStr, "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var toTime))
            {
                _notyf.Error("Invalid To Date format. Please use MM/dd/yyyy HH:mm");
                return Page();
            }
            
            URLsTimingDTO.FromTime = fromTime;
            URLsTimingDTO.ToTime = toTime;
            
            // Validate ToTime is after FromTime
            if (toTime <= fromTime)
            {
                _notyf.Error("To Date must be after From Date");
                return Page();
            }
            
            URLsTimingDTO = ModelAuditor<URLsTimingDTO>.SetAudit(User.Identity.Name, URLsTimingDTO.Id == 0 ? "Create" : "Edit", HttpContext.Connection.RemoteIpAddress.ToString(), URLsTimingDTO);
            var Result = await _httpClient.PostAsync("Documents/CreateURLsTiming", true, URLsTimingDTO);
            if (Result == "unauthorized")
            {
                _notyf.Information("Please login/register");
                return RedirectToPage("/Account/Login");
            }
            var TempDTO = !string.IsNullOrEmpty(Result) ? JsonConvert.DeserializeObject<URLsTimingDTO>(Result) : null;
            if (TempDTO == null)
                _notyf.Error("Save failed");
            else
                _notyf.Success("Saved successfully");

            return RedirectToPage("Index");
        }

    }
}
