using Application.Dtos;
using Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Application.ServiceInterfaces
{
    public interface IDistrictsService
    {
        //Common Methods
        Task<List<DistrictsVM>> Get();
        Task<DistrictsDTO> Get(int Id);
        Task<DistrictsDTO> Create(DistrictsDTO agrModelDto);
        Task<DistrictsDTO> Update(DistrictsDTO argModelDto);
        Task<int> Delete(int id);
        Task<List<DropdownVM>> GetDropdown();
        Task<List<DropdownVM>> GetDropdownByState(int stateid);
        Task<string> GetDistrictCode(int id);
    }
}