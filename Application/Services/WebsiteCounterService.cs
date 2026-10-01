using Application.Dtos;
using Application.ServiceInterfaces;
using AutoMapper;
using Domain.Models;
using Domain.RepositoryInterfaces;
using System.Threading.Tasks;

namespace Application.Services
{
    public class WebsiteCounterService : IWebsiteCounterService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public WebsiteCounterService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<WebsiteCounterDTO> Get(int id)
        {
            var model = await _unitOfWork.counterRepo.Get(id).ConfigureAwait(false);
            if (model == null) return null;
            var modelDto = _mapper.Map<WebsiteCounterDTO>(model);
            return modelDto;
        }

        public async Task<WebsiteCounterDTO> Update(WebsiteCounterDTO modelDto)
        {
            if (modelDto == null) return null;
            _unitOfWork.counterRepo.Update(_mapper.Map<WebSiteCounters>(modelDto));
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? modelDto : null;
        }
    }
}
