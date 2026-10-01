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
    public class MenusService : IMenusService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public MenusService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        //Common Methods
        public async Task<List<MenusVM>> Get()
        {
            var models = await _unitOfWork.MenusRepo.Get().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVms = _mapper.Map<List<MenusVM>>(models);
            if (modelVms == null || modelVms.Count <= 0) return null;
            return modelVms;
        }
        public async Task<MenuDTO> Get(int id)
        {
            var model = await _unitOfWork.MenusRepo.Get(id).ConfigureAwait(false);
            if (model == null) return null;
            var modelDto = _mapper.Map<MenuDTO>(model);
            return modelDto;
        }
        public async Task<MenuDTO> Create(MenuDTO argModelDto)
        {
            if (argModelDto == null) return null;
            var model = _mapper.Map<Menus>(argModelDto);
            _unitOfWork.MenusRepo.Create(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? argModelDto : null;
        }
        public async Task<MenuDTO> Update(MenuDTO argModelDto)
        {
            if (argModelDto == null) return null;
            var model = _mapper.Map<Menus>(argModelDto);
            _unitOfWork.MenusRepo.Update(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? argModelDto : null;
        }
        public async Task<int> Delete(int id)
        {
            var model = await _unitOfWork.MenusRepo.Get(id).ConfigureAwait(false);
            if (model == null) return -1;
            _unitOfWork.MenusRepo.Delete(model);
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
        public async Task<List<MenusVM>> GetWithAll()
        {
            var models = await _unitOfWork.MenusRepo.GetWithAll().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVms = _mapper.Map<List<MenusVM>>(models);
            return modelVms;
        }
        public async Task<List<MenusVM>> GetAllByRole(string role)
        {
            var models = await _unitOfWork.MenusRepo.GetWithAll().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            RemoveUnalloted(models, role);
            var modelVms = _mapper.Map<List<MenusVM>>(models);
            return modelVms;
        }
        private static void RemoveUnalloted(List<Menus> menus, string role)
        {
            foreach (var m in menus.ToList())
            {
                if (m.RoleMenus.All(rm => rm.RoleName != role))
                    menus.Remove(m);
                else
                    RemoveUnalloted(m.Children, role);
            }
        }
    }
}
