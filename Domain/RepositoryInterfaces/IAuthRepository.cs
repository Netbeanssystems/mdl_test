using Domain.Models;
using System.Threading.Tasks;
namespace Domain.RepositoryInterfaces
{
    public interface IAuthRepository
    {
        Task<bool> ResetPassword(string username, string newPassword);
    }
}
