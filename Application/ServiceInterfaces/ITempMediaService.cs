using Application.Dtos;
using Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.ServiceInterfaces
{
    public interface ITempMediaService
    {
        Task<List<TempMediaVM>> Get();
        Task<TempMediaDTO> Get(int Id);
        Task<TempMediaDTO> Add(TempMediaDTO TemppressReleaseDTO);
        Task<TempMediaDTO> Update(TempMediaDTO TemppressReleaseDTO);
        Task<int> Remove(int id);
        Task<TempMediaDTO> CreateAudit(TempMediaDTO TemppressReleaseDTO);
        Task<List<TempMediaVM>> GetByAction(string status);
  
    }
}
