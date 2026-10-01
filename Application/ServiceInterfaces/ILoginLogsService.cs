using Application.Dtos;
using Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Application.ServiceInterfaces
{
    public interface ILoginLogsService
    {
        //Common Methods
        Task<List<LoginLogsVM>> Get();
        Task<LoginLogsDTO> Get(int id);
        Task<LoginLogsVM> GetVM(int id);
        Task<LoginLogsDTO> Create(LoginLogsDTO entity);
        Task<LoginLogsDTO> Update(LoginLogsDTO entity);
        Task<int> Delete(int id);
        Task<List<LoginLogsDTO>> CreateRange(List<LoginLogsDTO> entities);
        Task<List<LoginLogsDTO>> Upsert(List<LoginLogsDTO> entities);
        Task<int> DeleteRange(List<LoginLogsDTO> entities);
    }
}
