using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;

namespace Infrastructure.Repositories
{
    internal class GrievanceRepository : Repository<GrievanceForm>, IGrievanceRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public GrievanceRepository(AppDbContext context) : base(context)
        {

        }
    }
}
