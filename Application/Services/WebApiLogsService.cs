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
    public class WebApiLogsService : IWebApiLogsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public WebApiLogsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        //Common Methods
        public async Task<List<WebApiLogsVM>> Get()
        {
            var models = await _unitOfWork.WebApiLogsRepo.Get().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVms = _mapper.Map<List<WebApiLogsVM>>(models);
            if (modelVms == null || modelVms.Count <= 0) return null;
            return modelVms;
        }
        public async Task<WebApiLogsDTO> Get(int id)
        {
            var model = await _unitOfWork.WebApiLogsRepo.Get(id).ConfigureAwait(false);
            if (model == null) return null;
            var modelDto = _mapper.Map<WebApiLogsDTO>(model);
            return modelDto;
        }
        public async Task<WebApiLogsVM> GetVM(int id)
        {
            var model = await _unitOfWork.WebApiLogsRepo.Get(id).ConfigureAwait(false);
            if (model == null) return null;
            var modelVm = _mapper.Map<WebApiLogsVM>(model);
            return modelVm;
        }
        public async Task<WebApiLogsDTO> Create(WebApiLogsDTO modelDto)
        {
            if (modelDto == null) return null;
            var model = _mapper.Map<WebApiLogs>(modelDto);
            _unitOfWork.WebApiLogsRepo.Create(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            modelDto = _mapper.Map<WebApiLogsDTO>(model);
            return rowsChanged > 0 ? modelDto : null;
        }
        public async Task<WebApiLogsDTO> Update(WebApiLogsDTO modelDto)
        {
            if (modelDto == null) return null;
            var ExistingModel = await _unitOfWork.WebApiLogsRepo.Get(modelDto.Id).ConfigureAwait(false);
            if (ExistingModel != null)
            {
                var AttachedModel = _unitOfWork.WebApiLogsRepo.GetEntityEntry(ExistingModel);
                AttachedModel.CurrentValues.SetValues(modelDto);
            }
            //_unitOfWork.WebApiLogsRepo.Update(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? modelDto : null;
        }
        public async Task<int> Delete(int id)
        {
            var model = await _unitOfWork.WebApiLogsRepo.Get(id).ConfigureAwait(false);
            if (model == null) return -1;
            _unitOfWork.WebApiLogsRepo.Delete(model);
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
        public async Task<List<WebApiLogsDTO>> CreateRange(List<WebApiLogsDTO> modelDtos)
        {
            if (modelDtos == null || modelDtos.Count <= 0) return null;
            var models = _mapper.Map<List<WebApiLogs>>(modelDtos);
            _unitOfWork.WebApiLogsRepo.CreateRange(models);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            modelDtos = _mapper.Map<List<WebApiLogsDTO>>(models);
            return rowsChanged > 0 ? modelDtos : null;
        }
        public async Task<List<WebApiLogsDTO>> Upsert(List<WebApiLogsDTO> modelDtos)
        {
            if (modelDtos == null || modelDtos.Count <= 0) return null;
            var models = _mapper.Map<List<WebApiLogs>>(modelDtos);
            foreach (var row in models)
            {
                if (_unitOfWork.WebApiLogsRepo.GetEntityState(row) != EntityState.Detached) continue;
                var ExistingRow = await _unitOfWork.WebApiLogsRepo.Get(row.Id).ConfigureAwait(false);
                if (ExistingRow != null)
                {
                    var attachedEntry = _unitOfWork.WebApiLogsRepo.GetEntityEntry(ExistingRow);
                    attachedEntry.CurrentValues.SetValues(row);
                }
                else
                {
                    _unitOfWork.WebApiLogsRepo.Create(row);
                }
            }
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            modelDtos = _mapper.Map<List<WebApiLogsDTO>>(models);
            return rowsChanged > 0 ? modelDtos : null;
        }
        public async Task<int> DeleteRange(List<WebApiLogsDTO> modelDtos)
        {
            if (modelDtos == null || modelDtos.Count <= 0) return -1;
            _unitOfWork.WebApiLogsRepo.DeleteRange(_mapper.Map<List<WebApiLogs>>(modelDtos));
            return await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
