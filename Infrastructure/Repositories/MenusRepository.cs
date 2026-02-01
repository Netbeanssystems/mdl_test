using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Infrastructure.Repositories
{
    public class MenusRepository : Repository<Menus>, IMenusRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public MenusRepository(AppDbContext context) : base(context)
        {
        }
        public Task<List<Menus>> GetWithAll()
        {
            return DbContext.Menus
                .Where(x =>
                    x.IsActive &&
                    x.ParentId == null)
                .OrderBy(c => c.Id)
                .ToListAsync();
        }
    }
}
