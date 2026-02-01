using Application.ViewModels;
using System.Threading.Tasks;

namespace Application.ServiceInterfaces
{
    public interface IPasswordHistoryService
    {
        Task<PasswordHistoryVM> GetByUsername(string username);
        Task<PasswordHistoryVM> CreateOrUpdate(PasswordHistoryVM argModelDto);
        Task<PasswordHistoryVM> GetByUsernamePwd(string username, string pwd);
    }
}
