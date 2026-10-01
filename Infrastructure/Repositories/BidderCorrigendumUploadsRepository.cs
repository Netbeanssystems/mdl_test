using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;

namespace Infrastructure.Repositories
{
    public class BidderCorrigendumUploadsRepository : Repository<BidderTenderCorrigendum>, IBidderCorrigendumUploadsRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public BidderCorrigendumUploadsRepository(AppDbContext context) : base(context)
        {
        }
    }
}