using Application.Dtos;
using Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Application.ServiceInterfaces
{
    public interface IBidderGeneralDocService
    {
        Task<List<BidderGeneralDocVM>> Get();
        Task<BidderGeneralDocDTO> Get(int id);
        //Task<bool> CheckDuplicate(BidderGeneralDocDTO argModelDto);
        Task<BidderGeneralDocDTO> Create(BidderGeneralDocDTO argModelDto);
        Task<BidderGeneralDocDTO> Update(BidderGeneralDocDTO argModelDto);
        Task<int> Delete(int id);
        //Custom Methods
        //Task<List<BidderGeneralDocVM>> GetWithAll();
        //Task<List<DropdownVM>> GetDropdown();
    }
}
