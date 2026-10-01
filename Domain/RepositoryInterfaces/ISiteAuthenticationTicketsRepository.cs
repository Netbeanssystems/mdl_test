using Domain.Models;
using System.Threading.Tasks;

namespace Domain.RepositoryInterfaces
{
    public interface ISiteAuthenticationTicketsRepository : IRepository<SiteAuthenticationTickets>
    {
        Task<SiteAuthenticationTickets> GetByIdStr(string Id);
        Task<SiteAuthenticationTickets> GetByUserId(string UserId);
    }
}
