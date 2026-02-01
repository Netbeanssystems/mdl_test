using Application.Helpers;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos
{
    public class ChangePasswordDTO
    {
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(32, ErrorMessage = "{1} characters max")]
        public string Username { get; set; }

        [DisplayName("Current Password")]
        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.Password)]
        [StringLength(32, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*?[a-z])(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[!#\$%&'\(\)\*\+,-\.\/:;<=>\?@[\]\^_`\{\|}~])[a-zA-Z0-9!#\$%&'\(\)\*\+,-\.\/:;<=>\?@[\]\^_`\{\|}~]{0,}$", ErrorMessage = "The password must contain atleast one uppercase, one lowercase, one digit and one special character")]
        public string Password { get; set; }

        [DisplayName("New Password")]
        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.Password)]
        [StringLength(32, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*?[a-z])(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[!#\$%&'\(\)\*\+,-\.\/:;<=>\?@[\]\^_`\{\|}~])[a-zA-Z0-9!#\$%&'\(\)\*\+,-\.\/:;<=>\?@[\]\^_`\{\|}~]{0,}$", ErrorMessage = "The password must contain atleast one uppercase, one lowercase, one digit and one special character")]
        public string NewPassword { get; set; }

        [DisplayName("Confirm password")]
        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string EncPassword { get; set; }
        public string EncNewPassword { get; set; }
        public string EncConfirmPassword { get; set; }

        public string EncSecret => string.IsNullOrEmpty(NewPassword) ? null : Convert.ToBase64String(EnDeCryptor.EncryptStringAES(NewPassword));
    }
}
