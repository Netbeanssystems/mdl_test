using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;

namespace Infrastructure.Repositories
{
    public class ContactUsRepository : Repository<ContactUsForm>, IContactUsRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public ContactUsRepository(AppDbContext context) : base(context)
        {

        }
    }
}
