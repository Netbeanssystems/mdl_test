using Application.Dtos;
using Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Application.ServiceInterfaces
{
    public interface IWebAppLogsService
    {
        //Common Methods
        Task<List<WebAppLogsVM>> Get();
        Task<WebAppLogsDTO> Get(int id);
        Task<WebAppLogsVM> GetVM(int id);
        Task<WebAppLogsDTO> Create(WebAppLogsDTO entity);
        Task<WebAppLogsDTO> Update(WebAppLogsDTO entity);
        Task<int> Delete(int id);
        Task<List<WebAppLogsDTO>> CreateRange(List<WebAppLogsDTO> entities);
        Task<List<WebAppLogsDTO>> Upsert(List<WebAppLogsDTO> entities);
        Task<int> DeleteRange(List<WebAppLogsDTO> entities);
    }
}
