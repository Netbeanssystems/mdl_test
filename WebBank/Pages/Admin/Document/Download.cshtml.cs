//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;

//namespace WebBank.Pages.Admin.Document
//{
//    public class DownloadModel : PageModel
//    {
//        public void OnGet()
//        {
//        }
//    }
//}


using Application.Dtos;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebBank.Pages.Admin.Document
{
    [Authorize(Roles = "BankUser,SuperAdmin")]
    public class DownloadModel : PageModel
    {
        private readonly IHttpClientService _httpClient;

        // Constructor injection for your existing HTTP client communication service
        public DownloadModel(IHttpClientService httpClient)
        {
            _httpClient = httpClient;
        }

        // Changed from IActionResult to async Task<IActionResult> for the API log dispatch
        public async Task<IActionResult> OnGetAsync(string file)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/Account/Login");
            }

            if (string.IsNullOrWhiteSpace(file))
                return NotFound();

            // Prevent path traversal
            var safeFileName = Path.GetFileName(file);

            if (!safeFileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                return BadRequest();

            var filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "bank",
                "img",
                "UploadedFiles",
                "Documents",
                safeFileName
            );

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            
            try
            {
                var logDto = new DocumentDownloadLogDTO
                {
                    DocumentName = safeFileName,
                    DownloadedBy = User.Identity?.Name ?? "Unknown User",
                    DownloadedAt = DateTimeOffset.Now, 
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown"
                };

                // Forwarding payload to your backend API route
                // Second parameter set to true assuming it handles token pass-through like your other gets
                await _httpClient.PostAsync("Documents/LogDownload", true, logDto);
            }
            catch (Exception ex)
            {
                // Log exception internally if backend logging pipeline fails, 
                // but don't crash the request—let the user download their file anyway.
                // _logger.LogError(ex, "Failed to record file download audit trail.");
            }

            //  FORCE DOWNLOAD
            return PhysicalFile(
                filePath,
                "application/pdf",
                safeFileName
            );
        }
    }
}

//[Authorize(Roles = "BankUser,SuperAdmin")]
//public class DownloadModel : PageModel
//{
//    private readonly IWebHostEnvironment _env;

//    public DownloadModel(IWebHostEnvironment env)
//    {
//        _env = env;
//    }

//    public IActionResult OnGet(string name)
//    {
//        // 1. SESSION CHECK: If the session variable is missing, block the request
//        var authFlag = HttpContext.Session.GetString("IsBankAuthorized");
//        if (string.IsNullOrEmpty(authFlag) || authFlag != "true")
//        {
//            return Unauthorized(); // Or RedirectToPage("/Account/Login")
//        }

//        if (string.IsNullOrWhiteSpace(name))
//            return NotFound();

//        var safeFileName = Path.GetFileName(name);

//        // 2. SECURITY CRITICAL: 
//        // Move your files to "C:\BankData\" or a folder NOT inside wwwroot.
//        // If they stay in wwwroot, the session check is easily bypassed by the direct URL.
//        var filePath = Path.Combine(_env.ContentRootPath, "SecureStorage", "Documents", safeFileName);

//        if (!System.IO.File.Exists(filePath))
//            return NotFound();

//        return PhysicalFile(filePath, "application/pdf", safeFileName);
//    }
//}


