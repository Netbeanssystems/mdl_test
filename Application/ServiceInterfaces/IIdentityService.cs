using Application.Dtos;
using Application.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Application.ServiceInterfaces
{
    public interface IIdentityService
    {
        //Users
        Task<List<ApplicationUser>> GetUsers();
        Task<ApplicationUser> GetUser(string id);
        Task<IList<ApplicationUser>> GetUsersByRole(string role);
        //Roles
        Task<List<ApplicationRole>> GetRoles();
        Task<ApplicationRole> GetRole(string id);
        Task<ApplicationRole> GetRoleByName(string rolename);
        Task<IdentityResult> AddRole(ApplicationRole role);
        Task<IdentityResult> EditRole(ApplicationRole role);
        Task<IdentityResult> DeleteRole(ApplicationRole role);
        Task<bool> RoleExists(string rolename);
        Task<List<string>> GetUserRoles(ApplicationUser user);
        Task<UserRolesVM> GetRolesForUser(ApplicationUser user);
        Task<UserRoleVM> GetRoleForUser(ApplicationUser user);
        Task<IdentityResult> AddToRole(ApplicationUser user, string role);
        Task<IdentityResult> RemoveFromRole(ApplicationUser user, string role);

        //NotInUse
        //Task<IdentityResult> RemoveFromRoles(ApplicationUser user, List<string> roles);
        Task<IdentityResult> Update(ApplicationUser user);
        //Task<IdentityResult> RemoveClaimAsync(ApplicationUser user, Claim claim);
        //Task<IdentityResult> AddClaimAsync(ApplicationUser user, Claim claim);
        Task<ApplicationUser> CreateUser(RegisterDTO model);
    }
}