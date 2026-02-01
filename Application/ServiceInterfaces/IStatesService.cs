using Application.Dtos;
using Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Application.ServiceInterfaces
{
    public interface IStatesService
    {
        //Common Methods
        Task<List<StatesVM>> Get();
        Task<StatesDTO> Get(int id);
        Task<StatesDTO> Create(StatesDTO entity);
        Task<StatesDTO> Update(StatesDTO entity);
        Task<int> Delete(int id);
        Task<List<StatesDTO>> CreateRange(List<StatesDTO> entities);
        Task<List<StatesDTO>> Upsert(List<StatesDTO> entities);
        Task<int> DeleteRange(List<StatesDTO> entities);
        //Custom Methods
        Task<List<DropdownVM>> GetDropdown();
        Task<List<DropdownVM>> GetDropdownByCountry(int countryid);
    }
}
