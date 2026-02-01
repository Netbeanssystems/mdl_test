using Domain.Models;
using System.Threading.Tasks;

namespace Domain.RepositoryInterfaces
{
    public interface IAuthenticationTicketsBankRepository : IRepository<AuthenticationTicketsBank>
    {
        Task<AuthenticationTicketsBank> GetByIdStr(string Id);
        Task<AuthenticationTicketsBank> GetByUserId(string UserId);
    }
}
