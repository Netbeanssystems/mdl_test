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
    public class WhatsNewService : IWhatsNewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WhatsNewService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        ///---------------------Old
        public async Task<List<WhatsNewVM>> Get()
        {
            var newProjects = await _unitOfWork.whatsRepo.Get().ConfigureAwait(false);
            if (newProjects == null || newProjects.Count <= 0) return null;
            var newProjectVms = _mapper.Map<List<WhatsNewVM>>(newProjects);
            if (newProjectVms == null || newProjectVms.Count <= 0) return null;
            return newProjectVms;
        }

        //-------------------------Karn 11Dec 2023 
        public async Task<List<WhatsNewModal>> GetAllWhats()
        {
            var newProjects = await _unitOfWork.whatsRepo.GetAllWhatsNew().ConfigureAwait(false);
            if (newProjects == null || newProjects.Count <= 0) return null;
            var newProjectVms = _mapper.Map<List<WhatsNewModal>>(newProjects);
            if (newProjectVms == null || newProjectVms.Count <= 0) return null;
            return newProjectVms;
        }
        public async Task<List<WhatsNewVM>> MostViewed()
        {
            var newProjects = await _unitOfWork.whatsRepo.MostViewed().ConfigureAwait(false);
            if (newProjects == null || newProjects.Count <= 0) return null;
            var newProjectVms = _mapper.Map<List<WhatsNewVM>>(newProjects);
            if (newProjectVms == null || newProjectVms.Count <= 0) return null;
            return newProjectVms;
        }
        public async Task<List<WhatsNewVM>> WhatsNew()
        {
            var newProjects = await _unitOfWork.whatsRepo.WhatsNew().ConfigureAwait(false);
            if (newProjects == null || newProjects.Count <= 0) return null;
            var newProjectVms = _mapper.Map<List<WhatsNewVM>>(newProjects);
            if (newProjectVms == null || newProjectVms.Count <= 0) return null;
            return newProjectVms;
        }
        public async Task<List<WhatsNewVM>> MinistryIndustryUpdates()
        {
            var newProjects = await _unitOfWork.whatsRepo.MinistryIndustryUpdates().ConfigureAwait(false);
            if (newProjects == null || newProjects.Count <= 0) return null;
            var newProjectVms = _mapper.Map<List<WhatsNewVM>>(newProjects);
            if (newProjectVms == null || newProjectVms.Count <= 0) return null;
            return newProjectVms;
        }
        public async Task<List<WhatsNewVM>> Getorderdesc()
        {
            var categories = await _unitOfWork.whatsRepo.Getorderdesc().ConfigureAwait(false);
            if (categories == null || categories.Count <= 0) return null;
            var categoryVms = _mapper.Map<List<WhatsNewVM>>(categories);
            return categoryVms;
        }
        public async Task<WhatsNewDTO> Add(WhatsNewDTO NewsDto)
        {
            if (NewsDto == null) return null;
            var newMarquee = _mapper.Map<WhatsNew>(NewsDto);
            _unitOfWork.whatsRepo.Create(newMarquee);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? NewsDto : null;
        }

        public async Task<List<WhatsNewVM>> GetCategoriesWithAll()
        {
            var categories = await _unitOfWork.whatsRepo.GetCategoriesWithAll().ConfigureAwait(false);
            if (categories == null || categories.Count <= 0) return null;
            var categoryVms = _mapper.Map<List<WhatsNewVM>>(categories);
            return categoryVms;
        }


        public async Task<WhatsNewDTO> Get(int id)
        {
            var newProject = await _unitOfWork.whatsRepo.Get(id).ConfigureAwait(false);
            if (newProject == null) return null;
            var newProjectDto = _mapper.Map<WhatsNewDTO>(newProject);
            return newProjectDto;
        }

        public async Task<WhatsNewVM> Get(string Heading)
        {
            var category = await _unitOfWork.whatsRepo.GetByMenuHeading(Heading).ConfigureAwait(false);
            if (category == null) return null;
            var categoryDto = _mapper.Map<WhatsNewVM>(category);
            return categoryDto;
        }

        public async Task<WhatsNewDTO> Update(WhatsNewDTO NewsDto)
        {
            if (NewsDto == null) return null;
            var newMarquee = _mapper.Map<WhatsNew>(NewsDto);
            _unitOfWork.whatsRepo.Update(newMarquee);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? NewsDto : null;
        }

        public async Task<int> Remove(int id)
        {
            var newProject = await _unitOfWork.whatsRepo.Get(id).ConfigureAwait(false);
            if (newProject == null) return -1;
            _unitOfWork.whatsRepo.Delete(newProject);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? rowsChanged : -1;
        }

        public async Task<WhatsNewDTO> EditMenuContent(WhatsNewDTO NewsDto)
        {
            WhatsNew menu = _mapper.Map<WhatsNew>(NewsDto);
            _unitOfWork.whatsRepo.Update(menu);
            int Result = await _unitOfWork.SaveChangesAsync();
            if (Result > 0) return NewsDto;
            return null;
        }

        public async Task<List<WhatsNewVM>> GetCategoriesWithAllTest(int id)
        {
            var categories = await _unitOfWork.whatsRepo.GetCategoriesWithAllTest(id).ConfigureAwait(false);
            if (categories == null || categories.Count <= 0) return null;
            var categoryVms = _mapper.Map<List<WhatsNewVM>>(categories);
            return categoryVms;
        }

        public Task<int> UpdateMenus(List<NewsListPriorityDto> lstMenuPriourty)
        {
            List<WhatsNewListPriorityModel> model = _mapper.Map<List<WhatsNewListPriorityModel>>(lstMenuPriourty);
            _unitOfWork.whatsRepo.UpdateMenuPriority(model);
            return _unitOfWork.SaveChangesAsync();
        }
    }
}