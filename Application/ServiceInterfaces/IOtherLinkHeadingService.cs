using Application.Dtos;
using Application.ViewModels;
using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.ServiceInterfaces
{
    public interface IOtherLinkHeadingService
    {
        Task<List<OtherLinkHeadingDTO>> Get();
        Task<OtherLinkHeadingDTO> Add(OtherLinkHeadingDTO productHeadingDto);
        Task<OtherLinkHeadingDTO> Get(int id);
        Task<OtherLinkHeadingVM> Get(string Heading);
        Task<OtherLinkHeadingDTO> Update(OtherLinkHeadingDTO productHeadingDto);
        Task<List<OtherLinkHeadingVM>> GetCategoriesWithAll();
        Task<int> Remove(int id);
        Task<List<OtherLinkHeadingVM>> GetCategoriesWithAllTest(int id);
        Task<OtherLinkHeadingDTO> EditMenuContent(OtherLinkHeadingDTO productHeadingDto);
        Task<int> UpdateMenus(List<OtherLinkListPriorityDTO> lstMenuPriourty);
        Task<List<OtherLinkHeadingDTO>> GetStockData(string Heading);
        Task<List<OtherLinkHeadingDTO>> GetStockDataHindi(string Heading);
        Task<List<OtherLinkHeadingDTO>> GetStockExchange();
        Task<List<OtherLinkHeadingDTO>> GetcorporateExchange();
        Task<List<OtherLinkHeadingDTO>> GetServiceCOCOs();
        Task<List<OtherLinkHeadingDTO>> GetCommitment();
        Task<List<OtherLinkHeadingDTO>> GetAnalystInstitutionalInvestors();
        Task<List<OtherLinkHeadingDTO>> GetHeading(string Type);
        Task<OtherLinkHeadingDTO> Getlastrecord();
        Task<OtherLinkHeadingDTO> Getlastrecordshareprice();
        Task<OtherLinkHeadingDTO> Getlastrecordanalystsinstitutional();
        Task<OtherLinkHeadingDTO> Getlastrecordstockexchange();
        Task<List<OtherLinkHeadingDTO>> GetHeadingForshareholding(string Type);
        Task<List<OtherLinkHeading>> GetSearchfromotherlinks(string q);
        Task<OtherLinkHeadingVM> GetcmsforCorporateGovernance();
    }
}
