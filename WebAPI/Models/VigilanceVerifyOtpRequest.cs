// WebAPI/Models/VigilanceVerifyOtpRequest.cs
namespace WebAPI.Models
{
    public class VigilanceVerifyOtpRequest
    {
        public string Email { get; set; }
        public string Otp { get; set; }
    }
}