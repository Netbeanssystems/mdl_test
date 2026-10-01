using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos
{
    public class VendorGSTDTO
    {
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(9, ErrorMessage = "{1} characters max")]
        //[RegularExpression(@"^\d{10}$", ErrorMessage = "Enter a valid Vendor Code")]
        public string VendorCode { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(64, ErrorMessage = "{1} characters max")]
        public string PANNo { get; set; }

        [DisplayName("Captcha Code")]
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(6)]
        public string CaptchaCode { get; set; }
    }
}
