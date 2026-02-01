using Application.Dtos;
using Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.ServiceInterfaces
{
    public interface ITempNewsService
    {
        //Common Methods
        Task<List<TempNewsVM>> Get();
        Task<TempNewsDTO> Get(int Id);
        Task<TempNewsDTO> Add(TempNewsDTO tempnewsDto);
        Task<TempNewsDTO> Update(TempNewsDTO tempnewsDto);
        Task<int> Remove(int id);
        //Custom Methods
        Task<TempNewsDTO> CreateAudit(TempNewsDTO tempnewsDto);
        Task<List<TempNewsVM>> GetByAction(string status);
    }
}
