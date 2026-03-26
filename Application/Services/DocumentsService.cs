using Application.Dtos;
using Application.ServiceInterfaces;
using Application.ViewModels;
using AutoMapper;
using Domain.Models;
using Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class DocumentsService : IDocumentsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public DocumentsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        //Common Methods
        public async Task<List<DocumentsVM>> Get()
        {
            var models = await _unitOfWork.DocumentsRepo.Get().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVms = _mapper.Map<List<DocumentsVM>>(models);
            if (modelVms == null || modelVms.Count <= 0) return null;
            return modelVms;
        }
        public async Task<DocumentsDTO> Get(int id)
        {
            var model = await _unitOfWork.DocumentsRepo.Get(id).ConfigureAwait(false);
            if (model == null) return null;
            var modelDto = _mapper.Map<DocumentsDTO>(model);
            return modelDto;
        }

        public async Task<List<DocumentsVM>> Getbycreatedby(string createdby)
        {
            var models = await _unitOfWork.DocumentsRepo.Getbycreatedby(createdby).ConfigureAwait(false);
            if (models == null) return null;
            var modelVms = _mapper.Map<List<DocumentsVM>>(models);
            if (modelVms == null || modelVms.Count <= 0) return null;
            return modelVms;
        }

        public async Task<List<DocumentsVM>> GetByYearAndDescription(int? year, int? urlsTimingId)
        {
            var models = await _unitOfWork.DocumentsRepo.GetByYearAndDescription(year, urlsTimingId).ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVms = _mapper.Map<List<DocumentsVM>>(models);
            if (modelVms == null || modelVms.Count <= 0) return null;
            return modelVms;
        }

        public async Task<List<DocumentsVM>> GetGroupedByURLsTiming()
        {
            var models = await _unitOfWork.DocumentsRepo.GetGroupedByURLsTiming().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVms = _mapper.Map<List<DocumentsVM>>(models);
            if (modelVms == null || modelVms.Count <= 0) return null;

            // Group by URLsTimingId and add count
            var grouped = modelVms.GroupBy(x => x.URLsTimingId)
                .SelectMany(g => g.Select(item =>
                {
                    item.DocumentCount = g.Count();
                    return item;
                })).ToList();

            return grouped;
        }

        public async Task<DocumentsDTO> Create(DocumentsDTO modelDto)
        {
            if (modelDto == null) return null;
            var model = _mapper.Map<Documents>(modelDto);
            _unitOfWork.DocumentsRepo.Create(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            modelDto = _mapper.Map<DocumentsDTO>(model);
            return rowsChanged > 0 ? modelDto : null;
        }
        public async Task<DocumentsDTO> Update(DocumentsDTO modelDto)
        {
            if (modelDto == null) return null;
            _unitOfWork.DocumentsRepo.Update(_mapper.Map<Documents>(modelDto));
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? modelDto : null;
        }
        public async Task<int> Delete(int id)
        {
            var model = await _unitOfWork.DocumentsRepo.Get(id).ConfigureAwait(false);
            if (model == null) return -1;
            _unitOfWork.DocumentsRepo.Delete(model);
            return await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }
        public async Task<List<DocumentsDTO>> CreateRange(List<DocumentsDTO> modelDtos)
        {
            if (modelDtos == null || modelDtos.Count <= 0) return null;
            var models = _mapper.Map<List<Documents>>(modelDtos);
            _unitOfWork.DocumentsRepo.CreateRange(models);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            modelDtos = _mapper.Map<List<DocumentsDTO>>(models);
            return rowsChanged > 0 ? modelDtos : null;
        }
        public async Task<List<DocumentsDTO>> Upsert(List<DocumentsDTO> modelDtos)
        {
            if (modelDtos == null || modelDtos.Count <= 0) return null;
            var models = _mapper.Map<List<Documents>>(modelDtos);
            foreach (var row in models)
            {
                if (_unitOfWork.DocumentsRepo.GetEntityState(row) != EntityState.Detached) continue;
                var ExistingRow = await _unitOfWork.DocumentsRepo.Get(row.Id).ConfigureAwait(false);
                if (ExistingRow != null)
                {
                    var attachedEntry = _unitOfWork.DocumentsRepo.GetEntityEntry(ExistingRow);
                    attachedEntry.CurrentValues.SetValues(row);
                }
                else
                {
                    _unitOfWork.DocumentsRepo.Create(row);
                }
            }
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            modelDtos = _mapper.Map<List<DocumentsDTO>>(models);
            return rowsChanged > 0 ? modelDtos : null;
        }
        public async Task<int> DeleteRange(List<DocumentsDTO> modelDtos)
        {
            if (modelDtos == null || modelDtos.Count <= 0) return -1;
            _unitOfWork.DocumentsRepo.DeleteRange(_mapper.Map<List<Documents>>(modelDtos));
            return await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }
        //Custom Methods
        public async Task<List<DropdownVM>> GetDropdown()
        {
            var models = await _unitOfWork.DocumentsRepo.GetActive().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVms = _mapper.Map<List<DropdownVM>>(models);
            if (modelVms == null || modelVms.Count <= 0) return null;
            return modelVms;
        }

        public async Task<URLsTimingDTO> CreateURLsTiming(URLsTimingDTO modelDto)
        {
            if (modelDto == null) return null;
            var model = _mapper.Map<URLsTiming>(modelDto);
            _unitOfWork.URLsTimingRepo.Create(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            modelDto = _mapper.Map<URLsTimingDTO>(model);
            return rowsChanged > 0 ? modelDto : null;
        }

        public async Task<List<URLsTimingVM>> GetURLsTiming()
        {
            var models = await _unitOfWork.URLsTimingRepo.Get().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVm = _mapper.Map<List<URLsTimingVM>>(models);
            if (modelVm == null) return null;
            return modelVm;
        }

        public async Task<List<URLsTimingVM>> GetActiveURLsTiming()
        {
            var models = await _unitOfWork.URLsTimingRepo.GetActive().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVm = _mapper.Map<List<URLsTimingVM>>(models);
            if (modelVm == null) return null;
            return modelVm;
        }

        public async Task<URLsTimingVM> GetCurrentActiveURLsTiming(string url)
        {
            var models = await _unitOfWork.URLsTimingRepo.GetActive().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            
            var model = models.Where(x => x.Url == url)
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();
            
            if (model == null) return null;
            
            var modelVm = _mapper.Map<URLsTimingVM>(model);
            
            // Check if current time is within the window
            var now = DateTime.Now;
            if (now >= model.FromTime && now <= model.ToTime)
            {
                return modelVm;
            }
            
            return null;
        }

        public async Task<List<int>> GetDocumentsYears()
        {
            var models = await _unitOfWork.DocumentsRepo.Get().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            
            var years = models.Select(x => x.CreatedDate.Year)
                .Distinct()
                .OrderByDescending(x => x)
                .ToList();
            
            return years;
        }
    }
}
