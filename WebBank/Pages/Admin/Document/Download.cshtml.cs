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


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.IO;

namespace WebBank.Pages.Admin.Document
{
    [Authorize(Roles = "BankUser,SuperAdmin")]
    [Authorize]
    public class DownloadModel : PageModel
    {



        public IActionResult OnGet(string file)
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

            // 🔐 FORCE DOWNLOAD
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


