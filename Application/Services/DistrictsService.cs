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
    public class DistrictsService : IDistrictsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public DistrictsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        //Common Methods
        public async Task<List<DistrictsVM>> Get()
        {
            var models = await _unitOfWork.DistrictsRepo.Get().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVms = _mapper.Map<List<DistrictsVM>>(models);
            if (modelVms == null || modelVms.Count <= 0) return null;
            return modelVms;
        }
        public async Task<DistrictsDTO> Get(int id)
        {
            var model = await _unitOfWork.DistrictsRepo.Get(id).ConfigureAwait(false);
            if (model == null) return null;
            var modelDto = _mapper.Map<DistrictsDTO>(model);
            return modelDto;
        }
        public async Task<DistrictsDTO> Create(DistrictsDTO argModelDto)
        {
            if (argModelDto == null) return null;
            var model = _mapper.Map<Districts>(argModelDto);
            _unitOfWork.DistrictsRepo.Create(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            argModelDto.Id = model.Id;
            return rowsChanged > 0 ? argModelDto : null;
        }
        public async Task<DistrictsDTO> Update(DistrictsDTO argModelDto)
        {
            if (argModelDto == null) return null;
            var model = _mapper.Map<Districts>(argModelDto);
            _unitOfWork.DistrictsRepo.Update(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? argModelDto : null;
        }
        public async Task<int> Delete(int id)
        {
            var model = await _unitOfWork.DistrictsRepo.Get(id).ConfigureAwait(false);
            if (model == null) return -1;
            _unitOfWork.DistrictsRepo.Delete(model);
            return await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            //var rowsChanged = -1;
            //try
            //{
            //    rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            //}
            //catch (Exception ex)
            //{
            //    if (ex.GetType() == typeof(DbUpdateException) || ex.GetType() == typeof(DbUpdateConcurrencyException))
            //        rowsChanged = -2;
            //}
            //return rowsChanged;
        }
        //Custom Methods
        public async Task<List<DropdownVM>> GetDropdown()
        {
            var models = await _unitOfWork.DistrictsRepo.GetActive().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVms = _mapper.Map<List<DropdownVM>>(models);
            if (modelVms == null || modelVms.Count <= 0) return null;
            return modelVms;
        }
        public async Task<List<DropdownVM>> GetDropdownByState(int stateid)
        {
            var models = await _unitOfWork.DistrictsRepo.GetDistrictsByState(stateid).ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVms = _mapper.Map<List<DropdownVM>>(models);
            if (modelVms == null || modelVms.Count <= 0) return null;
            return modelVms;
        }
        public async Task<string> GetDistrictCode(int id)
        {
            var District = await _unitOfWork.DistrictsRepo.Get(id).ConfigureAwait(false);
            return District.DistrictCode;
        }
    }
}