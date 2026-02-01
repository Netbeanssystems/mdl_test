using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Infrastructure.Repositories
{
    public class RoleMenusRepository : Repository<RoleMenus>, IRoleMenusRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public RoleMenusRepository(AppDbContext context) : base(context)
        {
        }
        public Task<List<RoleMenus>> GetAllByRole(string role)
        {
            return DbContext.RoleMenus
                .Include(x => x.Menu)
                .Where(x => x.RoleName == role)
                .ToListAsync();
        }
        public Task<RoleMenus> GetByRoleMenu(string roleName, int menuId)
        {
            return DbContext.RoleMenus.FirstOrDefaultAsync(x => x.RoleName == roleName && x.MenuId == menuId);
        }
    }
}
