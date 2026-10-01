using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryInterfaces
{
    public interface INewsRepository : IRepository<News>
    {
        Task<List<News>> GetCategoriesWithAll();
        Task<List<News>> GetCategoriesWithAllTest(int id);
        void UpdateMenuPriority(List<NewsListPriorityModel> lstMenuPriourty);
        Task<News> GetByMenuHeading(string Menu);
        Task<List<News>> Getorderdesc();
        Task<List<News>> MostViewed();
        Task<List<News>> WhatsNew();
        Task<List<News>> MinistryIndustryUpdates();


        Task<List<NewsData>> GetAllNewsList();                        //-------- Karn
    }
}
