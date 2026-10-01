using Application.Dtos;
using System.Threading.Tasks;

namespace Application.ServiceInterfaces
{
    public interface IVigilanceFormService
    {
        // Existing
        Task<VigilanceFormDTO> Create(VigilanceFormDTO argModelDto);
        // Application/ServiceInterfaces/IVigilanceFormService.cs

        // NEW — OTP flow
        Task<(bool success, string message, int? resendLeft)> SubmitEmail(string email);
        Task<(bool success, string message, int? attemptsLeft)> VerifyOtp(string email, string otp);
        Task<(bool success, string message, int? resendLeft)> ResendOtp(string email);

    }
}
