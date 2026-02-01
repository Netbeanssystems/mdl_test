using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Infrastructure.Repositories
{
    public class DistrictsRepository : Repository<Districts>, IDistrictsRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public DistrictsRepository(AppDbContext context) : base(context)
        {
        }
        public Task<List<Districts>> GetDistrictsByState(int stateid)
        {
            return DbContext.Districts
                .Where(x => x.StateId == stateid && x.IsActive).ToListAsync();
        }
    }
}