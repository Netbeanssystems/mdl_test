using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos
{
    public class EnquiryDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(64, ErrorMessage = "{1} characters max")]
        [RegularExpression(@"^[a-zA-Z]*$", ErrorMessage = "The Name field should accept only the alphabet.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(64, ErrorMessage = "{1} characters max")]
        [RegularExpression(@"^[a-zA-Z]*$", ErrorMessage = "The Designation field should accept only the alphabet.")]
        public string Designation { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(64, ErrorMessage = "{1} characters max")]
        [RegularExpression(@"^[a-zA-Z]*$", ErrorMessage = "The Organization field should accept only the alphabet.")]
        public string Organization { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(64, ErrorMessage = "{1} characters max")]
        [RegularExpression("^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9]+)*\\.([a-z]{2,4})$", ErrorMessage = "Enter a valid email id")]
        public string Email { get; set; }

        [DisplayName("Contact No.")]
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(10, ErrorMessage = "{1} characters max")]
        // [RegularExpression(@"^\d{10}$", ErrorMessage = "The Contact No field should accept a valid 10 digit number")]
        [RegularExpression(@"^(?!0{9,})\d{10}$", ErrorMessage = "Please enter a valid 10-digit number that is not all zeros.")]

        public string Contact { get; set; }

        //[DisplayName("Fax No.")]
        ////[Required(ErrorMessage = "{0} is required")]
        //[StringLength(10, ErrorMessage = "{1} characters max")]
        ////[RegularExpression(@"^\d{10}$", ErrorMessage = "Enter a valid 10 digit Fax number")]
        //[RegularExpression(@"^(?!0{9,})\d{10}$", ErrorMessage = "Please enter a valid Fax No. that is not all zeros.")]
        ////public string Fax { get; set; }
        //public int? Fax { get; set; }

        [DisplayName("Fax No.")]
        [StringLength(10, ErrorMessage = "{1} characters max")]
        [RegularExpression(@"^(?!0{9,})\d{10}$", ErrorMessage = "Please enter a valid Fax No. that is not all zeros.")]
        public string Fax { get; set; }


        [DisplayName("Postal Address")]
        [Required(ErrorMessage = "{0} is required")]
        [RegularExpression(@"^[a-zA-Z0-9 ,/-]*$", ErrorMessage = "The Postal Address field should accept only Alphabets and numbers")]
        [StringLength(240, ErrorMessage = "{1} characters max")]
        public string PostalAddress { get; set; }

        [DisplayName("Query")]
        [Required(ErrorMessage = "{0} is required")]
        [RegularExpression(@"^[a-zA-Z0-9 ,/-]*$", ErrorMessage = "The Queries field should accept only Alphabets and numbers")]
        [StringLength(1000, ErrorMessage = "{1} characters max")]
        public string ComplaintDetails { get; set; }
        public int? ShipBuildRelated { get; set; }
        public int? SubHeavyRelated { get; set; }
        public DateTime SubmitOn { get; set; }
        public string IP { get; set; }

        [DisplayName("Captcha Code")]
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(6)]
        public string CaptchaCode { get; set; }
    }
}
