using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
namespace Infrastructure.Repositories
{
    public class CountriesRepository : Repository<Countries>, ICountriesRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public CountriesRepository(AppDbContext context) : base(context)
        {
        }
    }
}
