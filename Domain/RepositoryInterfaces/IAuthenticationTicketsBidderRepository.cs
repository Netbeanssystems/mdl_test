using Domain.Models;
using System.Threading.Tasks;

namespace Domain.RepositoryInterfaces
{
    public interface IAuthenticationTicketsBidderRepository : IRepository<AuthenticationTicketsBidder>
    {
        Task<AuthenticationTicketsBidder> GetByIdStr(string Id);
        Task<AuthenticationTicketsBidder> GetByUserId(string UserId);
    }
}
