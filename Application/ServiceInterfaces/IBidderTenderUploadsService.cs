using Application.Dtos;
using Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.ServiceInterfaces
{
    public interface IBidderTenderUploadsService
    {
        //Common Methods
        Task<List<BidderTenderUploadsVM>> Get();
        Task<BidderTenderUploadsDTO> Add(BidderTenderUploadsDTO modelDto);
        Task<BidderTenderUploadsDTO> Get(int id);
        Task<BidderTenderUploadsDTO> Update(BidderTenderUploadsDTO modelDto);
        Task<int> Remove(int id);
        Task<BidderTenderUploadsVM> CheckTender(BidderTenderUploadsDTO modelDto);
        Task<BidderTenderUploadsVM> GetByTenderNo(string tenderNo);
    }
}