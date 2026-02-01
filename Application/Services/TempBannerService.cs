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
    public class TempBannerService : ITempBannerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public TempBannerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        //Common Methods
        public async Task<List<TempBannerVM>> Get()
        {
            var models = await _unitOfWork.TempBannerRepo.Get().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var tempModelVms = _mapper.Map<List<TempBannerVM>>(models);
            if (tempModelVms == null || tempModelVms.Count <= 0) return null;
            return tempModelVms.OrderBy(x => x.Priority).ToList();
        }
        public async Task<TempBannerDTO> Get(int id)
        {
            var tempModels = await _unitOfWork.TempBannerRepo.Get(id).ConfigureAwait(false);
            if (tempModels == null) return null;
            var tempModelsDto = _mapper.Map<TempBannerDTO>(tempModels);
            return tempModelsDto;
        }
        public async Task<TempBannerDTO> Add(TempBannerDTO modelDto)
        {
            if (modelDto == null) return null;
            var tempModel = _mapper.Map<TempBanners>(modelDto);
            _unitOfWork.TempBannerRepo.Create(tempModel);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? modelDto : null;
        }
        public async Task<TempBannerDTO> Update(TempBannerDTO modelDto)
        {
            if (modelDto == null) return null;
            var tempModel = _mapper.Map<TempBanners>(modelDto);
            _unitOfWork.TempBannerRepo.Update(tempModel);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? modelDto : null;
        }
        public async Task<int> Remove(int id)
        {
            var tempModel = await _unitOfWork.TempBannerRepo.Get(id).ConfigureAwait(false);
            if (tempModel == null) return -1;
            _unitOfWork.TempBannerRepo.Delete(tempModel);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? rowsChanged : -1;
        }
        //Custom Methods
        public async Task<TempBannerDTO> CreateAudit(TempBannerDTO modelDto)
        {
            if (modelDto == null) return null;
            var tempModel = _mapper.Map<TempBanners>(modelDto);
            _unitOfWork.TempBannerRepo.CreateAudit(tempModel);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? modelDto : null;
        }
        public async Task<List<TempBannerVM>> GetByAction(string status)
        {
            var TempMenu = await _unitOfWork.TempBannerRepo.GetByAction(status).ConfigureAwait(false);
            if (TempMenu == null || TempMenu.Count <= 0) return null;
            var TempMenuVms = _mapper.Map<List<TempBannerVM>>(TempMenu);
            return TempMenuVms;
        }
    }
}
