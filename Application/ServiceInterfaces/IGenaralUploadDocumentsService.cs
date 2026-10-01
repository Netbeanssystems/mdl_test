using Application.Dtos;
using Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Application.ServiceInterfaces
{
    public interface IGenaralUploadDocumentsService
    {
        //Common Methods  
        Task<List<GenaralUploadDocumentsVM>> Get();
        Task<GenaralUploadDocumentsDTO> Get(int id);
        Task<List<GenaralUploadDocumentsVM>> Getbycreatedby(string createdby);
        Task<GenaralUploadDocumentsDTO> Create(GenaralUploadDocumentsDTO entity);
        Task<GeneraluploadURLDTO> CreateURLsTiming(GeneraluploadURLDTO entity);
        Task<List<GeneraluploadURLDTO>> GetURLsTiming();
        Task<GenaralUploadDocumentsDTO> Update(GenaralUploadDocumentsDTO entity);
        Task<int> Delete(int id);
        Task<List<GenaralUploadDocumentsDTO>> CreateRange(List<GenaralUploadDocumentsDTO> entities);
        Task<List<GenaralUploadDocumentsDTO>> Upsert(List<GenaralUploadDocumentsDTO> entities);
        Task<int> DeleteRange(List<GenaralUploadDocumentsDTO> entities);
        //Custom Methods
        Task<List<DropdownVM>> GetDropdown();
    }
}
