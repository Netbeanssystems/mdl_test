using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Infrastructure.Repositories
{
    public class BidderGeneralDocRepository : Repository<BidderGeneralDoc>, IBidderGeneralDocRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public BidderGeneralDocRepository(AppDbContext context) : base(context)
        {
        }

        public new Task<List<BidderGeneralDoc>> Get()
        {
            return DbContext.BidderGeneralDoc.ToListAsync();
        }

        public new Task<BidderGeneralDoc> Get(int id)
        {
            return DbContext.BidderGeneralDoc.FirstOrDefaultAsync(x => x.Id == id);
        }

        public new Task<BidderGeneralDoc> Get(string id)
        {
            if (int.TryParse(id, out int intId))
            {
                return Get(intId);
            }
            return Task.FromResult<BidderGeneralDoc>(null);
        }

        public new Task<List<BidderGeneralDoc>> GetActive()
        {
            return DbContext.BidderGeneralDoc.Where(x => x.IsActive).ToListAsync();
        }
        //public Task<List<BidderGeneralDoc>> GetWithAll()
        //{
        //    return DbContext.BidderGeneralDoc
        //        .Where(x =>
        //            x.IsActive
        //           )
        //        .OrderBy(c => c.Id)
        //        .ToListAsync();
        //}
        //public Task<BidderGeneralDoc> CheckDuplicate(BidderGeneralDoc model)
        //{
        //    return DbContext.AcademicYears
        //        .FirstOrDefaultAsync(x =>
        //            x.Year == model.Year
        //           );
        //}
    }
}
