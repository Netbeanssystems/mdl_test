using Application.Helpers;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos
{
    public class ResetPasswordDTO
    {
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(64, ErrorMessage = "{1} characters max")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.Password)]
        [StringLength(32, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*?[a-z])(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[!#\$%&'\(\)\*\+,-\.\/:;<=>\?@[\]\^_`\{\|}~])[a-zA-Z0-9!#\$%&'\(\)\*\+,-\.\/:;<=>\?@[\]\^_`\{\|}~]{0,}$", ErrorMessage = "The password must contain atleast one uppercase, one lowercase, one digit and one special character")]
        public string Password { get; set; }

        [DisplayName("Confirm password")]
        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string EncPassword { get; set; }

        [DisplayName("Captcha Code")]
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(6)]
        public string CaptchaCode { get; set; }

        public string Code { get; set; }

        public string EncSecret => string.IsNullOrEmpty(Password) ? null : Convert.ToBase64String(EnDeCryptor.EncryptStringAES(Password));
    }
}