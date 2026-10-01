using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;

namespace Infrastructure.Repositories
{
    public class FeedbackRepository : Repository<FeedbackForm>, IFeedbackRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public FeedbackRepository(AppDbContext context) : base(context)
        {

        }
    }
}
