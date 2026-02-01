using Application.Dtos;
using Application.ServiceInterfaces;
using Application.ViewModels;
using AutoMapper;
using Domain.Models;
using Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Application.Services
{
    public class LoginLogsService : ILoginLogsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public LoginLogsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        //Common Methods
        public async Task<List<LoginLogsVM>> Get()
        {
            var models = await _unitOfWork.LoginLogsRepo.Get().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVms = _mapper.Map<List<LoginLogsVM>>(models);
            if (modelVms == null || modelVms.Count <= 0) return null;
            return modelVms;
        }
        public async Task<LoginLogsDTO> Get(int id)
        {
            var model = await _unitOfWork.LoginLogsRepo.Get(id).ConfigureAwait(false);
            if (model == null) return null;
            var modelDto = _mapper.Map<LoginLogsDTO>(model);
            return modelDto;
        }
        public async Task<LoginLogsVM> GetVM(int id)
        {
            var model = await _unitOfWork.LoginLogsRepo.Get(id).ConfigureAwait(false);
            if (model == null) return null;
            var modelVm = _mapper.Map<LoginLogsVM>(model);
            return modelVm;
        }
        public async Task<LoginLogsDTO> Create(LoginLogsDTO modelDto)
        {
            if (modelDto == null) return null;
            var model = _mapper.Map<LoginLogs>(modelDto);
            _unitOfWork.LoginLogsRepo.Create(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            modelDto = _mapper.Map<LoginLogsDTO>(model);
            return rowsChanged > 0 ? modelDto : null;
        }
        public async Task<LoginLogsDTO> Update(LoginLogsDTO modelDto)
        {
            if (modelDto == null) return null;
            var updatedModel = _mapper.Map<LoginLogs>(modelDto);
            var oldModel = await _unitOfWork.LoginLogsRepo.Get(updatedModel.Id).ConfigureAwait(false);
            _unitOfWork.LoginLogsRepo.GetEntityEntry(oldModel).CurrentValues.SetValues(updatedModel);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? modelDto : null;
        }
        public async Task<int> Delete(int id)
        {
            var model = await _unitOfWork.LoginLogsRepo.Get(id).ConfigureAwait(false);
            if (model == null) return -1;
            _unitOfWork.LoginLogsRepo.Delete(model);
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
        public async Task<List<LoginLogsDTO>> CreateRange(List<LoginLogsDTO> modelDtos)
        {
            if (modelDtos == null || modelDtos.Count <= 0) return null;
            var models = _mapper.Map<List<LoginLogs>>(modelDtos);
            _unitOfWork.LoginLogsRepo.CreateRange(models);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            modelDtos = _mapper.Map<List<LoginLogsDTO>>(models);
            return rowsChanged > 0 ? modelDtos : null;
        }
        public async Task<List<LoginLogsDTO>> Upsert(List<LoginLogsDTO> modelDtos)
        {
            if (modelDtos == null || modelDtos.Count <= 0) return null;
            var models = _mapper.Map<List<LoginLogs>>(modelDtos);
            foreach (var row in models)
            {
                if (_unitOfWork.LoginLogsRepo.GetEntityState(row) != EntityState.Detached) continue;
                var ExistingRow = await _unitOfWork.LoginLogsRepo.Get(row.Id).ConfigureAwait(false);
                if (ExistingRow != null)
                {
                    var attachedEntry = _unitOfWork.LoginLogsRepo.GetEntityEntry(ExistingRow);
                    attachedEntry.CurrentValues.SetValues(row);
                }
                else
                {
                    _unitOfWork.LoginLogsRepo.Create(row);
                }
            }
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            modelDtos = _mapper.Map<List<LoginLogsDTO>>(models);
            return rowsChanged > 0 ? modelDtos : null;
        }
        public async Task<int> DeleteRange(List<LoginLogsDTO> modelDtos)
        {
            if (modelDtos == null || modelDtos.Count <= 0) return -1;
            _unitOfWork.LoginLogsRepo.DeleteRange(_mapper.Map<List<LoginLogs>>(modelDtos));
            return await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
