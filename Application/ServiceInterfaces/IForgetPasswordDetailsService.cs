using Application.Dtos;
using Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.ServiceInterfaces
{
    public interface IForgetPasswordDetailsService
    {
        Task<List<ForgetPasswordDetailsVM>> Get();
        Task<ForgetPasswordDetailsDTO> Get(string id);
        Task<ForgetPasswordDetailsDTO> Create(ForgetPasswordDetailsDTO argModelDto);
        Task<ForgetPasswordDetailsDTO> Update(ForgetPasswordDetailsDTO argModelDto);
        Task<int> Delete(string id);
        Task<List<ForgetPasswordDetailsDTO>> CheckUserDetails(string userid, string date);
    }
}
