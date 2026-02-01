using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;

namespace Infrastructure.Repositories
{
    public class VigilanceRepository : Repository<VigilanceForm>, IVigilanceRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public VigilanceRepository(AppDbContext context) : base(context)
        {

        }
    }
}
