using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Infrastructure.Repositories
{
    public class StatesRepository : Repository<States>, IStatesRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public StatesRepository(AppDbContext context) : base(context)
        {
        }
        public Task<List<States>> GetStatesByCountry(int countryid)
        {
            return DbContext.States
                .Where(x => x.CountryId == countryid && x.IsActive).ToListAsync();
        }
    }
}
