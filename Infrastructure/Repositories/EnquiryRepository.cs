using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;

namespace Infrastructure.Repositories
{
    public class EnquiryRepository : Repository<EnquiryForm>, IEnquiryRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public EnquiryRepository(AppDbContext context) : base(context)
        {

        }
    }
}
