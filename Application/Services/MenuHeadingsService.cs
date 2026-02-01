using Application.Dtos;
using Application.ServiceInterfaces;
using Application.ViewModels;
using AutoMapper;
using Domain.Models;
using Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public class MenuHeadingsService : IMenuHeadingsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MenuHeadingsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<List<MenuHeadingsCustomVM>> Get()
        {
            var categories = await _unitOfWork.MenuHeadingsRepo.GetCategoriesWithAll().ConfigureAwait(false);
            if (categories == null || categories.Count <= 0) return null;
            var categoryDtos = _mapper.Map<List<MenuHeadingsCustomVM>>(categories);
            if (categoryDtos == null || categoryDtos.Count <= 0) return null;
            return categoryDtos;
        }

        public async Task<DateTime?> GetLastDateTime()
        {
            var date = await _unitOfWork.MenuHeadingsRepo.GetLastDateTime().ConfigureAwait(false);
            //if (categories == null || categories.Count <= 0) return null;
            //var categoryDtos = _mapper.Map<List<MenuHeadingsVM>>(categories);
            //if (categoryDtos == null || categoryDtos.Count <= 0) return null;
            return date;
        }

        public async Task<MenuHeadingsDTO> Add(MenuHeadingsDTO modelDto)
        {
            if (modelDto == null) return null;
            var heading = _mapper.Map<MenuHeadings>(modelDto);
            _unitOfWork.MenuHeadingsRepo.Create(heading);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            modelDto.Id = heading.Id;
            return rowsChanged > 0 ? modelDto : null;
        }
        //Custom Methods
        public async Task<List<MenuHeadingsVM>> GetCategoriesWithAll()
        {
            var categories = await _unitOfWork.MenuHeadingsRepo.GetCategoriesWithAll().ConfigureAwait(false);
            if (categories == null || categories.Count <= 0) return null;
            var categoryVms = _mapper.Map<List<MenuHeadingsVM>>(categories);
            return categoryVms;
        }
        public async Task<MenuHeadingsDTO> Get(int id)
        {
            var category = await _unitOfWork.MenuHeadingsRepo.Get(id).ConfigureAwait(false);
            if (category == null) return null;
            var categoryDto = _mapper.Map<MenuHeadingsDTO>(category);
            return categoryDto;
        }

        public async Task<MenuHeadingsVM> Get(string Heading)
        {
            var category = await _unitOfWork.MenuHeadingsRepo.GetByMenuHeading(Heading).ConfigureAwait(false);
            if (category == null) return null;
            var categoryDto = _mapper.Map<MenuHeadingsVM>(category);
            return categoryDto;
        }
        public async Task<List<MenuHeadingsVM>> GetForMenu(string Heading)
        {
            var category = await _unitOfWork.MenuHeadingsRepo.GetForMenu(Heading).ConfigureAwait(false);
            if (category == null) return null;
            var categoryDto = _mapper.Map<List<MenuHeadingsVM>>(category);
            return categoryDto;
        }
        public async Task<MenuHeadingsDTO> Update(MenuHeadingsDTO modelDto)
        {
            if (modelDto == null) return null;
            var category = _mapper.Map<MenuHeadings>(modelDto);
            _unitOfWork.MenuHeadingsRepo.Update(category);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? modelDto : null;
        }
        public async Task<int> Remove(int id)
        {
            var category = await _unitOfWork.MenuHeadingsRepo.Get(id).ConfigureAwait(false);
            if (category == null) return -1;
            _unitOfWork.MenuHeadingsRepo.Delete(category);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? rowsChanged : -1;
        }
        public async Task<List<MenuHeadingsVM>> GetCategoriesWithAllTest(int id)
        {
            var categories = await _unitOfWork.MenuHeadingsRepo.GetCategoriesWithAllTest(id).ConfigureAwait(false);
            if (categories == null || categories.Count <= 0) return null;
            var categoryVms = _mapper.Map<List<MenuHeadingsVM>>(categories);
            return categoryVms;
        }
        public async Task<MenuHeadingsDTO> EditMenuContent(MenuHeadingsDTO modelDTO)
        {
            MenuHeadings menu = _mapper.Map<MenuHeadings>(modelDTO);
            _unitOfWork.MenuHeadingsRepo.Update(menu);
            int Result = await _unitOfWork.SaveChangesAsync();
            if (Result > 0) return modelDTO;
            return null;
        }
        public Task<int> UpdateMenus(List<MenuHeadingsListPriorityDto> lstMenuPriourty)
        {
            List<MenuHeadingsListPriority> model = _mapper.Map<List<MenuHeadingsListPriority>>(lstMenuPriourty);
            _unitOfWork.MenuHeadingsRepo.UpdateMenuPriority(model);
            return _unitOfWork.SaveChangesAsync();
        }
        public async Task<List<MenuHeadings>> GetSearch(string q)
        {
            var category = await _unitOfWork.MenuHeadingsRepo.GetSearch(q).ConfigureAwait(false);
            if (category == null) return null;
            List<MenuHeadings> lst = new List<MenuHeadings>();
            foreach (var cat in category)
            {
                if (cat.Children != null && cat.Children.Count != 0)
                {
                    lst.AddRange(cat.Children);
                    return lst;
                }
                else
                {
                    var categoryDto1 = _mapper.Map<List<MenuHeadings>>(category);
                    return categoryDto1;
                }
            }
            var categoryDto = _mapper.Map<List<MenuHeadings>>(category);
            return categoryDto;
        }

    }
}