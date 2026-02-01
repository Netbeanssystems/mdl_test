using Application.Dtos;
using Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.ServiceInterfaces
{
    public interface INewsService
    {
        //Common Methods
        Task<List<NewsVM>> Get();
        Task<List<NewsVM>> MostViewed();
        Task<List<NewsVM>> WhatsNew();
        Task<List<NewsVM>> MinistryIndustryUpdates();
        Task<NewsDTO> Get(int Id);
        Task<NewsDTO> Add(NewsDTO NewsDto);
        Task<NewsDTO> Update(NewsDTO NewsDto);
        Task<int> Remove(int id);
        Task<NewsVM> Get(string Heading);
        Task<List<NewsVM>> GetCategoriesWithAll();
        Task<List<NewsVM>> GetCategoriesWithAllTest(int id);
        Task<NewsDTO> EditMenuContent(NewsDTO NewsDto);
        Task<int> UpdateMenus(List<NewsListPriorityDto> lstMenuPriourty);
        Task<List<NewsVM>> Getorderdesc();

        Task<List<NewsVMModal>> GetAllNewsList();                        //-------- Karn 
    }
}