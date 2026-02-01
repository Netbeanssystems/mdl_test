using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;

namespace Infrastructure.Repositories
{
    public class URLsTimingRepository : Repository<URLsTiming>, IURLsTimingRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public URLsTimingRepository(AppDbContext context) : base(context)
        {
        }
    }
}
