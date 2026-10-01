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
    public class VideoService : IVideoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public VideoService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<VideoVM>> Get()
        {
            var newProjects = await _unitOfWork.VideoRepo.Get().ConfigureAwait(false);
            if (newProjects == null || newProjects.Count <= 0) return null;
            var newProjectVms = _mapper.Map<List<VideoVM>>(newProjects);
            if (newProjectVms == null || newProjectVms.Count <= 0) return null;
            return newProjectVms;
        }

        public async Task<VideoDTO> Get(int id)
        {
            var newProject = await _unitOfWork.VideoRepo.Get(id).ConfigureAwait(false);
            if (newProject == null) return null;
            var newProjectDto = _mapper.Map<VideoDTO>(newProject);
            return newProjectDto;
        }

        public async Task<VideoDTO> Add(VideoDTO videoDto)
        {
            if (videoDto == null) return null;
            var newMarquee = _mapper.Map<Video>(videoDto);
            _unitOfWork.VideoRepo.Create(newMarquee);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            videoDto.Id = newMarquee.Id;
            return rowsChanged > 0 ? videoDto : null;
        }

        public async Task<VideoDTO> Update(VideoDTO videoDto)
        {
            if (videoDto == null) return null;
            var newMarquee = _mapper.Map<Video>(videoDto);
            _unitOfWork.VideoRepo.Update(newMarquee);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? videoDto : null;
        }

        public async Task<int> Remove(int id)
        {
            var newProject = await _unitOfWork.VideoRepo.Get(id).ConfigureAwait(false);
            if (newProject == null) return -1;
            _unitOfWork.VideoRepo.Delete(newProject);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? rowsChanged : -1;
        }
    }
}
