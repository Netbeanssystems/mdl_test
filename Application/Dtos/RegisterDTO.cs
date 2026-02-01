using Application.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos
{
    public class RegisterDTO
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public string Role { get; set; }

        //[Required(ErrorMessage = "{0} is required")]
        //[StringLength(32, ErrorMessage = "{1} characters max")]
        public string Username { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(64, ErrorMessage = "{1} characters max")]
        [RegularExpression("^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9]+)*\\.([a-z]{2,4})$", ErrorMessage = "Enter a valid email id")]
        public string Email { get; set; }

        //[Required(ErrorMessage = "{0} is required")]
        //[DataType(DataType.Password)]
        //[DisplayName("Password")]
        //[StringLength(32, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
        public string Password { get; set; }

        //[Required(ErrorMessage = "{0} is required")]
        //[DataType(DataType.Password)]
        //[DisplayName("Confirm password")]
        //[Compare("Password", ErrorMessage = "The password and confirmation password must match.")]
        //public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(15, ErrorMessage = "{1} characters max")]
        [RegularExpression(@"^\d{10,15}$", ErrorMessage = "Enter a valid number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(128, ErrorMessage = "{1} characters max")]
        public string Name { get; set; }

        //[Required(ErrorMessage = "{0} is required")]
        [StringLength(128, ErrorMessage = "{1} characters max")]
        public string BranchName { get; set; }

        //[Required(ErrorMessage = "{0} is required")]
        public string BranchAddress { get; set; }

        [StringLength(64, ErrorMessage = "{1} characters max")]
        public string ProfileImage { get; set; }

        public string ProjectId { get; set; }
        public List<int> ProjectIds { get; set; }

        public string YardId { get; set; }
        public List<int> YardIds { get; set; }
        [StringLength(50, ErrorMessage = "{1} characters max")]
        public string ExPNo { get; set; }
        [StringLength(50, ErrorMessage = "{1} characters max")]
        public string ExtensionNo { get; set; }
        [StringLength(50, ErrorMessage = "{1} characters max")]
        public string Designation { get; set; }
        [StringLength(50, ErrorMessage = "{1} characters max")]
        public string Department { get; set; }

        [StringLength(100, ErrorMessage = "{1} characters max")]
        public string OrganizationName { get; set; }

        [StringLength(200, ErrorMessage = "{1} characters max")]
        public string Address { get; set; }
        public string Country { get; set; }
        public DateTime? LoginValidFrom { get; set; }
        public DateTime? LoginValidTill { get; set; }

        public bool Approved { get; set; }

        public bool IsActive { get; set; }

        public bool ChangePassword { get; set; }

        //[DisplayName("Captcha Code")]
        //[Required(ErrorMessage = "{0} is required")]
        //[StringLength(4)]
        //public string CaptchaCode { get; set; }

        public string Pwd_SHA512 { get; set; }

        public string EncSecret => string.IsNullOrEmpty(Password) ? null : Convert.ToBase64String(EnDeCryptor.EncryptStringAES(Password));
    }
}
