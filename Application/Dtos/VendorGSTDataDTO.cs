using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos
{
    public class VendorGSTDataDTO
    {
        public string VendorCode { get; set; }
        public string VenderName { get; set; }
        public string VenderAddress { get; set; }
        public string Permanent_Account_Number { get; set; }
        public string Email_ID { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(16, ErrorMessage = "{1} characters max")]
        public string GST_Number { get; set; }
        public string GST_FileName { get; set; }
        public IFormFile GST_File { get; set; }
        public int Id { get; set; }
    }

    public class VendorGSTPostDTO
    {
        public int Id { get; set; }
        public string GST_Number { get; set; }
        public string GST_FileName { get; set; }
        public string IP { get; set; }
    }
}
