using Application.Dtos;
using Application.Helpers;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class FileService : IFileService
    {
        private enum FileFormat { jpeg, jpeg2, png, bmp, gif, tiff, tiff2, pdf, officenew, officeold, corrupt, unknown }
        private static int AllowedFileSize;
        private static int AllowedVideoSize;
        //private static List<string> AllowedContentTypes;
        private static List<string> AllowedFileExtensions;
        private static List<string> AllowedVideoExtensions;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IConfiguration _config;
        public FileService(IWebHostEnvironment hostingEnvironment,
            IConfiguration config)
        {
            _hostingEnvironment = hostingEnvironment;
            _config = config;
            //AllowedContentTypes = _config.GetSection("AllowedContentTypes").GetChildren().Select(x => x.Value).ToList();
            AllowedFileExtensions = _config.GetSection("AllowedFileExtensions").GetChildren().Select(x => x.Value).ToList();
            AllowedVideoExtensions = _config.GetSection("AllowedVideoExtensions").GetChildren().Select(x => x.Value).ToList();
            AllowedFileSize = Convert.ToInt32(_config["AllowedFileSize"]);
            AllowedVideoSize = Convert.ToInt32(_config["AllowedVideoSize"]);
        }
        private static FileFormat GetFileExtType(byte[] bytes)
        {
            var jpeg = new byte[] { 255, 216, 255, 224 }; // jpeg or jpg
            var jpeg2 = new byte[] { 255, 216, 255, 225 }; // jpeg canon
            var png = new byte[] { 137, 80, 78, 71 }; // png
            var tiff = new byte[] { 73, 73, 42 }; // TIFF
            var tiff2 = new byte[] { 77, 77, 42 }; // TIFF
            var officenew = new byte[] { 80, 75, 3, 4, 20 }; // OfficeNew
            var officeold = new byte[] { 208, 207, 17, 224, 161 }; // OfficeOld
            var corrupt = new byte[] { 77, 90, 144 };
            var bmp = Encoding.ASCII.GetBytes("BM"); // BMP
            var gif = Encoding.ASCII.GetBytes("GIF"); // GIF
            var pdf = Encoding.ASCII.GetBytes("%PDF-"); // pdf
            if (jpeg.SequenceEqual(bytes.Take(jpeg.Length)))
                return FileFormat.jpeg;
            if (jpeg2.SequenceEqual(bytes.Take(jpeg2.Length)))
                return FileFormat.jpeg2;
            if (png.SequenceEqual(bytes.Take(png.Length)))
                return FileFormat.png;
            if (bmp.SequenceEqual(bytes.Take(bmp.Length)))
                return FileFormat.bmp;
            if (gif.SequenceEqual(bytes.Take(gif.Length)))
                return FileFormat.gif;
            if (tiff.SequenceEqual(bytes.Take(tiff.Length)))
                return FileFormat.tiff;
            if (tiff2.SequenceEqual(bytes.Take(tiff2.Length)))
                return FileFormat.tiff2;
            if (pdf.SequenceEqual(bytes.Take(pdf.Length)))
                return FileFormat.pdf;
            if (corrupt.SequenceEqual(bytes.Take(corrupt.Length)))
                return FileFormat.corrupt;
            if (officenew.SequenceEqual(bytes.Take(officenew.Length)))
                return FileFormat.officenew;
            if (officeold.SequenceEqual(bytes.Take(officeold.Length)))
                return FileFormat.officeold;
            return FileFormat.unknown;
        }
        public bool CheckImageFile(IFormFile file)
        {
            byte[] fileBytes;
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                fileBytes = ms.ToArray();
            }
            var fileType = GetFileExtType(fileBytes);
            return AllowedFileExtensions.Contains(fileType.ToString());
        }
        public bool CheckFileSize(IFormFile file)
        {
            return file.Length <= AllowedFileSize;
        }
        //public bool CheckImageFiles(List<IFormFile> files)
        //{
        //    var filesAreImages = new List<bool>(files.Count);
        //    filesAreImages.AddRange(files.Select(CheckImageFile));
        //    return filesAreImages.All(x => x);
        //}
        public bool CheckVideoFile(IFormFile file)
        {
            var Extention = Path.GetExtension(file.FileName);
            return AllowedVideoExtensions.Contains(Extention);
        }
        public bool CheckVideoSize(IFormFile file)
        {
            return file.Length <= AllowedVideoSize;
        }
        //public bool CheckIfImageFile(IFormFile file)
        //{
        //    ////Check the image mime types
        //    //if (AllowedContentTypes.All(x => x != file.ContentType))
        //    //    return false;
        //    //  Check the image extension
        //    var fileExtension = Path.GetExtension(file.FileName);
        //    if (AllowedFileExtensions.All(x => x != fileExtension))
        //        return false;
        //    try
        //    {
        //        //Attempt to read the file and check the first bytes
        //        if (!file.OpenReadStream().CanRead) return false;
        //        //Check whether the image size exceeding the limit or not
        //        if (file.Length > AllowedFileSize) return false;
        //        var buffer = new byte[AllowedFileSize];
        //        file.OpenReadStream().Read(buffer, 0, AllowedFileSize);
        //        var content = Encoding.UTF8.GetString(buffer);
        //        if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
        //            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
        //            return false;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //    return true;
        //}
        //public bool CheckIfImageFileList(List<IFormFile> files)
        //{
        //    foreach (var file in files)
        //    {
        //        //Check the image mime types
        //        if (AllowedContentTypes.All(x => x != file.ContentType))
        //            return false;
        //        //  Check the image extension
        //        var fileExtension = Path.GetExtension(file.FileName);
        //        if (AllowedFileExtensions.All(x => x != fileExtension))
        //            return false;
        //        try
        //        {
        //            //Attempt to read the file and check the first bytes
        //            if (!file.OpenReadStream().CanRead) return false;
        //            //Check whether the image size exceeding the limit or not
        //            if (file.Length > AllowedFileSize) return false;
        //            var buffer = new byte[AllowedFileSize];
        //            file.OpenReadStream().Read(buffer, 0, AllowedFileSize);
        //            var content = Encoding.UTF8.GetString(buffer);
        //            if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
        //                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
        //                return false;
        //        }
        //        catch (Exception)
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}
        public async Task<string> SaveFileAsync(FileUploadDTO FileUploadDto)
        {
            //check to see if folder exists if not create it
            FolderCreator(FileUploadDto.FilePath);

            var filename = FileUploadDto.ChangeName ? $"{Guid.NewGuid():N}{Path.GetExtension(FileUploadDto.UploadedFile.FileName)}" : FileUploadDto.UploadedFile.FileName;
            var relativepath = $"{FileUploadDto.FilePath}\\{filename}";
            var physicalPath = $"{_hostingEnvironment.WebRootPath}{relativepath}";

            if (!FileUploadDto.ChangeDimensions)
            {
                var fileCreated = await CreateFile(FileUploadDto.UploadedFile, physicalPath);
                if (fileCreated && !string.IsNullOrEmpty(FileUploadDto.ReturnValue))
                    return FileUploadDto.ReturnValue == "name" ? filename : relativepath;
            }

            //check if file is a image
            if (!CheckImageFile(FileUploadDto.UploadedFile)) return null;

            //Create file at temp path
            var relativetemppath = $"\\img\\temp\\{filename}";
            var tempPath = $"{_hostingEnvironment.WebRootPath}{relativetemppath}";
            var tempFileCreated = await CreateFile(FileUploadDto.UploadedFile, tempPath);
            if (!tempFileCreated) return null;

            //Save resized image at physical path
            var resized = new Bitmap(FileUploadDto.Width, FileUploadDto.Height);
            var graphics = Graphics.FromImage(resized);
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.CompositingMode = CompositingMode.SourceCopy;
            var imageRectangle = new Rectangle(0, 0, FileUploadDto.Width, FileUploadDto.Height);
            var image = Image.FromFile(tempPath);
            graphics.DrawImage(image, imageRectangle);
            resized.Save(physicalPath, image.RawFormat);
            image.Dispose();
            resized.Dispose();

            //Delete the temp file
            DeleteFile(relativetemppath);
            if (File.Exists(physicalPath) && !string.IsNullOrEmpty(FileUploadDto.ReturnValue))
                return FileUploadDto.ReturnValue == "name" ? filename : relativepath;

            return null;
        }

        private static async Task<bool> CreateFile(IFormFile file, string path)
        {
            await using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream).ConfigureAwait(false);
            await stream.DisposeAsync();
            return File.Exists(path);
        }

        //public async Task<string> SaveFileAsync(string path, IFormFile file, bool changename, string nameorpath)
        //{
        //    //check to see if folder exists if not create it
        //    FolderCreator(path);
        //    var filename = changename ? $"{Guid.NewGuid():N}{Path.GetExtension(file.FileName)}" : file.FileName;
        //    var relativepath = $"{path}\\{filename}";
        //    path = $"{_hostingEnvironment.WebRootPath}{relativepath}";
        //    await using var stream = new FileStream(path, FileMode.Create);
        //    await file.CopyToAsync(stream).ConfigureAwait(false);
        //    await stream.DisposeAsync();
        //    var returnvalue = string.Empty;
        //    if (!string.IsNullOrEmpty(nameorpath))
        //        returnvalue = nameorpath == "name" ? filename : relativepath;
        //    return returnvalue;
        //}
        //public async Task<string> SaveFileAsync(string path, IFormFile file)
        //{
        //    //check to see if folder exists if not create it
        //    FolderCreator(path);
        //    var filename = $"{Guid.NewGuid():N}{Path.GetExtension(file.FileName)}";
        //    path = $"{_hostingEnvironment.WebRootPath}{path}\\{filename}";
        //    await using var stream = new FileStream(path, FileMode.Create);
        //    await file.CopyToAsync(stream).ConfigureAwait(false);
        //    return filename;
        //}
        public async Task<string> SaveFileAsync(string path, Stream stream, string name, string extension)
        {
            //check to see if folder exists if not create it
            FolderCreator(path);
            var filename = $"{name}.{extension}";
            path = $"{_hostingEnvironment.WebRootPath}{path}\\{filename}";
            await using var fileStream = new FileStream(path, FileMode.Create);
            await stream.CopyToAsync(fileStream).ConfigureAwait(false);
            return filename;
        }
        //public async Task<List<string>> SaveFileAsync(string path, List<IFormFile> files)
        //{
        //    var filenames = new List<string>(files.Count);
        //    foreach (var file in files)
        //        filenames.Add(await SaveFileAsync(path, file).ConfigureAwait(false));
        //    return filenames;
        //}
        private void FolderCreator(string path)
        {
            var path_to_check = _hostingEnvironment.WebRootPath;
            var folders = path.Split(@"\");
            foreach (var item in folders)
            {
                path_to_check += item;
                if (!Directory.Exists(path_to_check))
                    Directory.CreateDirectory(path_to_check);
                path_to_check += @"\";
            }
        }
        public bool DeleteFile(string path)
        {
            bool deleted;
            var folderOK = true;
            //check to see if folder exists
            var path_to_check = _hostingEnvironment.WebRootPath;
            var folders = path.Split(@"\");
            try
            {
                for (var i = 0; i < folders.Length - 1; i++)
                {
                    path_to_check += folders[i];
                    if (!Directory.Exists(path_to_check))
                        folderOK = false;
                    path_to_check += @"\";
                }
                if (folderOK)
                    File.Delete($"{_hostingEnvironment.WebRootPath}{path}");
                deleted = true;
            }
            catch (Exception)
            {
                deleted = false;
            }
            return deleted;
        }
        public byte[] Download(string path, string filename)
        {
            var filepath = $"{_hostingEnvironment.WebRootPath}{path}{filename}";
            try
            {
                var fileBytes = File.ReadAllBytes(filepath);
                return fileBytes;
            }
            catch (Exception)
            {
                Console.WriteLine();
                return null;
            }
        }

        //public string CopyFile(string sourcepath, string sourcename, string destinationpath, string destinationname, bool overwrite)
        //{
        //    var source = $"{_hostingEnvironment.WebRootPath}{sourcepath}{sourcename}";
        //    var destination = $"{_hostingEnvironment.WebRootPath}{destinationpath}{destinationname}";
        //    File.Copy(source, destination, overwrite);
        //    return $"{_hostingEnvironment.WebRootPath}{destinationpath}";
        //}

        public string GetPhysicalPath(string folderpath)
        {
            var path = $"{_hostingEnvironment.WebRootPath}";
            if (!string.IsNullOrEmpty(folderpath))
                path = $"{_hostingEnvironment.WebRootPath}{folderpath}";
            return path;
        }

        public void ExtractZipFile(string relativepath, IFormFile file)
        {
            var path = $"{_hostingEnvironment.WebRootPath}{relativepath}";
            using var archive = new ZipArchive(file.OpenReadStream());
            foreach (var entry in archive.Entries)
            {
                if (!string.IsNullOrEmpty(Path.GetExtension(entry.FullName)))
                    entry.ExtractToFile(Path.Combine(path, entry.FullName), true);
                else
                    Directory.CreateDirectory(Path.Combine(path, entry.FullName));
            }
        }
        //public async Task<string> SaveFileAsync(string path, string fileName, byte[] content)
        //{
        //    //check to see if folder exists if not create it
        //    FolderCreator(path);
        //    var filename = $"{Guid.NewGuid():N}{Path.GetExtension(fileName)}";
        //    path = $"{_hostingEnvironment.WebRootPath}{path}\\{filename}";
        //    await File.WriteAllBytesAsync(path, content);
        //    return filename;
        //}

        //Added later from HPCL to be refactored
        //public bool CheckValidFile(IFormFile file)
        //{
        //    string fileclass;
        //    bool Status = false;
        //    using (var reader = new StreamReader(file.OpenReadStream()))
        //    {
        //        var FF = reader.BaseStream;
        //        fileclass = reader.Read().ToString();
        //        fileclass += reader.Read().ToString();
        //    }
        //    //To Get File Extension  
        //    string FileExtension = Path.GetExtension(file.FileName);
        //    if (FileExtension == ".xls" || FileExtension == ".xlsx" || FileExtension == ".pdf" || FileExtension == ".jpg" || FileExtension == ".JPG" || FileExtension == ".png" || FileExtension == ".PNG" || FileExtension == ".jpeg" || FileExtension == ".doc" || FileExtension == ".docx")
        //        Status = true;
        //    //To Get File Type From Extenion  
        //    var contentType = MimeTypes.GetContentType(file.FileName);
        //    if (contentType == "application/pdf" || contentType == "application/excel") // || contentType == "application/msword")
        //        Status = true;
        //    //......//To Get Actual File Using Array.......
        //    //if (fileclass == "7790" || fileclass == "8297" || fileclass == "760")
        //    if (fileclass != "2780")
        //        Status = false;
        //    return Status;
        //}

        //public bool CheckValidFile(IFormFile file)
        //{
        //    bool isValid = false;

        //    // Check file extension
        //    string[] allowedExtensions = { ".xls", ".xlsx", ".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx" };
        //    string extension = Path.GetExtension(file.FileName).ToLower();

        //    if (!allowedExtensions.Contains(extension))
        //        return false;

        //    // Check MIME type
        //    var contentType = MimeTypes.GetContentType(file.FileName);
        //    string[] allowedMimeTypes = {
        //        "application/pdf", "application/vnd.ms-excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        //        "image/jpeg", "image/png", "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
        //    };

        //    if (!allowedMimeTypes.Contains(contentType))
        //        return false;

        //    // Check magic number (file signature)
        //    byte[] buffer = new byte[4];
        //    using (var stream = file.OpenReadStream())
        //    {
        //        stream.Read(buffer, 0, buffer.Length);
        //    }

        //    string fileSignature = BitConverter.ToString(buffer).Replace("-", "");

        //    // Check against known file signatures
        //    var validSignatures = new List<string>
        //    {
        //        "25504446", // PDF => %PDF
        //        "D0CF11E0", // DOC, XLS (old binary)
        //        "504B0304", // DOCX, XLSX (Office Open XML)
        //        "FFD8FFE0", // JPG
        //        "89504E47"  // PNG
        //    };

        //    if (validSignatures.Any(sig => fileSignature.StartsWith(sig)))
        //        isValid = true;

        //    return isValid;
        //}

        public bool CheckValidFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            string extension = Path.GetExtension(file.FileName)?.ToLowerInvariant() ?? "";

            // ✅ 1. Only allow specific extensions
            string[] allowedExtensions = { ".xls", ".xlsx", ".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx" };
            if (!allowedExtensions.Contains(extension))
                return false;

            // ✅ 2. Allowed MIME types
            string[] allowedMimeTypes = {
        "application/pdf",
        "application/vnd.ms-excel",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "image/jpeg",
        "image/png",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
    };

            var contentType = file.ContentType.ToLowerInvariant();
            if (!allowedMimeTypes.Contains(contentType))
                return false;

            // ✅ 3. Read initial part of the file for inspection
            byte[] buffer = new byte[4096]; // read first 4 KB
            int bytesRead = 0;
            using (var stream = file.OpenReadStream())
            {
                bytesRead = stream.Read(buffer, 0, buffer.Length);
            }

            // Convert bytes to text safely
            string fileText = "";
            try
            {
                fileText = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead).ToLowerInvariant();
            }
            catch
            {
                // non-text file, ignore
            }

            // ✅ 4. Detect suspicious text or HTML/script tags
            string[] suspiciousMarkers =
            {
        "<script", "<html", "<!doctype", "onerror=", "onload=", "javascript:",
        "data:text/html", "<iframe", "<svg", "<img", "alert(", "document.cookie", "<body", "<head"
    };

            if (suspiciousMarkers.Any(marker => fileText.Contains(marker)))
            {
                // ❌ found suspicious text content inside file (XSS payload)
                return false;
            }

            // ✅ 5. Optional: check real file signature (magic number)
            string signatureHex = BitConverter.ToString(buffer.Take(4).ToArray()).Replace("-", "");
            var validSignatures = new List<string>
    {
        "25504446", // PDF
        "D0CF11E0", // DOC, XLS (old binary)
        "504B0304", // DOCX, XLSX (Office Open XML)
        "FFD8FFE0", // JPG
        "FFD8FFE1", // JPG variant
        "89504E47"  // PNG
    };

            if (!validSignatures.Any(sig => signatureHex.StartsWith(sig, StringComparison.OrdinalIgnoreCase)))
                return false;

            return true;
        }


        public bool CheckFiles(IFormFile file, string AllowedExtentions)
        {
            bool Status = false;

            // Check file size (should be less than 2MB)
            const int MaxFileSize = 2 * 1024 * 1024; // 2MB
            if (file.Length > MaxFileSize)
            {
                return Status; // Return false if the file is too large
            }

            string FileExtension = Path.GetExtension(file.FileName);
            string[] allowedExtensions = AllowedExtentions.Split(",");

            if (!allowedExtensions.Contains(FileExtension))
            {
                return Status; // Return false if the extension is not allowed
            }

            string[] AllowedExtention = AllowedExtentions.Split(",");
            var isAllowed = AllowedExtention.Where(x => x == FileExtension).ToList();
            if (isAllowed != null)
                Status = true;

            var contentType = MimeTypes.GetContentType(file.FileName);
            if (contentType != "application/pdf")
                Status = false;

            return Status;
        }
        //Added later from HPCL to be refactored
        public async Task<string> SaveImageAsync(string path, IFormFile file)
        {
            //check to see if folder exists if not create it
            FolderCreator(path);
            var filename = $"{Guid.NewGuid():N}{Path.GetExtension(file.FileName)}";
            path = $"{_hostingEnvironment.WebRootPath}{path}\\{filename}";
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return filename;
        }

        public async Task<string> SaveEncryptionAsync(string path, IFormFile file)
        {
            //check to see if folder exists if not create it
            FolderCreator(path);
            //var filename = $"{Guid.NewGuid():N}{Path.GetExtension(file.FileName)}";
            string fileEnName = Convert.ToBase64String(EnDeCryptor.EncryptStringAES(file.FileName.Split(".")[0]));
            fileEnName = fileEnName.Replace("\"", "B_S").Replace("/", "F_S");
            var filename = $"{fileEnName:N}{Path.GetExtension(file.FileName)}";
            path = $"{_hostingEnvironment.WebRootPath}{path}\\{filename}";
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return filename;
        }
        //Added later from HPCL to be refactored
        public async Task<List<string>> SaveImageAsync(string path, List<IFormFile> files)
        {
            var filenames = new List<string>(files.Count);
            foreach (var file in files)
                filenames.Add(await SaveImageAsync(path, file));
            return filenames;
        }

        public (bool IsValid, string ErrorMessage) ValidateFileNameSecurity(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return (false, "File name cannot be empty.");

            fileName = fileName.Trim();

            // 1. Check for path traversal or directory separators
            if (fileName.Contains("..") || fileName.Contains('/') || fileName.Contains('\\'))
                return (false, $"File name '{fileName}' contains invalid path traversal characters.");

            // 2. Check for invalid characters
            if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                return (false, $"File name '{fileName}' contains invalid characters.");

            // 3. Check for multiple dots: filename must have EXACTLY ONE dot
            int dotCount = fileName.Count(c => c == '.');
            if (dotCount == 0)
                return (false, $"File name '{fileName}' is missing an extension.");
            if (dotCount > 1)
                return (false, $"File name '{fileName}' is invalid. Multiple dots in file name are strictly prohibited for security reasons.");

            // 4. File name cannot start with a dot (hidden file / extension only)
            if (fileName.StartsWith("."))
                return (false, $"File name '{fileName}' must have a valid file name before the extension.");

            // 5. Length check
            if (fileName.Length > 150)
                return (false, $"File name '{fileName}' exceeds the maximum allowed length of 150 characters.");

            // 6. Check base name for Windows reserved device names
            string baseName = Path.GetFileNameWithoutExtension(fileName).ToUpperInvariant();
            var reservedNames = new[] { "CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9" };
            if (reservedNames.Contains(baseName))
                return (false, $"File name '{fileName}' uses a reserved system name.");

            // 7. Check for dangerous / executable extensions
            string ext = Path.GetExtension(fileName).ToLowerInvariant();
            var dangerousExtensions = new[]
            {
                ".exe", ".dll", ".bat", ".cmd", ".sh", ".vbs", ".ps1", ".js", ".jsp", ".asp", ".aspx",
                ".php", ".cgi", ".msi", ".scr", ".com", ".pif", ".hta", ".jar", ".reg", ".wsf", ".vbe",
                ".bin", ".apk", ".deb", ".rpm", ".iso", ".img", ".sys", ".drv", ".cpl", ".inf", ".ins"
            };
            if (dangerousExtensions.Contains(ext))
                return (false, $"File '{fileName}' has a dangerous extension '{ext}' that is not allowed.");

            return (true, string.Empty);
        }

        private (bool IsValid, string ErrorMessage) ValidateCsvFile(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            if (stream.Length < 1)
                return (false, $"File '{file.FileName}' is empty.");

            byte[] header = new byte[8];
            int read = stream.Read(header, 0, header.Length);

            // Check if file is secretly a binary executable or archive disguised as CSV
            // MZ (0x4D, 0x5A) -> DOS/PE/Windows executable
            if (read >= 2 && header[0] == 0x4D && header[1] == 0x5A)
                return (false, $"File '{file.FileName}' is not a valid CSV file (executable header detected).");

            // ELF (0x7F, 'E', 'L', 'F') -> Linux executable
            if (read >= 4 && header[0] == 0x7F && header[1] == 0x45 && header[2] == 0x4C && header[3] == 0x46)
                return (false, $"File '{file.FileName}' is not a valid CSV file (executable header detected).");

            // PK (0x50, 0x4B, 0x03, 0x04) -> Zip
            if (read >= 4 && header[0] == 0x50 && header[1] == 0x4B && header[2] == 0x03 && header[3] == 0x04)
                return (false, $"File '{file.FileName}' is a ZIP archive, not a CSV file.");

            // OLE (0xD0, 0xCF, 0x11, 0xE0) -> OLE Compound / Excel binary
            if (read >= 4 && header[0] == 0xD0 && header[1] == 0xCF && header[2] == 0x11 && header[3] == 0xE0)
                return (false, $"File '{file.FileName}' is a binary document, not a CSV file.");

            // Read text content sample (first 64KB) to verify plain text and check for malicious script tags or formula execution injection
            stream.Position = 0;
            using var reader = new StreamReader(stream, Encoding.UTF8, true, 4096, true);
            char[] buffer = new char[Math.Min(stream.Length, 65536)];
            int charsRead = reader.Read(buffer, 0, buffer.Length);
            string contentSample = new string(buffer, 0, charsRead);

            // Check for embedded script execution
            var lowerSample = contentSample.ToLowerInvariant();
            if (lowerSample.Contains("<script") || lowerSample.Contains("javascript:") || lowerSample.Contains("vbscript:"))
            {
                return (false, $"File '{file.FileName}' contains potentially malicious script content.");
            }

            // Check for CSV Formula Injection / DDE commands
            var dangerousFormulaPrefixes = new[] { "=cmd|", "=cmd'", "@cmd|", "-cmd|", "+cmd|", "=dde(", "@dde(", "-dde(", "+dde(" };
            foreach (var prefix in dangerousFormulaPrefixes)
            {
                if (lowerSample.Contains(prefix))
                {
                    return (false, $"File '{file.FileName}' contains potentially malicious formula execution commands ({prefix}).");
                }
            }

            return (true, string.Empty);
        }

        public (bool IsValid, string ErrorMessage) ValidateTenderArchive(IFormFile file, bool isPriceBid = false)
        {
            if (file == null || file.Length == 0)
                return (false, "File is empty or not provided.");

            var nameCheck = ValidateFileNameSecurity(file.FileName);
            if (!nameCheck.IsValid)
                return (false, nameCheck.ErrorMessage);

            const long maxFileSize = 30 * 1024 * 1024;
            if (file.Length > maxFileSize)
                return (false, $"File '{file.FileName}' exceeds the maximum allowed size of 30 MB.");

            string ext = Path.GetExtension(file.FileName)?.ToLowerInvariant();
            var allowedExts = new[] { ".zip", ".rar", ".pdf", ".xlsx", ".xls", ".csv" };
            if (!allowedExts.Contains(ext))
                return (false, $"File '{file.FileName}' has an unsupported extension '{ext}'. Allowed formats: .pdf, .xlsx, .xls, .csv, .zip, .rar.");

            if (ext == ".pdf")
            {
                return ValidatePdfSignature(file);
            }

            if (ext == ".xlsx" || ext == ".xls")
            {
                return ValidateExcelSignature(file, ext);
            }

            if (ext == ".csv")
            {
                return ValidateCsvFile(file);
            }

            byte[] header = new byte[7];
            using (var stream = file.OpenReadStream())
            {
                int read = stream.Read(header, 0, header.Length);
                if (read < 4)
                    return (false, $"File '{file.FileName}' is corrupted or invalid.");

                bool isZip = header[0] == 0x50 && header[1] == 0x4B && header[2] == 0x03 && header[3] == 0x04;
                bool isRar = read >= 6 && header[0] == 0x52 && header[1] == 0x61 && header[2] == 0x72 && header[3] == 0x21 && header[4] == 0x1A && header[5] == 0x07;

                if (ext == ".zip" && !isZip)
                    return (false, $"File '{file.FileName}' is not a valid ZIP archive.");

                if (ext == ".rar" && !isRar)
                    return (false, $"File '{file.FileName}' is not a valid RAR archive.");
            }

            if (ext == ".zip")
            {
                try
                {
                    using (var stream = file.OpenReadStream())
                    using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
                    {
                        var allowedInnerExtensions = new[] { ".pdf", ".xlsx", ".xls", ".csv" };
                        var fileEntries = archive.Entries
                            .Where(e => !string.IsNullOrEmpty(e.Name) && !e.FullName.EndsWith("/") && !e.FullName.EndsWith("\\"))
                            .ToList();

                        if (fileEntries.Count == 0)
                            return (false, $"Archive '{file.FileName}' does not contain any files.");

                        long totalUncompressedSize = 0;
                        foreach (var entry in fileEntries)
                        {
                            totalUncompressedSize += entry.Length;
                            if (totalUncompressedSize > 150 * 1024 * 1024)
                            {
                                return (false, $"Archive '{file.FileName}' exceeds the maximum allowed uncompressed size.");
                            }

                            var entryNameCheck = ValidateFileNameSecurity(entry.Name);
                            if (!entryNameCheck.IsValid)
                            {
                                return (false, $"Archive '{file.FileName}' contains invalid file '{entry.Name}': {entryNameCheck.ErrorMessage}");
                            }

                            string innerExt = Path.GetExtension(entry.Name)?.ToLowerInvariant();
                            if (!allowedInnerExtensions.Contains(innerExt))
                            {
                                return (false, $"Archive '{file.FileName}' contains invalid file '{entry.FullName}'. Only .pdf, .xlsx, .xls, and .csv files are allowed inside the archive.");
                            }

                            using var entryStream = entry.Open();
                            byte[] entryHeader = new byte[4];
                            int entryRead = entryStream.Read(entryHeader, 0, entryHeader.Length);
                            if (entryRead >= 2 && entryHeader[0] == 0x4D && entryHeader[1] == 0x5A)
                            {
                                return (false, $"Archive '{file.FileName}' contains executable content in '{entry.Name}'.");
                            }
                        }

                        if (isPriceBid)
                        {
                            bool isArchivePasswordProtected = fileEntries.Any(e => (e.GetType().GetProperty("BitFlag")?.GetValue(e) is ushort flag && (flag & 1) != 0));
                            if (!isArchivePasswordProtected)
                            {
                                return (false, $"Price Bid archive '{file.FileName}' must be a password-protected archive.");
                            }
                        }
                    }
                }
                catch (InvalidDataException) when (isPriceBid)
                {
                    return (true, string.Empty);
                }
                catch (Exception ex)
                {
                    return (false, $"Could not read archive '{file.FileName}': {ex.Message}");
                }
            }
            else if (ext == ".rar" && isPriceBid)
            {
                bool isRarPasswordProtected = (header[0] == 0x52 && header[1] == 0x61 && header[2] == 0x72 && (header[6] & 0x80) != 0);
                if (!isRarPasswordProtected)
                {
                    return (false, $"Price Bid archive '{file.FileName}' must be a password-protected archive.");
                }
            }

            return (true, string.Empty);
        }

        public (bool IsValid, string ErrorMessage) ValidateBidDocument(IFormFile file, string docType)
        {
            if (file == null || file.Length == 0)
                return (false, "File is empty or not provided.");

            var nameCheck = ValidateFileNameSecurity(file.FileName);
            if (!nameCheck.IsValid)
                return (false, nameCheck.ErrorMessage);

            const long maxFileSize = 30 * 1024 * 1024;
            if (file.Length > maxFileSize)
                return (false, $"File '{file.FileName}' exceeds the maximum allowed size of 30 MB.");

            string ext = Path.GetExtension(file.FileName)?.ToLowerInvariant();
            bool isPriceBid = string.Equals(docType, "Price Bid", StringComparison.OrdinalIgnoreCase);

            if (isPriceBid)
            {
                if (ext != ".pdf" && ext != ".xlsx" && ext != ".xls" && ext != ".xlx")
                {
                    return (false, $"For Price Bid, only password-protected .pdf, .xls, and .xlsx files are allowed. Given file has extension '{ext}'.");
                }

                if (ext == ".pdf")
                {
                    return ValidatePasswordProtectedPdf(file);
                }
                else
                {
                    return ValidatePasswordProtectedExcel(file, ext);
                }
            }
            else
            {
                var allowedExts = new[] { ".zip", ".rar", ".pdf", ".xlsx", ".xls", ".xlx", ".csv" };
                if (!allowedExts.Contains(ext))
                {
                    return (false, $"File '{file.FileName}' has an unsupported extension '{ext}'. Allowed formats: .pdf, .xlsx, .xls, .csv, .zip, .rar.");
                }

                if (ext == ".zip" || ext == ".rar")
                {
                    return ValidateTenderArchive(file, false);
                }
                else if (ext == ".pdf")
                {
                    return ValidatePdfSignature(file);
                }
                else if (ext == ".csv")
                {
                    return ValidateCsvFile(file);
                }
                else
                {
                    return ValidateExcelSignature(file, ext);
                }
            }
        }

        private (bool IsValid, string ErrorMessage) ValidatePasswordProtectedPdf(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            byte[] header = new byte[5];
            int read = stream.Read(header, 0, header.Length);
            if (read < 4)
                return (false, $"File '{file.FileName}' is corrupted or not a valid PDF.");

            string pdfHeader = Encoding.ASCII.GetString(header, 0, read);
            if (!pdfHeader.StartsWith("%PDF"))
                return (false, $"File '{file.FileName}' is not a valid PDF file.");

            stream.Position = 0;
            using var reader = new StreamReader(stream, Encoding.ASCII, true, 8192, true);
            string fullContent = reader.ReadToEnd();

            bool isPasswordProtected = fullContent.Contains("/Encrypt") ||
                                       fullContent.Contains("/Standard") ||
                                       fullContent.Contains("/CFM");

            if (!isPasswordProtected)
            {
                return (false, $"Price Bid file '{file.FileName}' is not password protected. Please encrypt your PDF with a password before uploading.");
            }

            return (true, string.Empty);
        }

        private (bool IsValid, string ErrorMessage) ValidatePasswordProtectedExcel(IFormFile file, string ext)
        {
            using var stream = file.OpenReadStream();
            byte[] header = new byte[8];
            int bytesRead = stream.Read(header, 0, header.Length);
            if (bytesRead < 4)
                return (false, $"File '{file.FileName}' is corrupted or not a valid Excel file.");

            bool isZip = header[0] == 0x50 && header[1] == 0x4B && header[2] == 0x03 && header[3] == 0x04;
            bool isOle = bytesRead >= 8 &&
                         header[0] == 0xD0 && header[1] == 0xCF && header[2] == 0x11 && header[3] == 0xE0 &&
                         header[4] == 0xA1 && header[5] == 0xB1 && header[6] == 0x1A && header[7] == 0xE1;

            if (!isZip && !isOle)
            {
                return (false, $"File '{file.FileName}' is not a valid Excel document (.xls, .xlsx).");
            }

            // Case 1: .xlsx / .xlx saved with password to open is converted into an OLE Compound Document containing EncryptedPackage
            if ((ext == ".xlsx" || ext == ".xlx") && isOle)
            {
                stream.Position = 0;
                using var reader = new StreamReader(stream, Encoding.ASCII, true, 8192, true);
                string content = reader.ReadToEnd();
                if (content.Contains("EncryptedPackage") || content.Contains("EncryptionInfo"))
                {
                    return (true, string.Empty);
                }
                return (true, string.Empty);
            }

            // Case 2: .xls (Excel 97-2003 binary format)
            if (ext == ".xls" && isOle)
            {
                stream.Position = 0;
                using var reader = new StreamReader(stream, Encoding.ASCII, true, 8192, true);
                string content = reader.ReadToEnd();
                if (content.Contains("EncryptedPackage") || content.Contains("EncryptionInfo") || content.Contains("FILEPASS"))
                {
                    return (true, string.Empty);
                }

                stream.Position = 0;
                byte[] buffer = new byte[Math.Min(stream.Length, 65536)];
                int readLen = stream.Read(buffer, 0, buffer.Length);
                for (int i = 0; i < readLen - 4; i++)
                {
                    if (buffer[i] == 0x2F && buffer[i + 1] == 0x00)
                    {
                        return (true, string.Empty);
                    }
                }

                return (false, $"Price Bid Excel file '{file.FileName}' is not password protected. Please encrypt your Excel file with a password before uploading.");
            }

            // Case 3: .xlsx file as Zip archive -> check workbook / sheet protection
            if (isZip)
            {
                try
                {
                    stream.Position = 0;
                    using var archive = new ZipArchive(stream, ZipArchiveMode.Read, true);

                    bool hasEncryptedZipEntry = archive.Entries.Any(e =>
                        (e.GetType().GetProperty("BitFlag")?.GetValue(e) is ushort flag && (flag & 1) != 0));
                    if (hasEncryptedZipEntry)
                    {
                        return (true, string.Empty);
                    }

                    foreach (var entry in archive.Entries)
                    {
                        if (entry.FullName.StartsWith("xl/worksheets/sheet", StringComparison.OrdinalIgnoreCase) ||
                            entry.FullName.Equals("xl/workbook.xml", StringComparison.OrdinalIgnoreCase))
                        {
                            using var entryStream = entry.Open();
                            using var entryReader = new StreamReader(entryStream);
                            string xml = entryReader.ReadToEnd();
                            if (xml.Contains("<sheetProtection") || xml.Contains("<workbookProtection") || xml.Contains("password=") || xml.Contains("algorithmName="))
                            {
                                return (true, string.Empty);
                            }
                        }
                    }
                }
                catch (InvalidDataException)
                {
                    return (true, string.Empty);
                }
                catch
                {
                    // Continue
                }

                try
                {
                    stream.Position = 0;
                    using var wb = new ClosedXML.Excel.XLWorkbook(stream);
                    if (wb.IsProtected || wb.Worksheets.Any(ws => ws.IsProtected))
                    {
                        return (true, string.Empty);
                    }
                }
                catch
                {
                    if (isOle) return (true, string.Empty);
                }

                return (false, $"Price Bid Excel file '{file.FileName}' is not password protected. Please encrypt your Excel file with a password before uploading.");
            }

            return (false, $"Price Bid file '{file.FileName}' is not password protected. Please encrypt your Excel file with a password before uploading.");
        }

        private (bool IsValid, string ErrorMessage) ValidatePdfSignature(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            byte[] header = new byte[5];
            int read = stream.Read(header, 0, header.Length);
            if (read < 4 || !Encoding.ASCII.GetString(header, 0, read).StartsWith("%PDF"))
                return (false, $"File '{file.FileName}' is not a valid PDF file.");
            return (true, string.Empty);
        }

        private (bool IsValid, string ErrorMessage) ValidateExcelSignature(IFormFile file, string ext)
        {
            using var stream = file.OpenReadStream();
            byte[] header = new byte[8];
            int read = stream.Read(header, 0, header.Length);
            if (read < 4)
                return (false, $"File '{file.FileName}' is corrupted.");

            bool isZip = header[0] == 0x50 && header[1] == 0x4B && header[2] == 0x03 && header[3] == 0x04;
            bool isOle = read >= 8 &&
                         header[0] == 0xD0 && header[1] == 0xCF && header[2] == 0x11 && header[3] == 0xE0;

            if (!isZip && !isOle)
                return (false, $"File '{file.FileName}' is not a valid Excel file.");

            return (true, string.Empty);
        }
    }
}
