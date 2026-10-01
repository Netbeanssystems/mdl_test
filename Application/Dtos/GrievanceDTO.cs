using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos
{
    public class GrievanceDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(64, ErrorMessage = "{1} characters max")]
        [RegularExpression(@"^[a-zA-Z ]*$", ErrorMessage = "The Name field should accept only the alphabet.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(64, ErrorMessage = "{1} characters max")]
        [RegularExpression(@"^[a-zA-Z ]*$", ErrorMessage = "The Designation field should accept only the alphabet.")]
        public string Designation { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(64, ErrorMessage = "{1} characters max")]
        [RegularExpression(@"^[a-zA-Z ]*$", ErrorMessage = "The Organization field should accept only the alphabet.")]
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


        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Enter a valid Fax number")]
        public string Fax { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(64, ErrorMessage = "{1} characters max")]
        [RegularExpression(@"^[a-zA-Z ]*$", ErrorMessage = "The City field should accept only the alphabet.")]
        public string City { get; set; }

        [DisplayName("Pin Code")]
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(6, ErrorMessage = "{1} characters max")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Enter a valid 6 digit Pincode")]
        public string PinCode { get; set; }

        [Display(Name = "Country")]
        [Required(ErrorMessage = "{0} is required")]
        public string CountryId { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(240, ErrorMessage = "{1} characters max")]
        [RegularExpression(@"^[a-zA-Z0-9 ,/-]*$", ErrorMessage = "Only Alphabets and numbers allowed")]
        public string Address { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(1000, ErrorMessage = "{1} characters max")]
        [RegularExpression(@"^[a-zA-Z0-9 ,/-]*$", ErrorMessage = "Only Alphabets and numbers allowed")]
        public string Grievance { get; set; }
        public DateTime SubmitOn { get; set; }
        public string IP { get; set; }

        [DisplayName("Captcha Code")]
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(6)]
        public string CaptchaCode { get; set; }
    }
}
