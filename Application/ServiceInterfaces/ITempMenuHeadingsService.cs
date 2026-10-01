using Application.Dtos;
using Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.ServiceInterfaces
{
    public interface ITempMenuHeadingsService
    {
        //Common Methods
        Task<List<TempMenuHeadingsVM>> Get();
        Task<TempMenuHeadingsDTO> Get(int Id);
        Task<TempMenuHeadingsDTO> Add(TempMenuHeadingsDTO modelDto);
        Task<TempMenuHeadingsDTO> Update(TempMenuHeadingsDTO modelDto);
        Task<int> Remove(int id);
        //Custom Methods
        Task<TempMenuHeadingsDTO> CreateAudit(TempMenuHeadingsDTO modelDto);
        Task<List<TempMenuHeadingsVM>> GetByAction(string status);
        Task<List<TempMenuHeadingsVM>> GetByActionupdate(string status);
    }
}
