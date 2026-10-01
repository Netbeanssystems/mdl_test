using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryInterfaces
{
    public interface IBannerRepository : IRepository<Banners>
    {
        Task<List<Banners>> GetCategoriesWithAll();
        Task<List<Banners>> GetCategoriesWithAllTest(int id);
        void UpdateMenuPriority(List<MenuHeadingListPriority> lstMenuPriourty);
        Task<Banners> GetBanner(string Menu);

        Task<List<BannersData>> GetAllBanner();             //------ Karn Added
    }
}
