using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class OtherLinkHeadingRepository : Repository<OtherLinkHeading>, IOtherLinkHeadingRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public OtherLinkHeadingRepository(AppDbContext context) : base(context)
        {
        }
        public Task<List<OtherLinkHeading>> GetCategoriesWithAll()
        {
            return DbContext.OtherLinkHeading.Where(c => c.Show && c.ParentId == null)
                .OrderBy(c => c.Priority).ToListAsync();
        }
        //..............................Test..................
        public Task<List<OtherLinkHeading>> GetCategoriesWithAllTest(int id)
        {
            return DbContext.OtherLinkHeading.Where(c => c.Show && c.ParentId == id)
                .OrderBy(c => c.Priority).ToListAsync();
        }
        public void UpdateMenuPriority(List<OtherLinkListPriority> lstMenuPriourty)
        {
            foreach (OtherLinkListPriority item in lstMenuPriourty)
            {
                var ExisitingEntity = DbContext.OtherLinkHeading.First(x => x.Id == item.Id);
                ExisitingEntity.Priority = item.Priority;
                DbContext.OtherLinkHeading.Update(ExisitingEntity);
            }
        }

        public Task<List<OtherLinkHeading>> GetAnalystInstitutionalInvestors()
        {
            return DbContext.OtherLinkHeading.Where(x => x.ParentId == 82).OrderBy(c => c.Priority).ToListAsync();

        }

        public Task<OtherLinkHeading> GetByMenuHeading(string Menu)
        {
            return DbContext.OtherLinkHeading.FirstOrDefaultAsync(a => a.EnglishHeadingName.ToLower() == Menu);
        }

        public Task<List<OtherLinkHeading>> GetStockExchange()
        {
            return DbContext.OtherLinkHeading.Where(x => x.ParentId == 49).OrderBy(c => c.Priority).ToListAsync();

        }
        public Task<List<OtherLinkHeading>> GetStockData(string Heading)
        {

            return DbContext.OtherLinkHeading.Where(x => x.EnglishHeadingName == Heading).OrderBy(c => c.Priority).ToListAsync();
        }
        public Task<List<OtherLinkHeading>> GetStockDataHindi(string Heading)
        {

            return DbContext.OtherLinkHeading.Where(x => x.HindiHeadingName == Heading).OrderBy(c => c.Priority).ToListAsync();
        }

        public Task<List<OtherLinkHeading>> GetHeading(string Type)
        {
            var Result = DbContext.OtherLinkHeading.Where(x => x.ParentId == 62).OrderBy(c => c.Priority).ToListAsync();
            return Result;
        }

        public Task<List<OtherLinkHeading>> GetHeadingForshareholding(string Type)
        {
            var Result = DbContext.OtherLinkHeading.Where(x => x.ParentId == 124).OrderBy(c => c.Priority).ToListAsync();
            return Result;
        }

        public Task<List<OtherLinkHeading>> GetcorporateExchange()
        {
            return DbContext.OtherLinkHeading.Where(x => x.ParentId == 75).OrderBy(c => c.Priority).ToListAsync();

        }

        public Task<List<OtherLinkHeading>> GetCommitment()
        {
            return DbContext.OtherLinkHeading.Where(x => x.ParentId == 77).OrderBy(c => c.Priority).ToListAsync();

        }

        public Task<List<OtherLinkHeading>> GetServiceCOCOs()
        {
            return DbContext.OtherLinkHeading.Where(x => x.ParentId == 147).OrderBy(c => c.Priority).ToListAsync();
        }

        public Task<List<OtherLinkHeading>> GetSearchfromotherlinks(string q)
        {
            return DbContext.OtherLinkHeading.Where(a => a.EnglishHeadingName.Contains(q)).OrderByDescending(x => x.Id).Distinct().ToListAsync();
        }

        public Task<OtherLinkHeading> GetcmsforCorporateGovernance()
        {
            return DbContext.OtherLinkHeading.Select(a => new OtherLinkHeading { EnglishHeadingName = a.EnglishHeadingName, HindiHeadingName = a.HindiHeadingName, EnglishContentDesc = a.EnglishContentDesc, HindiContentDesc = a.HindiContentDesc, Id = a.Id, Priority = a.Priority, Show = a.Show, Title = a.Title, Description = a.Description, Keyword = a.Keyword, UpdateDate = a.UpdateDate }).FirstOrDefaultAsync(x => x.EnglishHeadingName == "Corporate Governance");
        }
        public Task<OtherLinkHeading> Getlastrecord()
        {
            return DbContext.OtherLinkHeading.Where(x => x.ParentId == 124).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
        }
        public Task<OtherLinkHeading> Getlastrecordshareprice()
        {
            return DbContext.OtherLinkHeading.Where(x => x.ParentId == 62).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
        }

        public Task<OtherLinkHeading> Getlastrecordanalystsinstitutional()
        {
            return DbContext.OtherLinkHeading.Where(x => x.ParentId == 82).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
        }

        public Task<OtherLinkHeading> Getlastrecordstockexchange()
        {
            return DbContext.OtherLinkHeading.Where(x => x.ParentId == 49).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
        }

    }
}
