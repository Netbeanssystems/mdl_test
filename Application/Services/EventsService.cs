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
    public class EventsService : IEventsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public EventsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        //Common Methods
        //public async Task<List<EventVM>> Get()
        //{
        //    var models = await _unitOfWork.eventsRepo.Get().ConfigureAwait(false);
        //    if (models == null || models.Count <= 0) return null;
        //    var modelVms = _mapper.Map<List<EventVM>>(models);
        //    if (modelVms == null || modelVms.Count <= 0) return null;
        //    return modelVms;
        //}
        public async Task<List<EventVM>> GetAllEvents()
        {
            var models = await _unitOfWork.eventsRepo.GetAllEvents().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVms = _mapper.Map<List<EventVM>>(models);
            if (modelVms == null || modelVms.Count <= 0) return null;
            return modelVms;
        }

        public async Task<EventDTO> Get(int id)
        {
            var model = await _unitOfWork.eventsRepo.Get(id).ConfigureAwait(false);
            if (model == null) return null;
            var modelDto = _mapper.Map<EventDTO>(model);
            return modelDto;
        }

        public async Task<EventDTO> Create(EventDTO modelDto)
        {
            if (modelDto == null) return null;
            var model = _mapper.Map<Events>(modelDto);
            _unitOfWork.eventsRepo.Create(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            modelDto = _mapper.Map<EventDTO>(model);
            return rowsChanged > 0 ? modelDto : null;
        }

        public async Task<EventDTO> Update(EventDTO modelDto)
        {
            if (modelDto == null) return null;
            _unitOfWork.eventsRepo.Update(_mapper.Map<Events>(modelDto));
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? modelDto : null;
        }

        public async Task<int> Delete(int id)
        {
            var model = await _unitOfWork.eventsRepo.Get(id).ConfigureAwait(false);
            if (model == null) return -1;
            _unitOfWork.eventsRepo.Delete(model);
            return await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }

        //Custom Methods
        public async Task<List<DropdownVM>> GetDropdown()
        {
            var models = await _unitOfWork.eventsRepo.GetActive().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVms = _mapper.Map<List<DropdownVM>>(models);
            if (modelVms == null || modelVms.Count <= 0) return null;
            return modelVms;
        }
        public async Task<List<DropdownVM>> GetDropdownById(int id)
        {
            var models = await _unitOfWork.eventsRepo.GetDropdownById(id).ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVms = _mapper.Map<List<DropdownVM>>(models);
            if (modelVms == null || modelVms.Count <= 0) return null;
            return modelVms;
        }
    }
}
