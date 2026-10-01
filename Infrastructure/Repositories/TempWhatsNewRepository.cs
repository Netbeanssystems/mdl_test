using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class TempWhatsNewRepository : Repository<TempWhatsNew>, ITempWhatsNewRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public TempWhatsNewRepository(AppDbContext context) : base(context)
        {

        }

        public void CreateAudit(TempWhatsNew model)
        {
            DbContext.TempWhatsNew.Add(model);
        }
        public Task<List<TempWhatsNew>> GetByAction(string status)
        {
            return DbContext.TempWhatsNew.Where(c => c.Status == status).OrderBy(c => c.Id).ToListAsync();
        }
    }
}
