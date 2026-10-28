using Application.Dtos;
using System.Threading.Tasks;

namespace Application.ServiceInterfaces
{
    public interface IVigilanceFormService
    {
        Task<VigilanceFormDTO> Create(VigilanceFormDTO argModelDto);

        Task<(bool success, string message, string otp, int? resendLeft)> SubmitEmail(string email);

        Task<(bool success, string message, int? attemptsLeft)> VerifyOtp(string email, string otp);

        Task<(bool success, string message, string otp, int? resendLeft)> ResendOtp(string email);
    }
}