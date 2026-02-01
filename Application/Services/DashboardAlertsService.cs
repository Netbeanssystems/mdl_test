using Application.Dtos;
using Application.ServiceInterfaces;
using Application.ViewModels;
using AutoMapper;
using Domain.Models;
using Domain.RepositoryInterfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Application.Services
{
    public class DashboardAlertsService : IDashboardAlertsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public DashboardAlertsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        //Common Methods
        public async Task<List<DashboardAlertsVM>> Get()
        {
            var models = await _unitOfWork.DashboardAlertsRepo.Get().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVms = _mapper.Map<List<DashboardAlertsVM>>(models);
            if (modelVms == null || modelVms.Count <= 0) return null;
            return modelVms;
        }
        public async Task<DashboardAlertsDTO> Get(int id)
        {
            var model = await _unitOfWork.DashboardAlertsRepo.Get(id).ConfigureAwait(false);
            if (model == null) return null;
            var modelDto = _mapper.Map<DashboardAlertsDTO>(model);
            return modelDto;
        }
        public async Task<bool> CheckDuplicate(DashboardAlertsDTO argModelDto)
        {
            var model = _mapper.Map<DashboardAlerts>(argModelDto);
            var duplicate = await _unitOfWork.DashboardAlertsRepo.CheckDuplicate(model).ConfigureAwait(false);
            return duplicate != null;
        }
        public async Task<DashboardAlertsDTO> Create(DashboardAlertsDTO argModelDto)
        {
            if (argModelDto == null) return null;
            var model = _mapper.Map<DashboardAlerts>(argModelDto);
            _unitOfWork.DashboardAlertsRepo.Create(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? argModelDto : null;
        }
        public async Task<DashboardAlertsDTO> Update(DashboardAlertsDTO argModelDto)
        {
            if (argModelDto == null) return null;
            var model = _mapper.Map<DashboardAlerts>(argModelDto);
            _unitOfWork.DashboardAlertsRepo.Update(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? argModelDto : null;
        }
        public async Task<int> Delete(int id)
        {
            var model = await _unitOfWork.DashboardAlertsRepo.Get(id).ConfigureAwait(false);
            if (model == null) return -1;
            _unitOfWork.DashboardAlertsRepo.Delete(model);
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
        public async Task<DashboardAlertsVM> GetActiveAlert()
        {
            var models = await _unitOfWork.DashboardAlertsRepo.GetActiveAlert().ConfigureAwait(false);
            if (models == null) return null;
            var modelVms = _mapper.Map<DashboardAlertsVM>(models);
            return modelVms;
        }
        public async Task<List<DropdownVM>> GetDropdown()
        {
            var models = await _unitOfWork.DashboardAlertsRepo.GetActive().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVms = _mapper.Map<List<DropdownVM>>(models);
            if (modelVms == null || modelVms.Count <= 0) return null;
            return modelVms;
        }
    }
}
