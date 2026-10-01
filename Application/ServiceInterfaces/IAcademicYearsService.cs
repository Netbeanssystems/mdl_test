using Application.Dtos;
using Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Application.ServiceInterfaces
{
    public interface IAcademicYearsService
    {
        Task<List<AcademicYearsVM>> Get();
        Task<AcademicYearsDTO> Get(int id);
        Task<bool> CheckDuplicate(AcademicYearsDTO argModelDto);
        Task<AcademicYearsDTO> Create(AcademicYearsDTO argModelDto);
        Task<AcademicYearsDTO> Update(AcademicYearsDTO argModelDto);
        Task<int> Delete(int id);
        //Custom Methods
        Task<List<AcademicYearsVM>> GetWithAll();
        Task<List<DropdownVM>> GetDropdown();
    }
}
