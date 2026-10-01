using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;

namespace Infrastructure.Repositories
{
    public class GeneraluploadURLRepository : Repository<GeneraluploadURL>, IGeneraluploadURLRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public GeneraluploadURLRepository(AppDbContext context) : base(context)
        {
        }
    }
}
