using Application.Dtos;
using Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Application.ServiceInterfaces
{
    public interface IDocumentsService
    {
        //Common Methods  
        Task<List<DocumentsVM>> Get();
        Task<DocumentsDTO> Get(int id);
        Task<List<DocumentsVM>> Getbycreatedby(string createdby);
        Task<DocumentsDTO> Create(DocumentsDTO entity);
        Task<URLsTimingDTO> CreateURLsTiming(URLsTimingDTO entity);
        Task<List<URLsTimingVM>> GetURLsTiming();
        Task<DocumentsDTO> Update(DocumentsDTO entity);
        Task<int> Delete(int id);
        Task<List<DocumentsDTO>> CreateRange(List<DocumentsDTO> entities);
        Task<List<DocumentsDTO>> Upsert(List<DocumentsDTO> entities);
        Task<int> DeleteRange(List<DocumentsDTO> entities);
        //Custom Methods
        Task<List<DropdownVM>> GetDropdown();

    }
}
