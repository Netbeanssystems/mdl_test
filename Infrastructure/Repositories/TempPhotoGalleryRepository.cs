using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class TempPhotoGalleryRepository : Repository<TempPhotoGallery>, ITempPhotoGalleryRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public TempPhotoGalleryRepository(AppDbContext context) : base(context)
        {

        }

        public void CreateAudit(TempPhotoGallery model)
        {
            DbContext.TempPhotoGallery.Add(model);
        }
        public Task<List<TempPhotoGallery>> GetByAction(string status)
        {
            return DbContext.TempPhotoGallery.Where(c => c.Status == status).OrderBy(c => c.Id).ToListAsync();
        }

    }
}
