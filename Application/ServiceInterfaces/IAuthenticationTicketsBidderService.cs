using Application.Dtos;
using System.Threading.Tasks;
namespace Application.ServiceInterfaces
{
    public interface IAuthenticationTicketsBidderService
    {
        //Common Methods
        Task<AuthenticationTicketsBidderDTO> Get(string id);
        Task<AuthenticationTicketsBidderDTO> Create(AuthenticationTicketsBidderDTO entity);
        Task<AuthenticationTicketsBidderDTO> Update(AuthenticationTicketsBidderDTO entity);
        Task<int> Delete(string id);
        Task<AuthenticationTicketsBidderDTO> Upsert(AuthenticationTicketsBidderDTO modelDto);
    }
}
