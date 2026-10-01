using Application.Dtos;
using Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.ServiceInterfaces
{
    public interface ITempOtherLinkHeadingService
    {
        //Common Methods
        Task<List<TempOtherLinkHeadingVM>> Get();
        Task<TempOtherLinkHeadingDTO> Get(int Id);
        Task<TempOtherLinkHeadingDTO> Add(TempOtherLinkHeadingDTO modelDto);
        Task<TempOtherLinkHeadingDTO> Update(TempOtherLinkHeadingDTO modelDto);
        Task<int> Remove(int id);
        //Custom Methods
        Task<TempOtherLinkHeadingDTO> CreateAudit(TempOtherLinkHeadingDTO modelDto);
        Task<List<TempOtherLinkHeadingVM>> GetByAction(string status);
    }
}
