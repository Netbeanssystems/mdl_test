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
    public class TempWhatsNewService : ITempWhatsNewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TempWhatsNewService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<List<TempWhatsNewVM>> Get()
        {
            var tempNewProjects = await _unitOfWork.TempwhatsRepo.Get().ConfigureAwait(false);
            if (tempNewProjects == null || tempNewProjects.Count <= 0) return null;
            var tempNewProjectVms = _mapper.Map<List<TempWhatsNewVM>>(tempNewProjects);
            if (tempNewProjectVms == null || tempNewProjectVms.Count <= 0) return null;
            return tempNewProjectVms.OrderBy(x => x.Priority).ToList();
        }

        public async Task<TempWhatsNewDTO> Get(int id)
        {
            var tempNewProject = await _unitOfWork.TempwhatsRepo.Get(id).ConfigureAwait(false);
            if (tempNewProject == null) return null;
            var tempNewProjectDto = _mapper.Map<TempWhatsNewDTO>(tempNewProject);
            return tempNewProjectDto;
        }

        public async Task<TempWhatsNewDTO> Add(TempWhatsNewDTO tempnewsDto)
        {
            if (tempnewsDto == null) return null;
            var tempNewProject = _mapper.Map<TempWhatsNew>(tempnewsDto);
            _unitOfWork.TempwhatsRepo.Create(tempNewProject);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? tempnewsDto : null;
        }

        public async Task<TempWhatsNewDTO> Update(TempWhatsNewDTO tempnewsDto)
        {
            if (tempnewsDto == null) return null;
            var tempNewProject = _mapper.Map<TempWhatsNew>(tempnewsDto);
            _unitOfWork.TempwhatsRepo.Update(tempNewProject);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? tempnewsDto : null;
        }

        public async Task<int> Remove(int id)
        {
            var tempNewProject = await _unitOfWork.TempwhatsRepo.Get(id).ConfigureAwait(false);
            if (tempNewProject == null) return -1;
            _unitOfWork.TempwhatsRepo.Delete(tempNewProject);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? rowsChanged : -1;
        }

        public async Task<TempWhatsNewDTO> CreateAudit(TempWhatsNewDTO tempnewsDto)
        {
            if (tempnewsDto == null) return null;
            var tempNewProject = _mapper.Map<TempWhatsNew>(tempnewsDto);
            _unitOfWork.TempwhatsRepo.CreateAudit(tempNewProject);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? tempnewsDto : null;
        }
        public async Task<List<TempWhatsNewVM>> GetByAction(string status)
        {
            var TempNews = await _unitOfWork.TempwhatsRepo.GetByAction(status).ConfigureAwait(false);
            if (TempNews == null || TempNews.Count <= 0) return null;
            var TempNewsVms = _mapper.Map<List<TempWhatsNewVM>>(TempNews);
            return TempNewsVms;
        }
    }
}
