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
    public class BidderTenderDocumentsService : IBidderTenderDocumentsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BidderTenderDocumentsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<BidderTenderDocumentsVM>> Get()
        {
            var categories = await _unitOfWork.BidderTenderDocumentsRepo.Get().ConfigureAwait(false);
            if (categories == null || categories.Count <= 0) return null;
            var categoryDtos = _mapper.Map<List<BidderTenderDocumentsVM>>(categories);
            if (categoryDtos == null || categoryDtos.Count <= 0) return null;
            return categoryDtos;
        }
        public async Task<BidderTenderDocumentsDTO> Add(BidderTenderDocumentsDTO modelDto)
        {
            if (modelDto == null) return null;
            var heading = _mapper.Map<BidderTenderDocuments>(modelDto);
            _unitOfWork.BidderTenderDocumentsRepo.Create(heading);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            modelDto.Id = heading.Id;
            return rowsChanged > 0 ? modelDto : null;
        }
        public async Task<BidderTenderDocumentsDTO> Get(int id)
        {
            var category = await _unitOfWork.BidderTenderDocumentsRepo.Get(id).ConfigureAwait(false);
            if (category == null) return null;
            var categoryDto = _mapper.Map<BidderTenderDocumentsDTO>(category);
            return categoryDto;
        }
        public async Task<BidderTenderDocumentsDTO> Update(BidderTenderDocumentsDTO modelDto)
        {
            if (modelDto == null) return null;

            // Map and update main Tender entity
            var tenderEntity = _mapper.Map<BidderTenderDocuments>(modelDto);
            _unitOfWork.BidderTenderDocumentsRepo.Update(tenderEntity);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? modelDto : null;
        }

        public async Task<int> Remove(int id)
        {
            var category = await _unitOfWork.BidderTenderDocumentsRepo.Get(id).ConfigureAwait(false);
            if (category == null) return -1;
            _unitOfWork.BidderTenderDocumentsRepo.Delete(category);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? rowsChanged : -1;
        }
    }
}