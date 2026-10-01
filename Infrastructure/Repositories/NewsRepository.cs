using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class NewsRepository : Repository<News>, INewsRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public NewsRepository(AppDbContext context) : base(context)
        {

        }

        public Task<List<News>> GetCategoriesWithAll()
        {
            return DbContext.News.Where(c => c.Show && c.ParentId == null)
                .OrderBy(c => c.Priority).ToListAsync();

            //return DbContext.News.Where(c => c.Show && c.ParentId!= null)
            //    .OrderBy(c => c.Priority).ToListAsync();
        }

        public Task<List<News>> Getorderdesc()
        {
            return DbContext.News.Where(c => c.Show)
                .OrderBy(c => c.Priority).ToListAsync();
        }
        public Task<List<News>> MostViewed()
        {
            return DbContext.News.Where(c => c.Show && c.ParentId == 78)
                .OrderBy(c => c.Priority).ToListAsync();
        }
        public Task<List<News>> WhatsNew()
        {
            return DbContext.News.Where(c => c.Show && c.ParentId == 77)
                .OrderBy(c => c.Priority).ToListAsync();
        }
        public Task<List<News>> MinistryIndustryUpdates()
        {
            return DbContext.News.Where(c => c.Show && c.ParentId == 79)
                .OrderBy(c => c.Priority).ToListAsync();
        }
        public Task<List<News>> GetCategoriesWithAllTest(int id)
        {
            return DbContext.News.Where(c => c.Show && c.ParentId == id)
                .OrderBy(c => c.Priority).ToListAsync();
        }
        public void UpdateMenuPriority(List<NewsListPriorityModel> lstMenuPriourty)
        {
            foreach (NewsListPriorityModel item in lstMenuPriourty)
            {
                var ExisitingEntity = DbContext.News.First(x => x.Id == item.Id);
                ExisitingEntity.Priority = item.Priority;
                DbContext.News.Update(ExisitingEntity);
            }
        }

        public Task<News> GetByMenuHeading(string Menu)
        {
            return DbContext.News.FirstOrDefaultAsync(a => a.EnglishHeadingName.ToLower() == Menu);
        }



        //-----------------------------------New Karn Code 11 Dec 2023---------------  
        //public async Task<List<News>> GetAllNewsList()
        public async Task<List<NewsData>> GetAllNewsList()
        {


            //return await DbContext.News.ToListAsync();

            return await DbContext.News.Select(x =>
            new NewsData
            {
                Show = x.Show,
                EnglishContentDesc = x.EnglishContentDesc,
                Priority = x.Priority,
                HindiContentDesc = x.HindiContentDesc,

            }).ToListAsync();
        }
    }
}