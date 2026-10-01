using Application.Dtos;
using Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.ServiceInterfaces
{
    public interface IWhatsNewService
    {
        //Common Methods
        Task<List<WhatsNewVM>> Get();
        Task<List<WhatsNewVM>> MostViewed();
        Task<List<WhatsNewVM>> WhatsNew();
        Task<List<WhatsNewVM>> MinistryIndustryUpdates();
        Task<WhatsNewDTO> Get(int Id);
        Task<WhatsNewDTO> Add(WhatsNewDTO NewsDto);
        Task<WhatsNewDTO> Update(WhatsNewDTO NewsDto);
        Task<int> Remove(int id);
        Task<WhatsNewVM> Get(string Heading);
        Task<List<WhatsNewVM>> GetCategoriesWithAll();
        Task<List<WhatsNewVM>> GetCategoriesWithAllTest(int id);
        Task<WhatsNewDTO> EditMenuContent(WhatsNewDTO NewsDto);
        Task<int> UpdateMenus(List<NewsListPriorityDto> lstMenuPriourty);
        Task<List<WhatsNewVM>> Getorderdesc();



        Task<List<WhatsNewModal>> GetAllWhats();                        //-------- Karn
    }
}