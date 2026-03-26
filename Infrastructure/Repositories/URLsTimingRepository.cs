using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class URLsTimingRepository : Repository<URLsTiming>, IURLsTimingRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public URLsTimingRepository(AppDbContext context) : base(context)
        {
        }

        public new async Task<List<URLsTiming>> GetActive()
        {
            return await DbContext.URLsTiming
                .Where(u => u.IsActive)
                .OrderByDescending(u => u.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<URLsTiming>> GetWithDocuments()
        {
            return await DbContext.URLsTiming
                .Include(u => u.Documents)
                .OrderByDescending(u => u.CreatedDate)
                .ToListAsync();
        }
    }
}
