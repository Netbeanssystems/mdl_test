using Application.Dtos;
using Application.Helpers;
using Application.ServiceInterfaces;
using Application.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebBank.Pages.Admin.Document
{
    [Authorize(Roles = "BankAdmin")]

    public class AdminIndexModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly INotyfService _notyf;
        public AdminIndexModel(
           IHttpClientService httpClient,
           INotyfService notyf)
        {
            _httpClient = httpClient;
            _notyf = notyf;
        }
        public List<DocumentsVM> ModelVms { get; set; }
        [BindProperty] public DocumentsDTO ModelDto { get; set; }

        public List<URLsTimingVM> URLsTimingList { get; set; }
        public List<URLsTimingVM> OpenURLsTimingList { get; set; }
        public List<int> YearsList { get; set; }
        public int? SelectedYear { get; set; }
        public int? SelectedURLsTimingId { get; set; }
        public bool IsAnyWindowOpen { get; set; }

        public async Task<IActionResult> OnGetAsync(int? year, int? urlsTimingId)
        {
            SelectedYear = year;
            SelectedURLsTimingId = urlsTimingId;

            // Get ALL URLs Timing list for filter dropdown (open + closed)
            var urlsTimingResponse = await _httpClient.GetAsync("Documents/GetURLsTiming", true).ConfigureAwait(false);
            URLsTimingList = !string.IsNullOrEmpty(urlsTimingResponse) ? JsonConvert.DeserializeObject<List<URLsTimingVM>>(urlsTimingResponse) : null;

            // Get currently OPEN windows
            var now = DateTime.Now;
            OpenURLsTimingList = URLsTimingList?.Where(x => x.FromTime <= now && x.ToTime >= now).ToList();
            IsAnyWindowOpen = OpenURLsTimingList?.Any() ?? false;

            // Get Years list for filter dropdown
            var yearsResponse = await _httpClient.GetAsync("Documents/GetDocumentsYears", true).ConfigureAwait(false);
            YearsList = !string.IsNullOrEmpty(yearsResponse) ? JsonConvert.DeserializeObject<List<int>>(yearsResponse) : null;

            // Get OPEN windows IDs (to exclude from closed windows section)
            var openWindowIds = OpenURLsTimingList?.Select(x => x.Id).ToList() ?? new List<int>();

            if (IsAnyWindowOpen)
            {
                // Some windows are OPEN: 
                // - Show COUNT for OPEN windows
                // - Show FULL details for CLOSED windows ONLY (with filters applied)
                var openWindowResponse = await _httpClient.GetAsync("Documents/GetGroupedByURLsTiming", true).ConfigureAwait(false);
                var openWindowDocs = !string.IsNullOrEmpty(openWindowResponse) 
                    ? JsonConvert.DeserializeObject<List<DocumentsVM>>(openWindowResponse) 
                    : new List<DocumentsVM>();

                // Filter to only OPEN windows
                var openDocs = openWindowDocs.Where(x => openWindowIds.Contains(x.URLsTimingId ?? 0)).ToList();

                // Store open window counts in ViewData for the view
                ViewData["OpenWindowDocs"] = openDocs;
                ViewData["OpenURLsTimingList"] = OpenURLsTimingList;
                
                // For CLOSED windows: Show if filters are applied OR if "All" is selected
                // Get ALL closed window documents (excluding open windows)
                var allDocsResponse = await _httpClient.GetAsync("Documents/Get", true).ConfigureAwait(false);
                var allDocs = !string.IsNullOrEmpty(allDocsResponse) 
                    ? JsonConvert.DeserializeObject<List<DocumentsVM>>(allDocsResponse) 
                    : new List<DocumentsVM>();
                
                // Filter out OPEN windows
                var closedDocs = allDocs.Where(x => !openWindowIds.Contains(x.URLsTimingId ?? 0)).ToList();
                
                // Apply filters if selected
                if (SelectedYear.HasValue || SelectedURLsTimingId.HasValue)
                {
                    if (SelectedYear.HasValue)
                    {
                        closedDocs = closedDocs.Where(x => x.CreatedDate.Year == SelectedYear.Value).ToList();
                    }
                    if (SelectedURLsTimingId.HasValue)
                    {
                        closedDocs = closedDocs.Where(x => x.URLsTimingId == SelectedURLsTimingId.Value).ToList();
                    }
                }
                
                ModelVms = closedDocs;
            }
            else
            {
                // All windows are CLOSED: Show full details with filters
                string apiUrl = "Documents/GetByYearAndDescription?year=" + (SelectedYear?.ToString() ?? "") + "&urlsTimingId=" + (SelectedURLsTimingId?.ToString() ?? "");
                var modelResponse = await _httpClient.GetAsync(apiUrl, true).ConfigureAwait(false);
                if (modelResponse == "unauthorized")
                {
                    _notyf.Information("Please login");
                    return RedirectToPage("/Account/Login");
                }
                ModelVms = !string.IsNullOrEmpty(modelResponse) ? JsonConvert.DeserializeObject<List<DocumentsVM>>(modelResponse) : null;
            }

            if (ModelVms == null || ModelVms.Count <= 0)
            {
                // Only show warning if filters were applied but no results found
                if (SelectedYear.HasValue || SelectedURLsTimingId.HasValue)
                {
                    _notyf.Warning("No documents found for the selected filters!");
                }
                // Don't show warning when no filters are applied (expected behavior)
                return Page();
            }

            foreach (var item in ModelVms)
            {
                try
                {
                    // Try decrypting (for old records)
                    item.DocumentNameDecrypted =
                        EnDeCryptor.DecryptStringAES(
                            item.DocumentName.Split(".")[0]
                                .Replace("B_S", "\"")
                                .Replace("F_S", "/")
                        );
                }
                catch
                {
                    // If not encrypted (new records), show directly
                    item.DocumentNameDecrypted = item.DocumentName;
                }
            }
            return Page();
        }
    }
}
