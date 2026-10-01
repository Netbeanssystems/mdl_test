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
    public class TempNewsService : ITempNewsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TempNewsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<List<TempNewsVM>> Get()
        {
            var tempNewProjects = await _unitOfWork.TempNewsRepo.Get().ConfigureAwait(false);
            if (tempNewProjects == null || tempNewProjects.Count <= 0) return null;
            var tempNewProjectVms = _mapper.Map<List<TempNewsVM>>(tempNewProjects);
            if (tempNewProjectVms == null || tempNewProjectVms.Count <= 0) return null;
            return tempNewProjectVms.OrderBy(x => x.Priority).ToList();
        }

        public async Task<TempNewsDTO> Get(int id)
        {
            var tempNewProject = await _unitOfWork.TempNewsRepo.Get(id).ConfigureAwait(false);
            if (tempNewProject == null) return null;
            var tempNewProjectDto = _mapper.Map<TempNewsDTO>(tempNewProject);
            return tempNewProjectDto;
        }

        public async Task<TempNewsDTO> Add(TempNewsDTO tempnewsDto)
        {
            if (tempnewsDto == null) return null;
            var tempNewProject = _mapper.Map<TempNews>(tempnewsDto);
            _unitOfWork.TempNewsRepo.Create(tempNewProject);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? tempnewsDto : null;
        }

        public async Task<TempNewsDTO> Update(TempNewsDTO tempnewsDto)
        {
            if (tempnewsDto == null) return null;
            var tempNewProject = _mapper.Map<TempNews>(tempnewsDto);
            _unitOfWork.TempNewsRepo.Update(tempNewProject);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? tempnewsDto : null;
        }

        public async Task<int> Remove(int id)
        {
            var tempNewProject = await _unitOfWork.TempNewsRepo.Get(id).ConfigureAwait(false);
            if (tempNewProject == null) return -1;
            _unitOfWork.TempNewsRepo.Delete(tempNewProject);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? rowsChanged : -1;
        }

        public async Task<TempNewsDTO> CreateAudit(TempNewsDTO tempnewsDto)
        {
            if (tempnewsDto == null) return null;
            var tempNewProject = _mapper.Map<TempNews>(tempnewsDto);
            _unitOfWork.TempNewsRepo.CreateAudit(tempNewProject);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? tempnewsDto : null;
        }
        public async Task<List<TempNewsVM>> GetByAction(string status)
        {
            var TempNews = await _unitOfWork.TempNewsRepo.GetByAction(status).ConfigureAwait(false);
            if (TempNews == null || TempNews.Count <= 0) return null;
            var TempNewsVms = _mapper.Map<List<TempNewsVM>>(TempNews);
            return TempNewsVms;
        }
    }
}
