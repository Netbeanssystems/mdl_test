using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class TempMenuHeadingsRepository : Repository<TempMenuHeadings>, ITempMenuHeadingsRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public TempMenuHeadingsRepository(AppDbContext context) : base(context)
        {

        }
        public void CreateAudit(TempMenuHeadings model)
        {
            DbContext.TempMenuHeadings.Add(model);
        }
        public Task<List<TempMenuHeadings>> GetByAction(string status)
        {
            //return DbContext.TempMenuHeadings.Where(c => c.Status == status).OrderBy(c => c.Id).ToListAsync();
            return DbContext.TempMenuHeadings.Where(c => c.Status == status && c.ForReview == 2) .OrderBy(c => c.Id).ToListAsync();
        }
        public Task<List<TempMenuHeadings>> GetByActionupdate(string status)
        {
            //return DbContext.TempMenuHeadings.Where(c => c.Status == status).OrderBy(c => c.Id).ToListAsync();
            return DbContext.TempMenuHeadings.Where(c => c.Status == status && c.ForReview == 1).OrderBy(c => c.Id).ToListAsync();
        }

    }
}