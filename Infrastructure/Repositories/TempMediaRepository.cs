using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class TempMediaRepository : Repository<TempMedia>, ITempMediaRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public TempMediaRepository(AppDbContext context) : base(context)
        {

        }
        public Task<List<TempMedia>> GetByAction(string status)
        {
            return DbContext.TempMedia.Where(c => c.Status == status).OrderBy(c => c.Id).ToListAsync();
        }
        public void CreateAudit(TempMedia model)
        {
            DbContext.TempMedia.Add(model);
        }
    }
}
