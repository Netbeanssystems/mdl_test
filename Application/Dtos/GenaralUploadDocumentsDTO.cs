using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
namespace Application.Dtos
{
    public class GenaralUploadDocumentsDTO : BaseDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(64, ErrorMessage = "{1} characters max")]
        public string Description { get; set; }
        public string DocumentName { get; set; }

        public IFormFile DocumentFile { get; set; }
    }
}
