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
    public class FeedbackService : IFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public FeedbackService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<FeedbackDTO> Create(FeedbackDTO argModelDto)
        {
            if (argModelDto == null) return null;
            var model = _mapper.Map<FeedbackForm>(argModelDto);
            _unitOfWork.feedbackRepo.Create(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            //argModelDto.Id = model.Id;
            return rowsChanged > 0 ? argModelDto : null;
        }

        public async Task<List<FeedbackVM>> Get()
        {
            var categories = await _unitOfWork.feedbackRepo.Get().ConfigureAwait(false);
            if (categories == null || categories.Count <= 0) return null;
            var categoryDtos = _mapper.Map<List<FeedbackVM>>(categories);
            if (categoryDtos == null || categoryDtos.Count <= 0) return null;
            return categoryDtos;
        }
    }
}
