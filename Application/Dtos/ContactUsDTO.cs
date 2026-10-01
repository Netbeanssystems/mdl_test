using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Application.Dtos
{
    public class ContactUsDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(64, ErrorMessage = "{1} characters max")]
        [RegularExpression(@"^[a-zA-Z]*$", ErrorMessage = "The First Name field should accept only the alphabet.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(64, ErrorMessage = "{1} characters max")]
        [RegularExpression(@"^[a-zA-Z]*$", ErrorMessage = "The Last Name field should accept only the alphabet.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(64, ErrorMessage = "{1} characters max")]
        [RegularExpression(@"^[a-zA-Z ]*$", ErrorMessage = "The Company Name field should accept only the alphabet.")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(64, ErrorMessage = "{1} characters max")]
        [RegularExpression(@"^[a-zA-Z]*$", ErrorMessage = "The Country Name field should accept only the alphabet.")]
        public string CountryName { get; set; }


        public string InterestedIn { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(64, ErrorMessage = "{1} characters max")]
        //[RegularExpression("^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9]+)*\\.([a-z]{2,4})$", ErrorMessage = "Enter a valid email id")]
        [RegularExpression("^[a-zA-Z0-9][a-zA-Z0-9._-]*[a-zA-Z0-9]?@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,3}$", ErrorMessage = "Enter a valid email id")]
        public string EmailId { get; set; }
        public string ToEmailId { get; set; }

        [DisplayName("Contact No.")]
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(10, ErrorMessage = "{1} characters max")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "The Contact No field should accept a valid 10 digit number")]
        public string Contact { get; set; }

        [DisplayName("Message")]
        [Required(ErrorMessage = "{0} is required")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "The Queries field should accept only Alphabets and numbers")]
        [StringLength(1000, ErrorMessage = "{1} characters max")]
        public string Message { get; set; }
        public DateTime SubmitOn { get; set; }
        public string IP { get; set; }
        public string SubmitedFrom { get; set; }

        // ✅ ADD THIS (IMPORTANT)
        [NotMapped]
        public string CaptchaCode { get; set; }
    }
}
