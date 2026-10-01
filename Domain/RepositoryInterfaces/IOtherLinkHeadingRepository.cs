using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryInterfaces
{
    public interface IOtherLinkHeadingRepository : IRepository<OtherLinkHeading>
    {
        Task<OtherLinkHeading> GetByMenuHeading(string Menu);
        Task<List<OtherLinkHeading>> GetCategoriesWithAll();
        Task<List<OtherLinkHeading>> GetCategoriesWithAllTest(int id);
        void UpdateMenuPriority(List<OtherLinkListPriority> lstMenuPriourty);
        Task<List<OtherLinkHeading>> GetStockData(string Heading);
        Task<List<OtherLinkHeading>> GetStockDataHindi(string Heading);
        Task<List<OtherLinkHeading>> GetStockExchange();
        Task<List<OtherLinkHeading>> GetcorporateExchange();
        Task<List<OtherLinkHeading>> GetCommitment();
        Task<List<OtherLinkHeading>> GetServiceCOCOs();
        Task<List<OtherLinkHeading>> GetAnalystInstitutionalInvestors();
        Task<List<OtherLinkHeading>> GetHeading(string Type);
        Task<List<OtherLinkHeading>> GetHeadingForshareholding(string Type);
        Task<List<OtherLinkHeading>> GetSearchfromotherlinks(string q);
        Task<OtherLinkHeading> GetcmsforCorporateGovernance();
        Task<OtherLinkHeading> Getlastrecord();
        Task<OtherLinkHeading> Getlastrecordshareprice();
        Task<OtherLinkHeading> Getlastrecordanalystsinstitutional();
        Task<OtherLinkHeading> Getlastrecordstockexchange();
    }
}
