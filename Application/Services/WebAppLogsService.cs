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
    public class WebAppLogsService : IWebAppLogsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public WebAppLogsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        //Common Methods
        public async Task<List<WebAppLogsVM>> Get()
        {
            var models = await _unitOfWork.WebAppLogsRepo.Get().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVms = _mapper.Map<List<WebAppLogsVM>>(models);
            if (modelVms == null || modelVms.Count <= 0) return null;
            return modelVms;
        }
        public async Task<WebAppLogsDTO> Get(int id)
        {
            var model = await _unitOfWork.WebAppLogsRepo.Get(id).ConfigureAwait(false);
            if (model == null) return null;
            var modelDto = _mapper.Map<WebAppLogsDTO>(model);
            return modelDto;
        }
        public async Task<WebAppLogsVM> GetVM(int id)
        {
            var model = await _unitOfWork.WebAppLogsRepo.Get(id).ConfigureAwait(false);
            if (model == null) return null;
            var modelVm = _mapper.Map<WebAppLogsVM>(model);
            return modelVm;
        }
        public async Task<WebAppLogsDTO> Create(WebAppLogsDTO modelDto)
        {
            if (modelDto == null) return null;
            var model = _mapper.Map<WebAppLogs>(modelDto);
            _unitOfWork.WebAppLogsRepo.Create(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            modelDto = _mapper.Map<WebAppLogsDTO>(model);
            return rowsChanged > 0 ? modelDto : null;
        }
        public async Task<WebAppLogsDTO> Update(WebAppLogsDTO modelDto)
        {
            if (modelDto == null) return null;
            var ExistingModel = await _unitOfWork.WebAppLogsRepo.Get(modelDto.Id).ConfigureAwait(false);
            if (ExistingModel != null)
            {
                var AttachedModel = _unitOfWork.WebAppLogsRepo.GetEntityEntry(ExistingModel);
                AttachedModel.CurrentValues.SetValues(modelDto);
            }
            //_unitOfWork.WebAppLogsRepo.Update(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? modelDto : null;
        }
        public async Task<int> Delete(int id)
        {
            var model = await _unitOfWork.WebAppLogsRepo.Get(id).ConfigureAwait(false);
            if (model == null) return -1;
            _unitOfWork.WebAppLogsRepo.Delete(model);
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
        public async Task<List<WebAppLogsDTO>> CreateRange(List<WebAppLogsDTO> modelDtos)
        {
            if (modelDtos == null || modelDtos.Count <= 0) return null;
            var models = _mapper.Map<List<WebAppLogs>>(modelDtos);
            _unitOfWork.WebAppLogsRepo.CreateRange(models);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            modelDtos = _mapper.Map<List<WebAppLogsDTO>>(models);
            return rowsChanged > 0 ? modelDtos : null;
        }
        public async Task<List<WebAppLogsDTO>> Upsert(List<WebAppLogsDTO> modelDtos)
        {
            if (modelDtos == null || modelDtos.Count <= 0) return null;
            var models = _mapper.Map<List<WebAppLogs>>(modelDtos);
            foreach (var row in models)
            {
                if (_unitOfWork.WebAppLogsRepo.GetEntityState(row) != EntityState.Detached) continue;
                var ExistingRow = await _unitOfWork.WebAppLogsRepo.Get(row.Id).ConfigureAwait(false);
                if (ExistingRow != null)
                {
                    var attachedEntry = _unitOfWork.WebAppLogsRepo.GetEntityEntry(ExistingRow);
                    attachedEntry.CurrentValues.SetValues(row);
                }
                else
                {
                    _unitOfWork.WebAppLogsRepo.Create(row);
                }
            }
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            modelDtos = _mapper.Map<List<WebAppLogsDTO>>(models);
            return rowsChanged > 0 ? modelDtos : null;
        }
        public async Task<int> DeleteRange(List<WebAppLogsDTO> modelDtos)
        {
            if (modelDtos == null || modelDtos.Count <= 0) return -1;
            _unitOfWork.WebAppLogsRepo.DeleteRange(_mapper.Map<List<WebAppLogs>>(modelDtos));
            return await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
