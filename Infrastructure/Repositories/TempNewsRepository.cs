using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class TempNewsRepository : Repository<TempNews>, ITempNewsRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public TempNewsRepository(AppDbContext context) : base(context)
        {

        }

        public void CreateAudit(TempNews model)
        {
            DbContext.TempNews.Add(model);
        }
        public Task<List<TempNews>> GetByAction(string status)
        {
            return DbContext.TempNews.Where(c => c.Status == status).OrderBy(c => c.Id).ToListAsync();
        }
    }
}
