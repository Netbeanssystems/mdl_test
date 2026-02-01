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
    public class TempMediaService : ITempMediaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TempMediaService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<TempMediaVM>> Get()
        {
            var tempNewProjects = await _unitOfWork.TempMediaRepo.Get().ConfigureAwait(false);
            if (tempNewProjects == null || tempNewProjects.Count <= 0) return null;
            var tempNewProjectVms = _mapper.Map<List<TempMediaVM>>(tempNewProjects);
            if (tempNewProjectVms == null || tempNewProjectVms.Count <= 0) return null;
            return tempNewProjectVms;
        }

        public async Task<TempMediaDTO> Get(int id)
        {
            var tempNewProject = await _unitOfWork.TempMediaRepo.Get(id).ConfigureAwait(false);
            if (tempNewProject == null) return null;
            var tempNewProjectDto = _mapper.Map<TempMediaDTO>(tempNewProject);
            return tempNewProjectDto;
        }

        public async Task<TempMediaDTO> Add(TempMediaDTO TemppressReleaseDTO)
        {
            if (TemppressReleaseDTO == null) return null;
            var tempNewProject = _mapper.Map<TempMedia>(TemppressReleaseDTO);
            _unitOfWork.TempMediaRepo.Create(tempNewProject);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? TemppressReleaseDTO : null;
        }

        public async Task<TempMediaDTO> Update(TempMediaDTO TemppressReleaseDTO)
        {
            if (TemppressReleaseDTO == null) return null;
            var tempNewProject = _mapper.Map<TempMedia>(TemppressReleaseDTO);
            _unitOfWork.TempMediaRepo.Update(tempNewProject);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? TemppressReleaseDTO : null;
        }

        public async Task<int> Remove(int id)
        {
            var tempNewProject = await _unitOfWork.TempMediaRepo.Get(id).ConfigureAwait(false);
            if (tempNewProject == null) return -1;
            _unitOfWork.TempMediaRepo.Delete(tempNewProject);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? rowsChanged : -1;
        }

        public async Task<TempMediaDTO> CreateAudit(TempMediaDTO TemppressReleaseDTO)
        {
            if (TemppressReleaseDTO == null) return null;
            var tempNewProject = _mapper.Map<TempMedia>(TemppressReleaseDTO);
            _unitOfWork.TempMediaRepo.CreateAudit(tempNewProject);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? TemppressReleaseDTO : null;
        }
        public async Task<List<TempMediaVM>> GetByAction(string status)
        {
            var TempPress = await _unitOfWork.TempMediaRepo.GetByAction(status).ConfigureAwait(false);
            if (TempPress == null || TempPress.Count <= 0) return null;
            var TempPressVms = _mapper.Map<List<TempMediaVM>>(TempPress);
            return TempPressVms;
        }
    }
}
