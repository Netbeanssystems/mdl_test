using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class MediaRepository : Repository<Media>, IMediaRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public MediaRepository(AppDbContext context) : base(context)
        {
        }

        public Task<List<Media>> Getorder()
        {
            var result = DbContext.Media.ToListAsync();
            return (result);
        }

        public Task<List<Media>> Getorderdesc()
        {
            var result = DbContext.Media.OrderByDescending(a => a.Id).Distinct().ToListAsync();
            return (result);
        }

        public Task<List<Media>> Getnews()
        {
            var result = DbContext.Media.OrderByDescending(a => a.Id).Distinct().ToListAsync();
            return (result);
        }

        public Task<Media> GetMenus(int Id)
        {
            return DbContext.Media.FirstOrDefaultAsync(a => a.Id == Id);
        }

        public Task<Media> Getlastrecord()
        {
            return DbContext.Media.OrderByDescending(a => a.Id).FirstOrDefaultAsync();
        }

        //public Task<List<int>> GetNewsYear()
        //{
        //    var years = DbContext.Media.OrderByDescending(x => x.Id).Select(x => x.NewsDate.Value.Year).Distinct();
        //    var test = years.ToList();
        //    return years.ToListAsync();
        //}

        public Task<List<Media>> GetNewsData(int Heading)
        {
            return DbContext.Media.ToListAsync();
        }



        //-----------------------------------New Karn Code 11 Dec 2023--------------- 
        //public async Task<List<Media>> GetMedia()
        public async Task<List<MediaNewData>> GetMedia()
        {

            return await DbContext.Media.Select(x =>
            new MediaNewData
            {
                Show = x.Show,
                UploadedDate = x.UploadedDate,
                EnglishHeading = x.EnglishHeading,
                EnglishImage = x.EnglishImage,
                EnglishContent = x.EnglishContent,



            }).ToListAsync();
        }


    }
}
