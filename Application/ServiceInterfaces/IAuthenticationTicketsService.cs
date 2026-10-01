using Application.Dtos;
using System.Threading.Tasks;
namespace Application.ServiceInterfaces
{
    public interface IAuthenticationTicketsService
    {
        //Common Methods
        Task<AuthenticationTicketsDTO> Get(string id);
        Task<AuthenticationTicketsDTO> Create(AuthenticationTicketsDTO entity);
        Task<AuthenticationTicketsDTO> Update(AuthenticationTicketsDTO entity);
        Task<int> Delete(string id);
        Task<AuthenticationTicketsDTO> Upsert(AuthenticationTicketsDTO modelDto);
    }
}
