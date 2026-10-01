using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;

namespace Infrastructure.Repositories
{
    public class BidderTenderDocumentsRepository : Repository<BidderTenderDocuments>, IBidderTenderDocumentsRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public BidderTenderDocumentsRepository(AppDbContext context) : base(context)
        {
        }
    }
}