using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos
{
    public class FeedbackDTO
    {
        public int Id { get; set; }
        public int RatingNo1 { get; set; }
        public int RatingNo2 { get; set; }
        public int RatingNo3 { get; set; }
        public int RatingNo4 { get; set; }
        public int RatingNo5 { get; set; }
        public int RatingNo6 { get; set; }
        public int RatingNo7 { get; set; }
        public int RatingNo8 { get; set; }

        [DisplayName("Satisfied Reason")]
        [Required(ErrorMessage = "{0} is required")]
        [RegularExpression(@"^[a-zA-Z0-9 ,/-]*$", ErrorMessage = "Only Alphabets and numbers allowed")]
        [StringLength(500, ErrorMessage = "{1} characters max")]
        public string ReasonSatisfied { get; set; }

        [DisplayName("DisSatisfied Reason")]
        [Required(ErrorMessage = "{0} is required")]
        [RegularExpression(@"^[a-zA-Z0-9 ,/-]*$", ErrorMessage = "Only Alphabets and numbers allowed")]
        [StringLength(500, ErrorMessage = "{1} characters max")]
        public string ReasonDisSatisfied { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [RegularExpression(@"^[a-zA-Z ]*$", ErrorMessage = "Only Alphabets are allowed")]
        [StringLength(64, ErrorMessage = "{1} characters max")]
        public string Name { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Only Alphabets are allowed")]
        [StringLength(64, ErrorMessage = "{1} characters max")]
        public string Designation { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(64, ErrorMessage = "{1} characters max")]
        [RegularExpression(@"^[a-zA-Z ]*$", ErrorMessage = "Only Alphabets are allowed")]
        public string Organization { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(64, ErrorMessage = "{1} characters max")]
        [RegularExpression(@"^[a-zA-Z ]*$", ErrorMessage = "Only Alphabets are allowed")]
        public string City { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(100, ErrorMessage = "{1} characters max")]
        [RegularExpression("^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9]+)*\\.([a-z]{2,4})$", ErrorMessage = "Enter a valid email id")]
        public string Email { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(240, ErrorMessage = "{1} characters max")]
        [RegularExpression(@"^[a-zA-Z0-9 ,/-]*$", ErrorMessage = "Only Alphabets and numbers allowed")]
        public string Address { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(500, ErrorMessage = "{1} characters max")]
        [RegularExpression(@"^[a-zA-Z0-9 ,/-]*$", ErrorMessage = "Only Alphabets and numbers allowed")]
        public string Comment { get; set; }
        public DateTime SubmitOn { get; set; }
        public string IP { get; set; }

        [DisplayName("Captcha Code")]
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(6)]
        public string CaptchaCode { get; set; }
    }

    public class TestDTO
    {
        public string City { get; set; }
    }
}
