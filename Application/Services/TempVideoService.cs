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
    public class TempVideoService : ITempVideoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TempVideoService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<TempVideoVM>> Get()
        {
            var tempvideo = await _unitOfWork.TempVideoRepo.Get().ConfigureAwait(false);
            if (tempvideo == null || tempvideo.Count <= 0) return null;
            var tempvideoVms = _mapper.Map<List<TempVideoVM>>(tempvideo);
            if (tempvideoVms == null || tempvideoVms.Count <= 0) return null;
            return tempvideoVms;
        }

        public async Task<TempVideoDTO> Get(int id)
        {
            var tempvideo = await _unitOfWork.TempVideoRepo.Get(id).ConfigureAwait(false);
            if (tempvideo == null) return null;
            var tempvideoDto = _mapper.Map<TempVideoDTO>(tempvideo);
            return tempvideoDto;
        }
        public async Task<TempVideoDTO> Add(TempVideoDTO TempVideoDto)
        {
            if (TempVideoDto == null) return null;
            var tempvideo = _mapper.Map<TempVideo>(TempVideoDto);
            _unitOfWork.TempVideoRepo.Create(tempvideo);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? TempVideoDto : null;
        }

        public async Task<TempVideoDTO> Update(TempVideoDTO TempVideoDto)
        {
            if (TempVideoDto == null) return null;
            var tempNewProject = _mapper.Map<TempVideo>(TempVideoDto);
            _unitOfWork.TempVideoRepo.Update(tempNewProject);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? TempVideoDto : null;
        }

        public async Task<int> Remove(int id)
        {
            var tempvideo = await _unitOfWork.TempVideoRepo.Get(id).ConfigureAwait(false);
            if (tempvideo == null) return -1;
            _unitOfWork.TempVideoRepo.Delete(tempvideo);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? rowsChanged : -1;
        }

        public async Task<TempVideoDTO> CreateAudit(TempVideoDTO TempVideoDto)
        {
            if (TempVideoDto == null) return null;
            var tempvideo = _mapper.Map<TempVideo>(TempVideoDto);
            _unitOfWork.TempVideoRepo.CreateAudit(tempvideo);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? TempVideoDto : null;
        }
        public async Task<List<TempVideoVM>> GetByAction(string status)
        {
            var TempQuick = await _unitOfWork.TempVideoRepo.GetByAction(status).ConfigureAwait(false);
            if (TempQuick == null || TempQuick.Count <= 0) return null;
            var TempQuickVms = _mapper.Map<List<TempVideoVM>>(TempQuick);
            return TempQuickVms;
        }

    }
}
