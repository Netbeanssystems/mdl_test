using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
namespace Application.Dtos
{
    public class MultipleFilesUploadDTO : FileUploadDTO
    {
        public int Id { get; set; }
        public string FileType { get; set; }
        public List<IFormFile> UploadedFiles { get; set; }
    }
}
