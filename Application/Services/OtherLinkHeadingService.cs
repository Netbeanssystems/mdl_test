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
    public class OtherLinkHeadingService : IOtherLinkHeadingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OtherLinkHeadingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<List<OtherLinkHeadingDTO>> Get()
        {
            var categories = await _unitOfWork.OtherLinkHeadingRepo.Get().ConfigureAwait(false);
            if (categories == null || categories.Count <= 0) return null;
            var categoryDtos = _mapper.Map<List<OtherLinkHeadingDTO>>(categories);
            if (categoryDtos == null || categoryDtos.Count <= 0) return null;
            return categoryDtos;
        }


        public async Task<OtherLinkHeadingDTO> Add(OtherLinkHeadingDTO modelHeadingDto)
        {
            if (modelHeadingDto == null) return null;
            var heading = _mapper.Map<OtherLinkHeading>(modelHeadingDto);
            _unitOfWork.OtherLinkHeadingRepo.Create(heading);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            modelHeadingDto.Id = heading.Id;
            return rowsChanged > 0 ? modelHeadingDto : null;
        }
        //Custom Methods

        public async Task<List<OtherLinkHeadingVM>> GetCategoriesWithAll()
        {
            var categories = await _unitOfWork.OtherLinkHeadingRepo.GetCategoriesWithAll().ConfigureAwait(false);
            if (categories == null || categories.Count <= 0) return null;
            var categoryVms = _mapper.Map<List<OtherLinkHeadingVM>>(categories);
            return categoryVms;
        }

        public async Task<OtherLinkHeadingDTO> Get(int id)
        {
            var category = await _unitOfWork.OtherLinkHeadingRepo.Get(id).ConfigureAwait(false);
            if (category == null) return null;
            var categoryDto = _mapper.Map<OtherLinkHeadingDTO>(category);
            return categoryDto;
        }

        public async Task<OtherLinkHeadingVM> Get(string Heading)
        {
            var category = await _unitOfWork.OtherLinkHeadingRepo.GetByMenuHeading(Heading).ConfigureAwait(false);
            if (category == null) return null;
            var categoryDto = _mapper.Map<OtherLinkHeadingVM>(category);
            return categoryDto;
        }

        public async Task<OtherLinkHeadingDTO> Update(OtherLinkHeadingDTO modelHeadingDto)
        {
            if (modelHeadingDto == null) return null;
            var category = _mapper.Map<OtherLinkHeading>(modelHeadingDto);
            _unitOfWork.OtherLinkHeadingRepo.Update(category);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? modelHeadingDto : null;
        }

        public async Task<int> Remove(int id)
        {
            var category = await _unitOfWork.OtherLinkHeadingRepo.Get(id).ConfigureAwait(false);
            if (category == null) return -1;
            _unitOfWork.OtherLinkHeadingRepo.Delete(category);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? rowsChanged : -1;
        }

        //..............................Test..................

        public async Task<List<OtherLinkHeadingVM>> GetCategoriesWithAllTest(int id)
        {
            var categories = await _unitOfWork.OtherLinkHeadingRepo.GetCategoriesWithAllTest(id).ConfigureAwait(false);
            if (categories == null || categories.Count <= 0) return null;
            var categoryVms = _mapper.Map<List<OtherLinkHeadingVM>>(categories);
            return categoryVms;
        }
        //--------------------------------

        public async Task<OtherLinkHeadingDTO> EditMenuContent(OtherLinkHeadingDTO modelHeadingDto)
        {
            OtherLinkHeading menu = _mapper.Map<OtherLinkHeading>(modelHeadingDto);
            _unitOfWork.OtherLinkHeadingRepo.Update(menu);
            int Result = await _unitOfWork.SaveChangesAsync();
            if (Result > 0) return modelHeadingDto;
            return null;
        }

        public Task<int> UpdateMenus(List<OtherLinkListPriorityDTO> lstMenuPriourty)
        {
            List<OtherLinkListPriority> model = _mapper.Map<List<OtherLinkListPriority>>(lstMenuPriourty);
            _unitOfWork.OtherLinkHeadingRepo.UpdateMenuPriority(model);
            return _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<OtherLinkHeadingDTO>> GetStockData(string Heading)
        {
            var Result = await _unitOfWork.OtherLinkHeadingRepo.GetStockData(Heading).ConfigureAwait(false);
            if (Result == null || Result.Count <= 0) return null;
            var CPIOvm = _mapper.Map<List<OtherLinkHeadingDTO>>(Result);
            return CPIOvm;
        }
        public async Task<List<OtherLinkHeadingDTO>> GetStockDataHindi(string Heading)
        {
            var Result = await _unitOfWork.OtherLinkHeadingRepo.GetStockDataHindi(Heading).ConfigureAwait(false);
            if (Result == null || Result.Count <= 0) return null;
            var CPIOvm = _mapper.Map<List<OtherLinkHeadingDTO>>(Result);
            return CPIOvm;
        }
        public async Task<List<OtherLinkHeadingDTO>> GetStockExchange()
        {
            var Result = await _unitOfWork.OtherLinkHeadingRepo.GetStockExchange().ConfigureAwait(false);
            if (Result == null || Result.Count <= 0) return null;
            var CPIOvm = _mapper.Map<List<OtherLinkHeadingDTO>>(Result);
            return CPIOvm;
        }

        public async Task<List<OtherLinkHeadingDTO>> GetcorporateExchange()
        {
            var Result = await _unitOfWork.OtherLinkHeadingRepo.GetcorporateExchange().ConfigureAwait(false);
            if (Result == null || Result.Count <= 0) return null;
            var CPIOvm = _mapper.Map<List<OtherLinkHeadingDTO>>(Result);
            return CPIOvm;
        }

        public async Task<List<OtherLinkHeadingDTO>> GetCommitment()
        {
            var Result = await _unitOfWork.OtherLinkHeadingRepo.GetCommitment().ConfigureAwait(false);
            if (Result == null || Result.Count <= 0) return null;
            var CPIOvm = _mapper.Map<List<OtherLinkHeadingDTO>>(Result);
            return CPIOvm;
        }

        //--- [in-use]
        public async Task<List<OtherLinkHeadingDTO>> GetServiceCOCOs()
        {
            var Result = await _unitOfWork.OtherLinkHeadingRepo.GetServiceCOCOs().ConfigureAwait(false);
            if (Result == null || Result.Count <= 0) return null;
            var CPIOvm = _mapper.Map<List<OtherLinkHeadingDTO>>(Result);
            return CPIOvm;
        }

        public async Task<List<OtherLinkHeadingDTO>> GetAnalystInstitutionalInvestors()
        {
            var Result = await _unitOfWork.OtherLinkHeadingRepo.GetAnalystInstitutionalInvestors().ConfigureAwait(false);
            if (Result == null || Result.Count <= 0) return null;
            var CPIOvm = _mapper.Map<List<OtherLinkHeadingDTO>>(Result);
            return CPIOvm;
        }

        public async Task<List<OtherLinkHeadingDTO>> GetHeading(string Type)
        {
            var Result = await _unitOfWork.OtherLinkHeadingRepo.GetHeading(Type).ConfigureAwait(false);
            if (Result == null || Result.Count <= 0) return null;
            var CPIOvm = _mapper.Map<List<OtherLinkHeadingDTO>>(Result);
            return CPIOvm;
        }
        public async Task<List<OtherLinkHeadingDTO>> GetHeadingForshareholding(string Type)
        {
            var Result = await _unitOfWork.OtherLinkHeadingRepo.GetHeadingForshareholding(Type).ConfigureAwait(false);
            if (Result == null || Result.Count <= 0) return null;
            var CPIOvm = _mapper.Map<List<OtherLinkHeadingDTO>>(Result);
            return CPIOvm;
        }
        public async Task<List<OtherLinkHeading>> GetSearchfromotherlinks(string q)
        {
            var category = await _unitOfWork.OtherLinkHeadingRepo.GetSearchfromotherlinks(q).ConfigureAwait(false);
            if (category == null) return null;
            List<OtherLinkHeading> lst = new List<OtherLinkHeading>();
            foreach (var cat in category)
            {
                if (cat.Children != null && cat.Children.Count != 0)
                {
                    lst.AddRange(cat.Children);
                    return lst;
                }
                else
                {
                    var categoryDto1 = _mapper.Map<List<OtherLinkHeading>>(category);
                    return categoryDto1;
                }
            }
            var categoryDto = _mapper.Map<List<OtherLinkHeading>>(category);
            return categoryDto;
        }
        public async Task<OtherLinkHeadingVM> GetcmsforCorporateGovernance()
        {
            var category = await _unitOfWork.OtherLinkHeadingRepo.GetcmsforCorporateGovernance().ConfigureAwait(false);
            if (category == null) return null;
            var categoryDto = _mapper.Map<OtherLinkHeadingVM>(category);
            return categoryDto;
        }
        public async Task<OtherLinkHeadingDTO> Getlastrecord()
        {
            var category = await _unitOfWork.OtherLinkHeadingRepo.Getlastrecord().ConfigureAwait(false);
            if (category == null) return null;
            var categoryDto = _mapper.Map<OtherLinkHeadingDTO>(category);
            return categoryDto;
        }
        public async Task<OtherLinkHeadingDTO> Getlastrecordshareprice()
        {
            var category = await _unitOfWork.OtherLinkHeadingRepo.Getlastrecordshareprice().ConfigureAwait(false);
            if (category == null) return null;
            var categoryDto = _mapper.Map<OtherLinkHeadingDTO>(category);
            return categoryDto;
        }
        public async Task<OtherLinkHeadingDTO> Getlastrecordanalystsinstitutional()
        {
            var category = await _unitOfWork.OtherLinkHeadingRepo.Getlastrecordanalystsinstitutional().ConfigureAwait(false);
            if (category == null) return null;
            var categoryDto = _mapper.Map<OtherLinkHeadingDTO>(category);
            return categoryDto;
        }
        public async Task<OtherLinkHeadingDTO> Getlastrecordstockexchange()
        {
            var category = await _unitOfWork.OtherLinkHeadingRepo.Getlastrecordstockexchange().ConfigureAwait(false);
            if (category == null) return null;
            var categoryDto = _mapper.Map<OtherLinkHeadingDTO>(category);
            return categoryDto;
        }
    }
}
