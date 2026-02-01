using Application.Dtos;
using Application.ServiceInterfaces;
using AutoMapper;
using Domain.Models;
using Domain.RepositoryInterfaces;
using System.Threading.Tasks;

namespace Application.Services
{
    public class GrievanceService : IGrievanceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GrievanceService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<GrievanceDTO> Create(GrievanceDTO argModelDto)
        {
            if (argModelDto == null) return null;
            var model = _mapper.Map<GrievanceForm>(argModelDto);
            _unitOfWork.grievanceRepo.Create(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            argModelDto.Id = model.Id;
            return rowsChanged > 0 ? argModelDto : null;
        }
    }
}
