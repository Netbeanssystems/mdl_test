using Application.Dtos;
using System.Threading.Tasks;
namespace Application.ServiceInterfaces
{
    public interface ISiteAuthenticationTicketsService
    {
        //Common Methods
        Task<SiteAuthenticationTicketsDTO> Get(string id);
        Task<SiteAuthenticationTicketsDTO> Create(SiteAuthenticationTicketsDTO entity);
        Task<SiteAuthenticationTicketsDTO> Update(SiteAuthenticationTicketsDTO entity);
        Task<int> Delete(string id);
        Task<SiteAuthenticationTicketsDTO> Upsert(SiteAuthenticationTicketsDTO modelDto);
    }
}
