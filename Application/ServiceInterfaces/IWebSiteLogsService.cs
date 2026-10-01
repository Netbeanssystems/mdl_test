using Application.Dtos;
using Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Application.ServiceInterfaces
{
    public interface IWebSiteLogsService
    {
        //Common Methods
        Task<List<WebSiteLogsVM>> Get();
        Task<WebSiteLogsDTO> Get(int id);
        Task<WebSiteLogsVM> GetVM(int id);
        Task<WebSiteLogsDTO> Create(WebSiteLogsDTO entity);
        Task<WebSiteLogsDTO> Update(WebSiteLogsDTO entity);
        Task<int> Delete(int id);
        Task<List<WebSiteLogsDTO>> CreateRange(List<WebSiteLogsDTO> entities);
        Task<List<WebSiteLogsDTO>> Upsert(List<WebSiteLogsDTO> entities);
        Task<int> DeleteRange(List<WebSiteLogsDTO> entities);
    }
}
