using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;

namespace Infrastructure.Repositories
{
    public class BidderTenderUploadDocumentsRepository : Repository<BidderTenderUploadDocuments>, IBidderTenderUploadDocumentsRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public BidderTenderUploadDocumentsRepository(AppDbContext context) : base(context)
        {
        }
    }
}
