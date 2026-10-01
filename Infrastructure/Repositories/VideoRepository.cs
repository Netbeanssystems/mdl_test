using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;

namespace Infrastructure.Repositories
{
    public class VideoRepository : Repository<Video>, IVideoRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public VideoRepository(AppDbContext context) : base(context)
        {

        }
    }
}
