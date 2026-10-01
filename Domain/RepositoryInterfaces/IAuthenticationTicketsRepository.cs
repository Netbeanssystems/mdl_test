using Domain.Models;
using System.Threading.Tasks;

namespace Domain.RepositoryInterfaces
{
    public interface IAuthenticationTicketsRepository : IRepository<AuthenticationTickets>
    {
        Task<AuthenticationTickets> GetByIdStr(string Id);
        Task<AuthenticationTickets> GetByUserId(string UserId);
    }
}
