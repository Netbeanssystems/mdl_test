using Application.ServiceInterfaces;
using Application.ServiceInterfaces;
using Application.Services; // Adjust based on your namespace
using Application.ViewModels; // Adjust based on your namespace
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebBank.Pages.Admin.OpeningInvestmentWindow
{
    [Authorize(Roles = "BankAdmin")]
    public class IndexModel(IHttpClientService httpClient) : PageModel
    {

        // This collection will store all windows to display them on the page
        public List<ClosedWindowsVM> ClosedWindowsVM { get; set; } = new List<ClosedWindowsVM>();

        public async Task<IActionResult> OnGetAsync()
        {
            // Call your API
            var response = await httpClient.GetAsync("Documents/GetClosedWindows", true);

            var list  = !string.IsNullOrEmpty(response)
               ? JsonConvert.DeserializeObject<List<ClosedWindowsVM>>(response)
               : new List<ClosedWindowsVM>();

            ClosedWindowsVM = list.OrderByDescending(x => x.CreatedDate).ToList();
            return Page();
        }
    }
}