using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryInterfaces
{
    public interface IWhatsNewRepository : IRepository<WhatsNew>
    {
        Task<List<WhatsNew>> GetCategoriesWithAll();
        Task<List<WhatsNew>> GetCategoriesWithAllTest(int id);
        void UpdateMenuPriority(List<WhatsNewListPriorityModel> lstMenuPriourty);
        Task<WhatsNew> GetByMenuHeading(string Menu);
        Task<List<WhatsNew>> Getorderdesc();
        Task<List<WhatsNew>> MostViewed();
        Task<List<WhatsNew>> WhatsNew();
        Task<List<WhatsNew>> MinistryIndustryUpdates();

        Task<List<WhatsNewData>> GetAllWhatsNew();                        //-------- Karn
    }
}
