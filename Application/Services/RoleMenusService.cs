using Application.Dtos;
using Application.ServiceInterfaces;
using Application.ViewModels;
using AutoMapper;
using Domain.Models;
using Domain.RepositoryInterfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Application.Services
{
    public class RoleMenusService : IRoleMenusService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public RoleMenusService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        //Common Methods
        public async Task<List<RoleMenusVM>> Get()
        {
            var models = await _unitOfWork.RoleMenusRepo.Get().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVms = _mapper.Map<List<RoleMenusVM>>(models);
            if (modelVms == null || modelVms.Count <= 0) return null;
            return modelVms;
        }
        public async Task<RoleMenusDTO> Get(int id)
        {
            var model = await _unitOfWork.RoleMenusRepo.Get(id).ConfigureAwait(false);
            if (model == null) return null;
            var modelDto = _mapper.Map<RoleMenusDTO>(model);
            return modelDto;
        }
        public async Task<RoleMenusDTO> Create(RoleMenusDTO argModelDto)
        {
            if (argModelDto == null) return null;
            var model = _mapper.Map<RoleMenus>(argModelDto);
            _unitOfWork.RoleMenusRepo.Create(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? argModelDto : null;
        }
        public async Task<RoleMenusDTO> Update(RoleMenusDTO argModelDto)
        {
            if (argModelDto == null) return null;
            var model = _mapper.Map<RoleMenus>(argModelDto);
            _unitOfWork.RoleMenusRepo.Update(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? argModelDto : null;
        }
        public async Task<int> Delete(int id)
        {
            var model = await _unitOfWork.RoleMenusRepo.Get(id).ConfigureAwait(false);
            if (model == null) return -1;
            _unitOfWork.RoleMenusRepo.Delete(model);
            return await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            //var rowsChanged = -1;
            //try
            //{
            //    rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            //}
            //catch (Exception ex)
            //{
            //    if (ex.GetType() == typeof(DbUpdateException) || ex.GetType() == typeof(DbUpdateConcurrencyException))
            //        rowsChanged = -2;
            //}
            //return rowsChanged;
        }
        //Custom Methods
        public async Task<RoleMenusVM> GetAllByRole(string rolename)
        {
            return await GetRoleMenusByRole(rolename).ConfigureAwait(false);
        }
        public async Task<RoleMenusVM> AssignToRole(RoleMenusDTO argModelDto)
        {
            var model = _mapper.Map<RoleMenus>(argModelDto);
            _unitOfWork.RoleMenusRepo.Create(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            if (rowsChanged > 0)
                return await GetRoleMenusByRole(argModelDto.RoleName).ConfigureAwait(false);
            return null;
        }
        public async Task<RoleMenusVM> RemoveFromRole(RoleMenusDTO argModelDto)
        {
            var rolemodel = await _unitOfWork.RoleMenusRepo.GetByRoleMenu(argModelDto.RoleName, argModelDto.MenuId).ConfigureAwait(false);
            if (rolemodel == null) return await GetRoleMenusByRole(argModelDto.RoleName).ConfigureAwait(false);
            _unitOfWork.RoleMenusRepo.Delete(rolemodel);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            if (rowsChanged > 0)
                return await GetRoleMenusByRole(argModelDto.RoleName).ConfigureAwait(false);
            return null;
        }
        private async Task<RoleMenusVM> GetRoleMenusByRole(string rolename)
        {
            var assignedMenus = await _unitOfWork.RoleMenusRepo.GetAllByRole(rolename).ConfigureAwait(false);
            var models = await _unitOfWork.MenusRepo.Get().ConfigureAwait(false);
            var remainingMenus = models.Where(m => assignedMenus.All(am => am.MenuId != m.Id))
                .Select(m => new DropdownVM { Id = m.Id, Text = m.MenuText }).ToList();
            var roleMenuVm = new RoleMenusVM
            {
                roleName = rolename,
                assignedMenus = assignedMenus.Select(x => new DropdownVM { Id = x.MenuId, Text = x.Menu.MenuText }).ToList(),
                remainingMenus = remainingMenus
            };
            return roleMenuVm;
        }
        public async Task<int> UpdateMenus(List<RoleMenusDTO> argModelDtos)
        {
            if (argModelDtos == null || argModelDtos.Count <= 0) return 0;
            var role = argModelDtos[0].RoleName;
            var rawRoleMenus = _mapper.Map<List<RoleMenus>>(argModelDtos);
            var existingRoleMenus = await _unitOfWork.RoleMenusRepo.GetAllByRole(role).ConfigureAwait(false);
            var matchingRoleMenus = new List<RoleMenus>();
            var remainingRoleMenus = new List<RoleMenus>();
            foreach (var item in rawRoleMenus)
            {
                var match = existingRoleMenus.FirstOrDefault(erm => erm.RoleName == role && erm.MenuId == item.MenuId);
                if (match != null)
                    matchingRoleMenus.Add(match);
                else
                    remainingRoleMenus.Add(item);
            }
            var nonMatchingRoleMenus = existingRoleMenus.Except(matchingRoleMenus);
            foreach (var matchingRoleMenu in matchingRoleMenus)
                _unitOfWork.RoleMenusRepo.Update(matchingRoleMenu);
            _unitOfWork.RoleMenusRepo.CreateRange(remainingRoleMenus);
            _unitOfWork.RoleMenusRepo.DeleteRange(nonMatchingRoleMenus);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? rowsChanged : 0;
        }
    }
}
