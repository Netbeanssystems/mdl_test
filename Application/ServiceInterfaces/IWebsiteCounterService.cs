using Application.Dtos;
using System.Threading.Tasks;

namespace Application.ServiceInterfaces
{
    public interface IWebsiteCounterService
    {
        Task<WebsiteCounterDTO> Get(int id);
        Task<WebsiteCounterDTO> Update(WebsiteCounterDTO entity);
    }
}
