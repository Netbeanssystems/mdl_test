using Application.Dtos;
using Application.ServiceInterfaces;
using AutoMapper;
using Domain.Models;
using Domain.RepositoryInterfaces;
using System.Threading.Tasks;

namespace Application.Services
{
    public class VigilanceService : IVigilanceFormService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public VigilanceService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<VigilanceFormDTO> Create(VigilanceFormDTO argModelDto)
        {
            if (argModelDto == null) return null;
            var model = _mapper.Map<VigilanceForm>(argModelDto);
            _unitOfWork.vigiRepo.Create(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            argModelDto.Id = model.Id;
            return rowsChanged > 0 ? argModelDto : null;
        }
    }
}
