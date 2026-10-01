using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos
{
    public class VigilanceFormDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(64, ErrorMessage = "{1} characters max")]
        public string Name { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(64, ErrorMessage = "{1} characters max")]
        public string Designation { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(64, ErrorMessage = "{1} characters max")]
        public string Organization { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(64, ErrorMessage = "{1} characters max")]
        [RegularExpression("^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9]+)*\\.([a-z]{2,4})$", ErrorMessage = "Enter a valid email id")]
        public string Email { get; set; }

        [DisplayName("Contact No.")]
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(10, ErrorMessage = "{1} characters max")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Enter a valid 10 digit number")]
        public string Contact { get; set; }
        public int? Fax { get; set; }

        [DisplayName("Postal Address")]
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(240, ErrorMessage = "{1} characters max")]
        public string PostalAddress { get; set; }

        [DisplayName("Complaint Details")]
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(5000, ErrorMessage = "{1} characters max")]
        public string ComplaintDetails { get; set; }
        public int? ShipBuildRelated { get; set; }
        public int? SubHeavyRelated { get; set; }
        public DateTime SubmitOn { get; set; }
        public string IP { get; set; }

        [DisplayName("Captcha Code")]
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(6)]
        public string CaptchaCode { get; set; }

        public string UploadFileName { get; set; }
        public IFormFile UploadFile { get; set; }
    }
}