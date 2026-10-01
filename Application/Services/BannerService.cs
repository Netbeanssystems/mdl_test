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
    public class BannerService : IBannerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BannerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        //------------------------------- Old Code ---------------------
        public async Task<List<BannerVM>> Get()
        {
            var categories = await _unitOfWork.BannerRepo.Get().ConfigureAwait(false);
            if (categories == null || categories.Count <= 0) return null;
            var categoryDtos = _mapper.Map<List<BannerVM>>(categories);
            if (categoryDtos == null || categoryDtos.Count <= 0) return null;
            return categoryDtos;
        }
        //-----------------------------------New Karn Code 8 Dec 2023---------------
        public async Task<List<BannerVMModal>> GetAllBanner()
        //public async Task<List<BannerVM>> GetAllBanner()
        {
            var categories = await _unitOfWork.BannerRepo.GetAllBanner().ConfigureAwait(false);
            if (categories == null || categories.Count <= 0) return null;
            var categoryDtos = _mapper.Map<List<BannerVMModal>>(categories);
            if (categoryDtos == null || categoryDtos.Count <= 0) return null;
            return categoryDtos;
        }
        public async Task<BannerDTO> Add(BannerDTO modelDto)
        {
            if (modelDto == null) return null;
            var heading = _mapper.Map<Banners>(modelDto);
            _unitOfWork.BannerRepo.Create(heading);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            modelDto.Id = heading.Id;
            return rowsChanged > 0 ? modelDto : null;
        }
        //Custom Methods
        public async Task<List<BannerVM>> GetCategoriesWithAll()
        {
            var categories = await _unitOfWork.BannerRepo.GetCategoriesWithAll().ConfigureAwait(false);
            if (categories == null || categories.Count <= 0) return null;
            var categoryVms = _mapper.Map<List<BannerVM>>(categories);
            return categoryVms;
        }
        public async Task<BannerDTO> Get(int id)
        {
            var category = await _unitOfWork.BannerRepo.Get(id).ConfigureAwait(false);
            if (category == null) return null;
            var categoryDto = _mapper.Map<BannerDTO>(category);
            return categoryDto;
        }
        public async Task<BannerDTO> Update(BannerDTO modelDto)
        {
            if (modelDto == null) return null;
            var category = _mapper.Map<Banners>(modelDto);
            _unitOfWork.BannerRepo.Update(category);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? modelDto : null;
        }
        public async Task<int> Remove(int id)
        {
            var category = await _unitOfWork.BannerRepo.Get(id).ConfigureAwait(false);
            if (category == null) return -1;
            _unitOfWork.BannerRepo.Delete(category);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? rowsChanged : -1;
        }

        //..............................Test..................
        public async Task<List<BannerVM>> GetCategoriesWithAllTest(int id)
        {
            var categories = await _unitOfWork.BannerRepo.GetCategoriesWithAllTest(id).ConfigureAwait(false);
            if (categories == null || categories.Count <= 0) return null;
            var categoryVms = _mapper.Map<List<BannerVM>>(categories);
            return categoryVms;
        }
        //-----------------Naman Start Work----------------
        public async Task<BannerDTO> EditMenuContent(BannerDTO modelDTO)
        {
            Banners menu = _mapper.Map<Banners>(modelDTO);
            _unitOfWork.BannerRepo.Update(menu);
            int Result = await _unitOfWork.SaveChangesAsync();
            if (Result > 0) return modelDTO;
            return null;
        }
        public Task<int> UpdateMenus(List<MenuHeadingListPriorityDto> lstMenuPriourty)
        {
            List<MenuHeadingListPriority> model = _mapper.Map<List<MenuHeadingListPriority>>(lstMenuPriourty);
            _unitOfWork.BannerRepo.UpdateMenuPriority(model);
            return _unitOfWork.SaveChangesAsync();
        }
    }
}