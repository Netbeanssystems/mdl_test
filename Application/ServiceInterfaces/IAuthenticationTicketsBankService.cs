using Application.Dtos;
using System.Threading.Tasks;
namespace Application.ServiceInterfaces
{
    public interface IAuthenticationTicketsBankService
    {
        //Common Methods
        Task<AuthenticationTicketsBankDTO> Get(string id);
        Task<AuthenticationTicketsBankDTO> Create(AuthenticationTicketsBankDTO entity);
        Task<AuthenticationTicketsBankDTO> Update(AuthenticationTicketsBankDTO entity);
        Task<int> Delete(string id);
        Task<AuthenticationTicketsBankDTO> Upsert(AuthenticationTicketsBankDTO modelDto);
    }
}
