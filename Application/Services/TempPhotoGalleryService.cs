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
    public class TempPhotoGalleryService : ITempPhotoGalleryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TempPhotoGalleryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<List<TempPhotoGalleryVM>> Get()
        {
            var tempvideo = await _unitOfWork.TempPhotoGalleryRepo.Get().ConfigureAwait(false);
            if (tempvideo == null || tempvideo.Count <= 0) return null;
            var tempvideoVms = _mapper.Map<List<TempPhotoGalleryVM>>(tempvideo);
            if (tempvideoVms == null || tempvideoVms.Count <= 0) return null;
            return tempvideoVms;
        }

        public async Task<TempPhotoGalleryDTO> Get(int id)
        {
            var tempvideo = await _unitOfWork.TempPhotoGalleryRepo.Get(id).ConfigureAwait(false);
            if (tempvideo == null) return null;
            var tempvideoDto = _mapper.Map<TempPhotoGalleryDTO>(tempvideo);
            return tempvideoDto;
        }
        public async Task<TempPhotoGalleryDTO> Add(TempPhotoGalleryDTO TempVideoDto)
        {
            if (TempVideoDto == null) return null;
            var tempvideo = _mapper.Map<TempPhotoGallery>(TempVideoDto);
            _unitOfWork.TempPhotoGalleryRepo.Create(tempvideo);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? TempVideoDto : null;
        }

        public async Task<TempPhotoGalleryDTO> Update(TempPhotoGalleryDTO TempVideoDto)
        {
            if (TempVideoDto == null) return null;
            var tempNewProject = _mapper.Map<TempPhotoGallery>(TempVideoDto);
            _unitOfWork.TempPhotoGalleryRepo.Update(tempNewProject);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? TempVideoDto : null;
        }

        public async Task<int> Remove(int id)
        {
            var tempvideo = await _unitOfWork.TempPhotoGalleryRepo.Get(id).ConfigureAwait(false);
            if (tempvideo == null) return -1;
            _unitOfWork.TempPhotoGalleryRepo.Delete(tempvideo);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? rowsChanged : -1;
        }

        public async Task<TempPhotoGalleryDTO> CreateAudit(TempPhotoGalleryDTO TempVideoDto)
        {
            if (TempVideoDto == null) return null;
            var tempvideo = _mapper.Map<TempPhotoGallery>(TempVideoDto);
            _unitOfWork.TempPhotoGalleryRepo.CreateAudit(tempvideo);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? TempVideoDto : null;
        }
        public async Task<List<TempPhotoGalleryVM>> GetByAction(string status)
        {
            var TempQuick = await _unitOfWork.TempPhotoGalleryRepo.GetByAction(status).ConfigureAwait(false);
            if (TempQuick == null || TempQuick.Count <= 0) return null;
            var TempQuickVms = _mapper.Map<List<TempPhotoGalleryVM>>(TempQuick);
            return TempQuickVms;
        }
    }
}
