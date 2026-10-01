using Microsoft.AspNetCore.Http;
namespace Application.Dtos
{
    public class FileUploadDTO
    {
        public IFormFile UploadedFile { get; set; }
        public string FilePath { get; set; }
        public bool ChangeName { get; set; }
        public string ReturnValue { get; set; }
        public string FileOldName { get; set; }
        public bool ChangeDimensions { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
