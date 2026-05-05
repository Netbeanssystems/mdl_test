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
using System.Net;
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
        public int? SelectedURLsTimingId { get; set; }
        public DateTime? SelectedFromDate { get; set; }
        public DateTime? SelectedToDate { get; set; }
        public bool IsAnyWindowOpen { get; set; }
        public bool HasAppliedFilter { get; set; }

        public async Task<IActionResult> OnGetAsync(int? urlsTimingId, DateTime? fromDate, DateTime? toDate)
        {
            SelectedURLsTimingId = urlsTimingId;
            SelectedFromDate = fromDate;
            SelectedToDate = toDate;
            HasAppliedFilter = SelectedURLsTimingId.HasValue || (SelectedFromDate.HasValue && SelectedToDate.HasValue);

            // Update this line in your OnGetAsync
            var urlsTimingResponse = await _httpClient.GetAsync("Documents/GetURLsTiming", true).ConfigureAwait(false);
            var rawList = !string.IsNullOrEmpty(urlsTimingResponse)
                ? JsonConvert.DeserializeObject<List<URLsTimingVM>>(urlsTimingResponse)
                : new List<URLsTimingVM>();

            // Sort by Id Descending (or FromTime) to show latest first
            URLsTimingList = rawList.OrderByDescending(x => x.Id).ToList();


            // Get currently OPEN windows
            var now = DateTime.Now;
            OpenURLsTimingList = URLsTimingList?.Where(x => x.FromTime <= now && x.ToTime >= now).ToList();
            IsAnyWindowOpen = OpenURLsTimingList?.Any() ?? false;

            // Get OPEN windows IDs (to exclude from closed windows section)
            var openWindowIds = OpenURLsTimingList?.Select(x => x.Id).ToList() ?? new List<int>();

            if (IsAnyWindowOpen)
            {
                // Some windows are OPEN:
                // - Show COUNT for OPEN windows
                // - Show FULL details for CLOSED windows ONLY (when filters are applied)
                var openWindowResponse = await _httpClient.GetAsync("Documents/GetGroupedByURLsTiming", true).ConfigureAwait(false);
                var openWindowDocs = !string.IsNullOrEmpty(openWindowResponse)
                    ? JsonConvert.DeserializeObject<List<DocumentsVM>>(openWindowResponse)
                    : new List<DocumentsVM>();

                // Filter to only OPEN windows
                var openDocs = openWindowDocs.Where(x => openWindowIds.Contains(x.URLsTimingId ?? 0)).ToList();

                // Store open window counts in ViewData for the view
                ViewData["OpenWindowDocs"] = openDocs;
                ViewData["OpenURLsTimingList"] = OpenURLsTimingList;

                // For CLOSED windows: Show ONLY when filters are applied
                if (HasAppliedFilter)
                {
                    // Call API with filters
                    string apiUrl = "Documents/GetByURLsTimingAndDateRange?urlsTimingId=" + (SelectedURLsTimingId?.ToString() ?? "")
                        + "&fromDate=" + (SelectedFromDate?.ToString("yyyy-MM-dd") ?? "")
                        + "&toDate=" + (SelectedToDate?.ToString("yyyy-MM-dd") ?? "");
                    var modelResponse = await _httpClient.GetAsync(apiUrl, true).ConfigureAwait(false);
                    if (modelResponse == "unauthorized")
                    {
                        _notyf.Information("Please login");
                        return RedirectToPage("/Account/Login");
                    }
                    var allDocs = !string.IsNullOrEmpty(modelResponse) ? JsonConvert.DeserializeObject<List<DocumentsVM>>(modelResponse) : new List<DocumentsVM>();

                    // Filter out OPEN windows
                    ModelVms = allDocs.Where(x => !openWindowIds.Contains(x.URLsTimingId ?? 0)).ToList();
                }
            }
            else
            {
                // All windows are CLOSED: Show full details with filters (only when filters are applied)
                if (HasAppliedFilter)
                {
                    // Ensure we include the full day for the 'To Date'
                    string formattedToDate = SelectedToDate.HasValue
                        ? SelectedToDate.Value.ToString("yyyy-MM-dd") + " 23:59:59"
                        : "";

                    string apiUrl = "Documents/GetByURLsTimingAndDateRange?urlsTimingId=" + (SelectedURLsTimingId?.ToString() ?? "")
                        + "&fromDate=" + (SelectedFromDate?.ToString("yyyy-MM-dd") ?? "")
                        + "&toDate=" + WebUtility.UrlEncode(formattedToDate); // Encode the space/colon

                    var modelResponse = await _httpClient.GetAsync(apiUrl, true).ConfigureAwait(false);
                    if (modelResponse == "unauthorized")
                    {
                        _notyf.Information("Please login");
                        return RedirectToPage("/Account/Login");
                    }
                    ModelVms = !string.IsNullOrEmpty(modelResponse) ? JsonConvert.DeserializeObject<List<DocumentsVM>>(modelResponse) : null;
                }
            }

            if (ModelVms == null || ModelVms.Count <= 0)
            {
                // Only show warning if filters were applied but no results found
                if (HasAppliedFilter)
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
