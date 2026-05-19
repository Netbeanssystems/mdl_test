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

             var  test =  await query.OrderByDescending(d => d.CreatedDate).ToListAsync();
            return test;
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

        public async Task<List<URLsTiming>> GetDocumentsIsShow()
        {
            return await DbContext.URLsTiming
                .Include(x => x.Documents)
                .Where(x => x.IsShow == true)
                .ToListAsync();
        }
        
            public async Task<List<Documents>> GetDocumentsList(int? urlsTimingId, DateTime? fromDate, DateTime? toDate)
          {
            var query = DbContext.Documents
                .Include(d => d.URLsTiming)
                .Where(d => d.URLsTiming.IsShow == true)
                .AsQueryable();

            // 1. Filter by the specific Window tracking ID if chosen
            if (urlsTimingId.HasValue && urlsTimingId.Value > 0)
            {
                query = query.Where(d => d.URLsTimingId == urlsTimingId.Value);
            }

            // 2. Filter from the START of the fromDate (00:00:00)
            if (fromDate.HasValue)
            {
                // Use Date property to ensure time is stripped back to midnight
                var startOfDays = fromDate.Value.Date;
                query = query.Where(d => d.CreatedDate >= startOfDays);
            }

            // 3. FIXED: Filter to the END of the toDate day (23:59:59)
            if (toDate.HasValue)
            {
                // This takes the date and moves the constraint to 1 tick before midnight of the next day
                var endOfDays = toDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(d => d.CreatedDate <= endOfDays);
            }

            return await query.OrderByDescending(d => d.CreatedDate).ToListAsync();
        }


        public async Task<bool> SaveDownloadLog(DocumentDownloadLog logs)
        {
            if (logs == null) return false;

            try
            {

            var log =  await DbContext.documentDownloadLogs.AddAsync(logs);
            var rowAffected = await DbContext.SaveChangesAsync();
            return rowAffected > 0  ;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}
