using Application.Dtos;
using Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Application.ServiceInterfaces
{
    public interface IWebApiLogsService
    {
        //Common Methods
        Task<List<WebApiLogsVM>> Get();
        Task<WebApiLogsDTO> Get(int id);
        Task<WebApiLogsVM> GetVM(int id);
        Task<WebApiLogsDTO> Create(WebApiLogsDTO entity);
        Task<WebApiLogsDTO> Update(WebApiLogsDTO entity);
        Task<int> Delete(int id);
        Task<List<WebApiLogsDTO>> CreateRange(List<WebApiLogsDTO> entities);
        Task<List<WebApiLogsDTO>> Upsert(List<WebApiLogsDTO> entities);
        Task<int> DeleteRange(List<WebApiLogsDTO> entities);
    }
}
