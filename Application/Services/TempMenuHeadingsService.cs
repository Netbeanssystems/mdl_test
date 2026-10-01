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
    public class TempMenuHeadingsService : ITempMenuHeadingsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public TempMenuHeadingsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        //Common Methods
        public async Task<List<TempMenuHeadingsVM>> Get()
        {
            var models = await _unitOfWork.TempMenuHeadingsRepo.Get().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var tempModelVms = _mapper.Map<List<TempMenuHeadingsVM>>(models);
            if (tempModelVms == null || tempModelVms.Count <= 0) return null;
            return tempModelVms.OrderBy(x => x.Priority).ToList();
        }
        public async Task<TempMenuHeadingsDTO> Get(int id)
        {
            var tempModels = await _unitOfWork.TempMenuHeadingsRepo.Get(id).ConfigureAwait(false);
            if (tempModels == null) return null;
            var tempModelsDto = _mapper.Map<TempMenuHeadingsDTO>(tempModels);
            return tempModelsDto;
        }
        public async Task<TempMenuHeadingsDTO> Add(TempMenuHeadingsDTO modelDto)
        {
            if (modelDto == null) return null;
            var tempModel = _mapper.Map<TempMenuHeadings>(modelDto);
            _unitOfWork.TempMenuHeadingsRepo.Create(tempModel);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? modelDto : null;
        }
        public async Task<TempMenuHeadingsDTO> Update(TempMenuHeadingsDTO modelDto)
        {
            if (modelDto == null) return null;
            var tempModel = _mapper.Map<TempMenuHeadings>(modelDto);
            _unitOfWork.TempMenuHeadingsRepo.Update(tempModel);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? modelDto : null;
        }
        public async Task<int> Remove(int id)
        {
            var tempModel = await _unitOfWork.TempMenuHeadingsRepo.Get(id).ConfigureAwait(false);
            if (tempModel == null) return -1;
            _unitOfWork.TempMenuHeadingsRepo.Delete(tempModel);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? rowsChanged : -1;
        }
        //Custom Methods
        public async Task<TempMenuHeadingsDTO> CreateAudit(TempMenuHeadingsDTO modelDto)
        {
            if (modelDto == null) return null;
            var tempModel = _mapper.Map<TempMenuHeadings>(modelDto);
            _unitOfWork.TempMenuHeadingsRepo.CreateAudit(tempModel);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? modelDto : null;
        }
        public async Task<List<TempMenuHeadingsVM>> GetByAction(string status)
        {
            var TempMenu = await _unitOfWork.TempMenuHeadingsRepo.GetByAction(status).ConfigureAwait(false);
            if (TempMenu == null || TempMenu.Count <= 0) return null;
            var TempMenuVms = _mapper.Map<List<TempMenuHeadingsVM>>(TempMenu);
            return TempMenuVms;
        }
        public async Task<List<TempMenuHeadingsVM>> GetByActionupdate(string status)
        {
            var TempMenu = await _unitOfWork.TempMenuHeadingsRepo.GetByActionupdate(status).ConfigureAwait(false);
            if (TempMenu == null || TempMenu.Count <= 0) return null;
            var TempMenuVms = _mapper.Map<List<TempMenuHeadingsVM>>(TempMenu);
            return TempMenuVms;
        }
    }
}