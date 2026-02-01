using Application.Dtos;
using Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.ServiceInterfaces
{
    public interface ITempBannerService
    {
        //Common Methods
        Task<List<TempBannerVM>> Get();
        Task<TempBannerDTO> Get(int Id);
        Task<TempBannerDTO> Add(TempBannerDTO modelDto);
        Task<TempBannerDTO> Update(TempBannerDTO modelDto);
        Task<int> Remove(int id);

        //Custom Methods
        Task<TempBannerDTO> CreateAudit(TempBannerDTO modelDto);
        Task<List<TempBannerVM>> GetByAction(string status);
       
    }
}
