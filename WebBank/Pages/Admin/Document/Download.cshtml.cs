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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.IO;

namespace WebBank.Pages.Admin.Document
{
  //  [Authorize(Roles = "BankUser,SuperAdmin")]
    public class DownloadModel : PageModel
    {
        public IActionResult OnGet(string file)
        {

            
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
