using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Application.Dtos
{
    public class ModelWithFile //: BaseDTO
    {
        public IFormFile UploadedFile { get; set; }
        public List<IFormFile> ListUploadedFile { get; set; }
        public string Data { get; set; }
    }
}
