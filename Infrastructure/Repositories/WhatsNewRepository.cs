using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class WhatsNewRepository : Repository<WhatsNew>, IWhatsNewRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public WhatsNewRepository(AppDbContext context) : base(context)
        {

        }

        public Task<List<WhatsNew>> GetCategoriesWithAll()
        {
            return DbContext.WhatsNew.Where(c => c.Show && c.ParentId == null)
                .OrderBy(c => c.Priority).ToListAsync();
        }

        public Task<List<WhatsNew>> Getorderdesc()
        {
            return DbContext.WhatsNew.Where(c => c.Show)
                .OrderBy(c => c.Priority).ToListAsync();
        }
        public Task<List<WhatsNew>> MostViewed()
        {
            return DbContext.WhatsNew.Where(c => c.Show && c.ParentId == 78)
                .OrderBy(c => c.Priority).ToListAsync();
        }
        public Task<List<WhatsNew>> WhatsNew()
        {
            return DbContext.WhatsNew.Where(c => c.Show && c.ParentId == 77)
                .OrderBy(c => c.Priority).ToListAsync();
        }
        public Task<List<WhatsNew>> MinistryIndustryUpdates()
        {
            return DbContext.WhatsNew.Where(c => c.Show && c.ParentId == 79)
                .OrderBy(c => c.Priority).ToListAsync();
        }
        public Task<List<WhatsNew>> GetCategoriesWithAllTest(int id)
        {
            return DbContext.WhatsNew.Where(c => c.Show && c.ParentId == id)
                .OrderBy(c => c.Priority).ToListAsync();
        }
        public void UpdateMenuPriority(List<WhatsNewListPriorityModel> lstMenuPriourty)
        {
            foreach (WhatsNewListPriorityModel item in lstMenuPriourty)
            {
                var ExisitingEntity = DbContext.WhatsNew.First(x => x.Id == item.Id);
                ExisitingEntity.Priority = item.Priority;
                DbContext.WhatsNew.Update(ExisitingEntity);
            }
        }

        public Task<WhatsNew> GetByMenuHeading(string Menu)
        {
            return DbContext.WhatsNew.FirstOrDefaultAsync(a => a.EnglishHeadingName.ToLower() == Menu);
        }



        //-----------------------------------New Karn Code 11 Dec 2023--------------- 
        public async Task<List<WhatsNewData>> GetAllWhatsNew()
        {

            return await DbContext.WhatsNew.Select(x =>
            new WhatsNewData
            {
                Show = x.Show,
                Priority = x.Priority,
                EnglishAttachment = x.EnglishAttachment,
                PublishDate = x.PublishDate,
                EnglishContentDesc = x.EnglishContentDesc,
                EnglishPageLink = x.EnglishPageLink,
                HindiContentDesc = x.HindiContentDesc,


            }).ToListAsync();
        }
    }
}