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
