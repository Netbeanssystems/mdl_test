using Application.Dtos;
using Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.ServiceInterfaces
{
    public interface ITempVideoService
    {
        Task<List<TempVideoVM>> Get();
        Task<TempVideoDTO> Get(int Id);
        Task<TempVideoDTO> Add(TempVideoDTO TempVideoDto);
        Task<TempVideoDTO> Update(TempVideoDTO TempVideoDto);
        Task<List<TempVideoVM>> GetByAction(string status);
        Task<int> Remove(int id);
        Task<TempVideoDTO> CreateAudit(TempVideoDTO TempVideoDto);
    }
}
