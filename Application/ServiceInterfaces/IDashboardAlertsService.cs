using Application.Dtos;
using Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Application.ServiceInterfaces
{
    public interface IDashboardAlertsService
    {
        Task<List<DashboardAlertsVM>> Get();
        Task<DashboardAlertsDTO> Get(int id);
        Task<bool> CheckDuplicate(DashboardAlertsDTO argModelDto);
        Task<DashboardAlertsDTO> Create(DashboardAlertsDTO argModelDto);
        Task<DashboardAlertsDTO> Update(DashboardAlertsDTO argModelDto);
        Task<int> Delete(int id);
        //Custom Methods
        Task<DashboardAlertsVM> GetActiveAlert();
        Task<List<DropdownVM>> GetDropdown();
    }
}
