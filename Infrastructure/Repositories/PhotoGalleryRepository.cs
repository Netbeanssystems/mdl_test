using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class PhotoGalleryRepository : Repository<PhotoGallery>, IPhotoGalleryRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public PhotoGalleryRepository(AppDbContext context) : base(context)
        {

        }

        public List<PhotoGalleryModel> GetEvents()
        {
            var ap = (from p in DbContext.PhotoGallery
                      join ev in DbContext.Events on p.EventId equals ev.Id into evname
                      from evnames in evname.DefaultIfEmpty()
                      select new PhotoGalleryModel
                      {
                          Id = p.Id,
                          EventId = p.EventId,
                          EventName = evnames.EventName + "(" + evnames.EventNameHindi + ")",
                          EnglishHeading = p.EnglishHeading,
                          HindiHeading = p.HindiHeading,
                          EnglishDescp = p.EnglishDescp,
                          HindiDescp = p.HindiDescp,
                          EnglishAttachment = p.EnglishAttachment,
                          HindiAttachment = p.HindiAttachment,
                          Active = p.Active,
                      }).ToList();
            return ap;
        }

        public Task<List<PhotoGallery>> GetbyPriorty()
        {
            var result = DbContext.PhotoGallery.OrderBy(x => x.Priority).Where(x => x.Priority != 2).ToListAsync();
            return (result);
        }
        public Task<List<PhotoGallery>> Getorder(string Key)
        {
            var result = DbContext.PhotoGallery.Where(x => x.EnglishHeading == Key).OrderBy(x => x.Priority).ToListAsync();
            return (result);
        }
        public Task<List<PhotoGallery>> GetorderHindi(string Key)
        {
            var result = DbContext.PhotoGallery.Where(x => x.HindiHeading == Key).OrderBy(x => x.Priority).ToListAsync();
            return (result);
        }

        public Task<List<PhotoGallery>> GetbyPriortyForAwardAccolades()
        {
            var result = DbContext.PhotoGallery.Where(x => x.Priority == 2).OrderByDescending(x => x.Id).ToListAsync();
            return (result);
        }
        public Task<List<PhotoGallery>> GetorderPriortyForAwardAccolades(string Key)
        {
            var result = DbContext.PhotoGallery.Where(x => x.EnglishHeading == Key).Where(x => x.Priority == 2).OrderByDescending(x => x.Id).ToListAsync();
            //  var result = DbContext.PhotoGallery.Where(x => x.EnglishHeading == Key && x.EnglishHeading == "Awards and Accolades").ToListAsync();
            return (result);
        }

        public Task<List<PhotoGallery>> GetorderPriortyForAwardAccoladeshindi(string Key)
        {
            var result = DbContext.PhotoGallery.Where(x => x.HindiHeading == Key).Where(x => x.Priority == 2).ToListAsync();
            // var result = DbContext.PhotoGallery.Where(x => x.HindiHeading == Key && x.HindiHeading == "पुरस्कार और सम्मान").ToListAsync();
            return (result);
        }

        public Task<List<PhotoGallery>> GetCategory(string Key)
        {
            var result = DbContext.PhotoGallery.Where(x => x.EnglishDescp == Key).ToListAsync();
            return (result);
        }

    }
}