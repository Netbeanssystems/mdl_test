using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class TempBannerRepository : Repository<TempBanners>, ITempBannerRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public TempBannerRepository(AppDbContext context) : base(context)
        {

        }
        public void CreateAudit(TempBanners model)
        {
            DbContext.TempBanners.Add(model);
        }
        public Task<List<TempBanners>> GetByAction(string status)
        {
            return DbContext.TempBanners.Where(c => c.Status == status).OrderBy(c => c.Id).ToListAsync();
        }
    }
}
