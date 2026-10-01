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
    public class NewsService : INewsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NewsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        //------------------------Old 
        public async Task<List<NewsVM>> Get()
        {
            var newProjects = await _unitOfWork.NewsRepo.Get().ConfigureAwait(false);
            if (newProjects == null || newProjects.Count <= 0) return null;
            var newProjectVms = _mapper.Map<List<NewsVM>>(newProjects);
            if (newProjectVms == null || newProjectVms.Count <= 0) return null;
            return newProjectVms;
        }

        //-------------------------Karn 11Dec 2023
        //public async Task<List<NewsVM>> GetAllNewsList()
        public async Task<List<NewsVMModal>> GetAllNewsList()
        {
            var newProjects = await _unitOfWork.NewsRepo.GetAllNewsList().ConfigureAwait(false);
            if (newProjects == null || newProjects.Count <= 0) return null;
            var newProjectVms = _mapper.Map<List<NewsVMModal>>(newProjects);
            //var newProjectVms = _mapper.Map<List<NewsVM>>(newProjects);
            if (newProjectVms == null || newProjectVms.Count <= 0) return null;
            return newProjectVms;
        }
        public async Task<List<NewsVM>> MostViewed()
        {
            var newProjects = await _unitOfWork.NewsRepo.MostViewed().ConfigureAwait(false);
            if (newProjects == null || newProjects.Count <= 0) return null;
            var newProjectVms = _mapper.Map<List<NewsVM>>(newProjects);
            if (newProjectVms == null || newProjectVms.Count <= 0) return null;
            return newProjectVms;
        }
        public async Task<List<NewsVM>> WhatsNew()
        {
            var newProjects = await _unitOfWork.NewsRepo.WhatsNew().ConfigureAwait(false);
            if (newProjects == null || newProjects.Count <= 0) return null;
            var newProjectVms = _mapper.Map<List<NewsVM>>(newProjects);
            if (newProjectVms == null || newProjectVms.Count <= 0) return null;
            return newProjectVms;
        }
        public async Task<List<NewsVM>> MinistryIndustryUpdates()
        {
            var newProjects = await _unitOfWork.NewsRepo.MinistryIndustryUpdates().ConfigureAwait(false);
            if (newProjects == null || newProjects.Count <= 0) return null;
            var newProjectVms = _mapper.Map<List<NewsVM>>(newProjects);
            if (newProjectVms == null || newProjectVms.Count <= 0) return null;
            return newProjectVms;
        }
        public async Task<List<NewsVM>> Getorderdesc()
        {
            var categories = await _unitOfWork.NewsRepo.Getorderdesc().ConfigureAwait(false);
            if (categories == null || categories.Count <= 0) return null;
            var categoryVms = _mapper.Map<List<NewsVM>>(categories);
            return categoryVms;
        }
        public async Task<NewsDTO> Add(NewsDTO NewsDto)
        {
            if (NewsDto == null) return null;
            var newMarquee = _mapper.Map<News>(NewsDto);
            _unitOfWork.NewsRepo.Create(newMarquee);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? NewsDto : null;
        }

        public async Task<List<NewsVM>> GetCategoriesWithAll()
        {
            var categories = await _unitOfWork.NewsRepo.GetCategoriesWithAll().ConfigureAwait(false);
            if (categories == null || categories.Count <= 0) return null;
            var categoryVms = _mapper.Map<List<NewsVM>>(categories);
            return categoryVms;
        }


        public async Task<NewsDTO> Get(int id)
        {
            var newProject = await _unitOfWork.NewsRepo.Get(id).ConfigureAwait(false);
            if (newProject == null) return null;
            var newProjectDto = _mapper.Map<NewsDTO>(newProject);
            return newProjectDto;
        }

        public async Task<NewsVM> Get(string Heading)
        {
            var category = await _unitOfWork.NewsRepo.GetByMenuHeading(Heading).ConfigureAwait(false);
            if (category == null) return null;
            var categoryDto = _mapper.Map<NewsVM>(category);
            return categoryDto;
        }

        public async Task<NewsDTO> Update(NewsDTO NewsDto)
        {
            if (NewsDto == null) return null;
            var newMarquee = _mapper.Map<News>(NewsDto);
            _unitOfWork.NewsRepo.Update(newMarquee);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? NewsDto : null;
        }

        public async Task<int> Remove(int id)
        {
            var newProject = await _unitOfWork.NewsRepo.Get(id).ConfigureAwait(false);
            if (newProject == null) return -1;
            _unitOfWork.NewsRepo.Delete(newProject);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? rowsChanged : -1;
        }
        public async Task<NewsDTO> EditMenuContent(NewsDTO NewsDto)
        {
            News menu = _mapper.Map<News>(NewsDto);
            _unitOfWork.NewsRepo.Update(menu);
            int Result = await _unitOfWork.SaveChangesAsync();
            if (Result > 0) return NewsDto;
            return null;
        }


        public async Task<List<NewsVM>> GetCategoriesWithAllTest(int id)
        {
            var categories = await _unitOfWork.NewsRepo.GetCategoriesWithAllTest(id).ConfigureAwait(false);
            if (categories == null || categories.Count <= 0) return null;
            var categoryVms = _mapper.Map<List<NewsVM>>(categories);
            return categoryVms;
        }

        public Task<int> UpdateMenus(List<NewsListPriorityDto> lstMenuPriourty)
        {
            List<NewsListPriorityModel> model = _mapper.Map<List<NewsListPriorityModel>>(lstMenuPriourty);
            _unitOfWork.NewsRepo.UpdateMenuPriority(model);
            return _unitOfWork.SaveChangesAsync();
        }

    }
}