using Application.Dtos;
using Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.ServiceInterfaces
{
    public interface ITempWhatsNewService
    {
        //Common Methods
        Task<List<TempWhatsNewVM>> Get();
        Task<TempWhatsNewDTO> Get(int Id);
        Task<TempWhatsNewDTO> Add(TempWhatsNewDTO tempnewsDto);
        Task<TempWhatsNewDTO> Update(TempWhatsNewDTO tempnewsDto);
        Task<int> Remove(int id);
        //Custom Methods
        Task<TempWhatsNewDTO> CreateAudit(TempWhatsNewDTO tempnewsDto);
        Task<List<TempWhatsNewVM>> GetByAction(string status);
    }
}