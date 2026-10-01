using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
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

        public async Task<List<URLsTiming>> GetClosed()
        {
            var currentTime = DateTime.Now;
            return await DbContext.URLsTiming
                .Where(t => t.IsActive && (t.ToTime < currentTime) && t.IsShow == false)
                .ToListAsync();
        }


        public async Task<List<URLsTiming>> GetByMultipleIds(List<int> ids)
        {
            return await DbContext.URLsTiming
                .Where(t => ids.Contains(t.Id))
                .ToListAsync();
        }

        public async Task Update(URLsTiming entity)
        {
            DbContext.Entry(entity).State = EntityState.Modified;
            await Task.CompletedTask;
        }
    }
}
