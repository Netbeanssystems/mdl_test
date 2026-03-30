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
    public class DocumentsRepository : Repository<Documents>, IDocumentsRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public DocumentsRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<List<Documents>> Getbycreatedby(string createdby)
        {
            return await DbContext.Documents
                .Where(c => c.CreatedBy == createdby)
                .OrderBy(c => c.CreatedBy)
                .ToListAsync();
        }
        // added this method for getting Year and Description  for bank modules
        public async Task<List<Documents>> GetByYearAndDescription(int? year, int? urlsTimingId)
        {
            var query = DbContext.Documents
                .Include(d => d.URLsTiming)
                .AsQueryable();

            if (year.HasValue)
            {
                query = query.Where(d => d.CreatedDate.Year == year.Value);
            }

            if (urlsTimingId.HasValue)
            {
                query = query.Where(d => d.URLsTimingId == urlsTimingId.Value);
            }

            return await query.OrderByDescending(d => d.CreatedDate).ToListAsync();
        }

        public async Task<List<Documents>> GetByURLsTimingAndDateRange(int? urlsTimingId, DateTime? fromDate, DateTime? toDate)
        {
            var query = DbContext.Documents
                .Include(d => d.URLsTiming)
                .AsQueryable();

            if (urlsTimingId.HasValue)
            {
                query = query.Where(d => d.URLsTimingId == urlsTimingId.Value);
            }

            if (fromDate.HasValue && toDate.HasValue)
            {
                query = query.Where(d => d.URLsTiming != null && d.URLsTiming.FromTime >= fromDate.Value && d.URLsTiming.FromTime <= toDate.Value);
            }

            return await query.OrderByDescending(d => d.CreatedDate).ToListAsync();
        }

        public async Task<List<Documents>> GetGroupedByURLsTiming()
        {
            return await DbContext.Documents
                .Include(d => d.URLsTiming)
                .OrderByDescending(d => d.CreatedDate)
                .ToListAsync();
        }

        public new async Task<List<Documents>> GetActive()
        {
            return await DbContext.Documents
                .Where(d => d.IsActive)
                .Include(d => d.URLsTiming)
                .OrderByDescending(d => d.CreatedDate)
                .ToListAsync();
        }
    }
}
