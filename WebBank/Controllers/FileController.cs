using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace WebBank.Controllers
{
    [Route("file")]
    public class FileController : Controller
    {
        // HARD-WIRED ROUTE — NO MVC MAGIC
        [HttpGet("download")]
        public IActionResult Download([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return NotFound("No file name");

            var safeName = Path.GetFileName(name);

            var filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "img",
                "UploadedFiles",
                "Documents",
                safeName
            );

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found");

            // 🔒 FORCE DOWNLOAD (CANNOT PREVIEW)
            Response.Headers["Content-Disposition"] =
                $"attachment; filename=\"{safeName}\"";
            Response.Headers["X-Content-Type-Options"] = "nosniff";

            return PhysicalFile(
                filePath,
                "application/octet-stream"
            );
        }
    }
}
