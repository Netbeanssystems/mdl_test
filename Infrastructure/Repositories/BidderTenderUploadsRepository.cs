using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class BidderTenderUploadsRepository : Repository<BidderTenderUploads>, IBidderTenderUploadsRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public BidderTenderUploadsRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<BidderTenderCorrigendumUploads> CheckTender(BidderTenderUploads model)
        {
            var tender = await DbContext.BidderTenderUploads
            .Where(c => c.ProjectId == model.ProjectId
                     && c.YardId == model.YardId
                     && c.TenderNo == model.TenderNo)
            .Select(c => new BidderTenderCorrigendumUploads
            {
                Id = c.Id,
                ProjectId = c.ProjectId,
                YardId = c.YardId,
                TenderNo = c.TenderNo,
                TenderDescription = c.TenderDescription,
                ForeignBidderId = c.ForeignBidderId,
                TenderOpeningDate = c.TenderOpeningDate,
                TenderClosingDate = c.TenderClosingDate,
                TenderDoc = c.TenderDoc,
                TenderCorrigendums = DbContext.BidderTenderCorrigendum
                    .Where(b => b.TenderId == c.Id.ToString())
                    .ToList()
            }).FirstOrDefaultAsync();

            return tender;
        }
    }
}