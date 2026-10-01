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
    public class WebSiteLogsService : IWebSiteLogsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public WebSiteLogsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        //Common Methods
        public async Task<List<WebSiteLogsVM>> Get()
        {
            var models = await _unitOfWork.WebSiteLogsRepo.Get().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVms = _mapper.Map<List<WebSiteLogsVM>>(models);
            if (modelVms == null || modelVms.Count <= 0) return null;
            return modelVms;
        }
        public async Task<WebSiteLogsDTO> Get(int id)
        {
            var model = await _unitOfWork.WebSiteLogsRepo.Get(id).ConfigureAwait(false);
            if (model == null) return null;
            var modelDto = _mapper.Map<WebSiteLogsDTO>(model);
            return modelDto;
        }
        public async Task<WebSiteLogsVM> GetVM(int id)
        {
            var model = await _unitOfWork.WebSiteLogsRepo.Get(id).ConfigureAwait(false);
            if (model == null) return null;
            var modelVm = _mapper.Map<WebSiteLogsVM>(model);
            return modelVm;
        }
        public async Task<WebSiteLogsDTO> Create(WebSiteLogsDTO modelDto)
        {
            if (modelDto == null) return null;
            var model = _mapper.Map<WebSiteLogs>(modelDto);
            _unitOfWork.WebSiteLogsRepo.Create(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            modelDto = _mapper.Map<WebSiteLogsDTO>(model);
            return rowsChanged > 0 ? modelDto : null;
        }
        public async Task<WebSiteLogsDTO> Update(WebSiteLogsDTO modelDto)
        {
            if (modelDto == null) return null;
            var ExistingModel = await _unitOfWork.WebSiteLogsRepo.Get(modelDto.Id).ConfigureAwait(false);
            if (ExistingModel != null)
            {
                var AttachedModel = _unitOfWork.WebSiteLogsRepo.GetEntityEntry(ExistingModel);
                AttachedModel.CurrentValues.SetValues(modelDto);
            }
            //_unitOfWork.WebSiteLogsRepo.Update(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? modelDto : null;
        }
        public async Task<int> Delete(int id)
        {
            var model = await _unitOfWork.WebSiteLogsRepo.Get(id).ConfigureAwait(false);
            if (model == null) return -1;
            _unitOfWork.WebSiteLogsRepo.Delete(model);
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
        public async Task<List<WebSiteLogsDTO>> CreateRange(List<WebSiteLogsDTO> modelDtos)
        {
            if (modelDtos == null || modelDtos.Count <= 0) return null;
            var models = _mapper.Map<List<WebSiteLogs>>(modelDtos);
            _unitOfWork.WebSiteLogsRepo.CreateRange(models);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            modelDtos = _mapper.Map<List<WebSiteLogsDTO>>(models);
            return rowsChanged > 0 ? modelDtos : null;
        }
        public async Task<List<WebSiteLogsDTO>> Upsert(List<WebSiteLogsDTO> modelDtos)
        {
            if (modelDtos == null || modelDtos.Count <= 0) return null;
            var models = _mapper.Map<List<WebSiteLogs>>(modelDtos);
            foreach (var row in models)
            {
                if (_unitOfWork.WebSiteLogsRepo.GetEntityState(row) != EntityState.Detached) continue;
                var ExistingRow = await _unitOfWork.WebSiteLogsRepo.Get(row.Id).ConfigureAwait(false);
                if (ExistingRow != null)
                {
                    var attachedEntry = _unitOfWork.WebSiteLogsRepo.GetEntityEntry(ExistingRow);
                    attachedEntry.CurrentValues.SetValues(row);
                }
                else
                {
                    _unitOfWork.WebSiteLogsRepo.Create(row);
                }
            }
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            modelDtos = _mapper.Map<List<WebSiteLogsDTO>>(models);
            return rowsChanged > 0 ? modelDtos : null;
        }
        public async Task<int> DeleteRange(List<WebSiteLogsDTO> modelDtos)
        {
            if (modelDtos == null || modelDtos.Count <= 0) return -1;
            _unitOfWork.WebSiteLogsRepo.DeleteRange(_mapper.Map<List<WebSiteLogs>>(modelDtos));
            return await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
