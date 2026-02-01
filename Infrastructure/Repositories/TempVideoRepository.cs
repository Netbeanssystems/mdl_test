using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class TempVideoRepository : Repository<TempVideo>, ITempVideoRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public TempVideoRepository(AppDbContext context) : base(context)
        {

        }

        public void CreateAudit(TempVideo model)
        {
            DbContext.TempVideo.Add(model);
        }
        public Task<List<TempVideo>> GetByAction(string status)
        {
            return DbContext.TempVideo.Where(c => c.Status == status).OrderBy(c => c.Id).ToListAsync();
        }
    }
}
