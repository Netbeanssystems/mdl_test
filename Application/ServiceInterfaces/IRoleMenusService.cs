using Application.Dtos;
using Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Application.ServiceInterfaces
{
    public interface IRoleMenusService
    {
        //Common Methods
        Task<List<RoleMenusVM>> Get();
        Task<RoleMenusDTO> Get(int id);
        Task<RoleMenusDTO> Create(RoleMenusDTO argModelDto);
        Task<RoleMenusDTO> Update(RoleMenusDTO argModelDto);
        Task<int> Delete(int id);
        //Custom Methods
        Task<RoleMenusVM> GetAllByRole(string rolename);
        Task<RoleMenusVM> AssignToRole(RoleMenusDTO argModelDto);
        Task<RoleMenusVM> RemoveFromRole(RoleMenusDTO argModelDto);
        Task<int> UpdateMenus(List<RoleMenusDTO> argModelDtos);
    }
}
