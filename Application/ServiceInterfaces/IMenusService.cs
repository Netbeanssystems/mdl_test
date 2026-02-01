using Application.Dtos;
using Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Application.ServiceInterfaces
{
    public interface IMenusService
    {
        //Common Methods
        Task<List<MenusVM>> Get();
        Task<MenuDTO> Get(int id);
        Task<MenuDTO> Create(MenuDTO argModelDto);
        Task<MenuDTO> Update(MenuDTO argModelDto);
        Task<int> Delete(int id);
        //Custom Methods
        Task<List<MenusVM>> GetWithAll();
        Task<List<MenusVM>> GetAllByRole(string role);
    }
}
