using Application.Dtos;
using Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.ServiceInterfaces
{
    public interface IBidderTenderDocumentsService
    {
        //Common Methods
        Task<List<BidderTenderDocumentsVM>> Get();
        Task<BidderTenderDocumentsDTO> Add(BidderTenderDocumentsDTO modelDto);
        Task<BidderTenderDocumentsDTO> Get(int id);
        Task<BidderTenderDocumentsDTO> Update(BidderTenderDocumentsDTO modelDto);
        Task<int> Remove(int id);
    }
}