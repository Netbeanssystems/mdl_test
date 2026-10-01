using Application.Dtos;
using Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.ServiceInterfaces
{
    public interface IBannerService
    {
        //Common Methods
        Task<List<BannerVM>> Get();
        Task<BannerDTO> Add(BannerDTO modelDto);
        Task<BannerDTO> Get(int id);
        Task<BannerDTO> Update(BannerDTO modelDto);
        Task<List<BannerVM>> GetCategoriesWithAll();
        Task<int> Remove(int id);
        Task<List<BannerVM>> GetCategoriesWithAllTest(int id);
        Task<BannerDTO> EditMenuContent(BannerDTO modelDTO);
        Task<int> UpdateMenus(List<MenuHeadingListPriorityDto> lstMenuPriourty);

        Task<List<BannerVMModal>> GetAllBanner();             //------------ Karn
    }
}