using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class BannerRepository : Repository<Banners>, IBannerRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public BannerRepository(AppDbContext context) : base(context)
        {
        }
        public Task<List<Banners>> GetCategoriesWithAll()
        {
            //return DbContext.Banners.Where(c => c.Show && c.ParentId == null)
            //    .OrderBy(c => c.Priority).ToListAsync();

            return DbContext.Banners.Where(c => c.Show)
                .OrderBy(c => c.Priority).ToListAsync();
        }

        public Task<List<Banners>> GetCategoriesWithAllTest(int id)
        {
            return DbContext.Banners.Where(c => c.Show && c.ParentId == id)
                .OrderBy(c => c.Priority).ToListAsync();
        }
        public void UpdateMenuPriority(List<MenuHeadingListPriority> lstMenuPriourty)
        {
            foreach (MenuHeadingListPriority item in lstMenuPriourty)
            {
                var ExisitingEntity = DbContext.Banners.First(x => x.Id == item.Id);
                ExisitingEntity.Priority = item.Priority;
                DbContext.Banners.Update(ExisitingEntity);
            }
        }

        public Task<Banners> GetBanner(string Menu)
        {
            return DbContext.Banners.FirstOrDefaultAsync(a => a.EnglishHeadingName.ToLower() == Menu);
        }

        //-----------------------------------New Karn Code 8 Dec 2023---------------
        public async Task<List<BannersData>> GetAllBanner()
        //public async Task<List<Banners>> GetAllBanner()
        {


            //return await DbContext.Banners.ToListAsync();

            return await DbContext.Banners.Select(x =>
            new BannersData
            {
                ParentId = x.ParentId,
                Show = x.Show,
                Linkopen = x.Linkopen,
                EnglishLink = x.EnglishLink,
                Title = x.Title,
                EnglishHeadingName = x.EnglishHeadingName,
                EnglishBanner = x.EnglishBanner,
                Alt = x.Alt,

            }).ToListAsync();
        }

    }
}