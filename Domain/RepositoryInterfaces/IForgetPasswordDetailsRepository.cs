using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryInterfaces
{
    public interface IForgetPasswordDetailsRepository : IRepository<ForgetPasswordDetails>
    {
        Task<List<ForgetPasswordDetails>> CheckUserDetails(string userid, string date);
    }
}
