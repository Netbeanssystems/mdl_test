using Application.Dtos;
using Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Application.ServiceInterfaces
{
    public interface IWebBankLogsService
    {
        //Common Methods
        Task<List<WebBankLogsVM>> Get();
        Task<WebBankLogsDTO> Get(int id);
        Task<WebBankLogsVM> GetVM(int id);
        Task<WebBankLogsDTO> Create(WebBankLogsDTO entity);
        Task<WebBankLogsDTO> Update(WebBankLogsDTO entity);
        Task<int> Delete(int id);
        Task<List<WebBankLogsDTO>> CreateRange(List<WebBankLogsDTO> entities);
        Task<List<WebBankLogsDTO>> Upsert(List<WebBankLogsDTO> entities);
        Task<int> DeleteRange(List<WebBankLogsDTO> entities);
    }
}
