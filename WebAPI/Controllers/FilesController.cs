using Application.Dtos;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class FilesController : Controller
    {
        private readonly IFileService _fileService;
        public FilesController(IFileService fileService)
        {
            _fileService = fileService;
        }
        //Method accepting MultipartFormData with IFormFile
        // POST: Files/UploadFile
        //[Authorize]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> UploadFile()
        {
            if (Request.Form.Files.Count <= 0) return Ok(null);
            var FormFile = Request.Form.Files[0];
            if (FormFile.Length <= 0 || !_fileService.CheckFileSize(FormFile)) return Ok(null);
            var ChangeDimensions = Convert.ToBoolean(Request.Form["ChangeDimensions"]);
            var FileUploadDto = new FileUploadDTO
            {
                UploadedFile = FormFile,
                FilePath = Request.Form["FilePath"].ToString(),
                FileOldName = Request.Form["FileOldName"].ToString(),
                ChangeName = Convert.ToBoolean(Request.Form["ChangeName"]),
                ReturnValue = Request.Form["ReturnValue"].ToString(),
                ChangeDimensions = ChangeDimensions,
                Width = ChangeDimensions ? Convert.ToInt32(Request.Form["Width"]) : 0,
                Height = ChangeDimensions ? Convert.ToInt32(Request.Form["Height"]) : 0
            };

            //Delete previous file
            _fileService.DeleteFile(!string.IsNullOrEmpty(FileUploadDto.FileOldName)
                ? $"{FileUploadDto.FilePath}\\{FileUploadDto.FileOldName}"
                : $"{FileUploadDto.FilePath}\\{FileUploadDto.UploadedFile.FileName}");
            //Save file
            var NameOrPath = await _fileService.SaveFileAsync(FileUploadDto).ConfigureAwait(false);
            return Ok(NameOrPath);
        }
        //Method accepting MultipartFormData with IFormFile
        // POST: Files/UploadFile
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UploadFiles()
        {
            if (Request.Form.Files.Count <= 0) return Ok(null);
            var NameOrPathList = new List<string>();
            foreach (var FormFile in Request.Form.Files)
            {
                if (FormFile == null || FormFile.Length <= 0 || !_fileService.CheckFileSize(FormFile)) continue;
                var ChangeDimensions = Convert.ToBoolean(Request.Form["ChangeDimensions"]);
                var FileUploadDto = new FileUploadDTO
                {
                    UploadedFile = FormFile,
                    FilePath = Request.Form["FilePath"].ToString(),
                    FileOldName = Request.Form["FileOldName"].ToString(),
                    ChangeName = Convert.ToBoolean(Request.Form["ChangeName"]),
                    ReturnValue = Request.Form["ReturnValue"].ToString(),
                    ChangeDimensions = ChangeDimensions,
                    Width = ChangeDimensions ? Convert.ToInt32(Request.Form["Width"]) : 0,
                    Height = ChangeDimensions ? Convert.ToInt32(Request.Form["Height"]) : 0
                };

                //Delete previous file
                _fileService.DeleteFile(!string.IsNullOrEmpty(FileUploadDto.FileOldName)
                    ? $"{FileUploadDto.FilePath}\\{FileUploadDto.FileOldName}"
                    : $"{FileUploadDto.FilePath}\\{FileUploadDto.UploadedFile.FileName}");

                //Save file
                var NameOrPath = await _fileService.SaveFileAsync(FileUploadDto).ConfigureAwait(false);
                NameOrPathList.Add(NameOrPath);
            }
            return Ok(NameOrPathList);
        }
        // GET: Files/DeleteFile
        //[Authorize]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult DeleteFile([FromBody] FileUploadDTO model)
        {
            var deleted = _fileService.DeleteFile(model.FilePath);
            return Ok(deleted);
        }
    }
}
