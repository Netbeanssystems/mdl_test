using Application.Dtos;
using Application.ServiceInterfaces;
using AutoMapper;
using Domain.Models;
using Domain.RepositoryInterfaces;
using System.Threading.Tasks;

namespace Application.Services
{
    public class EnquiryService : IEnquiryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public EnquiryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<EnquiryDTO> Create(EnquiryDTO argModelDto)
        {
            if (argModelDto == null) return null;
            var model = _mapper.Map<EnquiryForm>(argModelDto);
            _unitOfWork.enquiryRepo.Create(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            argModelDto.Id = model.Id;
            return rowsChanged > 0 ? argModelDto : null;
        }
    }
}
