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
    public class BidderGeneralDocService : IBidderGeneralDocService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public BidderGeneralDocService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        //Common Methods
        public async Task<List<BidderGeneralDocVM>> Get()
        {
            var models = await _unitOfWork.BidderGeneralDocRepo.Get().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVms = _mapper.Map<List<BidderGeneralDocVM>>(models);
            if (modelVms == null || modelVms.Count <= 0) return null;
            return modelVms;
        }
        public async Task<BidderGeneralDocDTO> Get(int id)
        {
            var model = await _unitOfWork.BidderGeneralDocRepo.Get(id).ConfigureAwait(false);
            if (model == null) return null;
            var modelDto = _mapper.Map<BidderGeneralDocDTO>(model);
            return modelDto;
        }
        //public async Task<bool> CheckDuplicate(BidderGeneralDocDTO argModelDto)
        //{
        //    var model = _mapper.Map<BidderGeneralDoc>(argModelDto);
        //    var duplicate = await _unitOfWork.BidderGeneralDocRepo.CheckDuplicate(model).ConfigureAwait(false);
        //    return duplicate != null;
        //}
        public async Task<BidderGeneralDocDTO> Create(BidderGeneralDocDTO argModelDto)
        {
            if (argModelDto == null) return null;
            var model = _mapper.Map<BidderGeneralDoc>(argModelDto);
            _unitOfWork.BidderGeneralDocRepo.Create(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? argModelDto : null;
        }
        public async Task<BidderGeneralDocDTO> Update(BidderGeneralDocDTO argModelDto)
        {
            if (argModelDto == null) return null;
            var model = _mapper.Map<BidderGeneralDoc>(argModelDto);
            _unitOfWork.BidderGeneralDocRepo.Update(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? argModelDto : null;
        }
        public async Task<int> Delete(int id)
        {
            var model = await _unitOfWork.BidderGeneralDocRepo.Get(id).ConfigureAwait(false);
            if (model == null) return -1;
            _unitOfWork.BidderGeneralDocRepo.Delete(model);
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
        //public async Task<List<BidderGeneralDocVM>> GetWithAll()
        //{
        //    var models = await _unitOfWork.BidderGeneralDocRepo.GetWithAll().ConfigureAwait(false);
        //    if (models == null || models.Count <= 0) return null;
        //    var modelVms = _mapper.Map<List<BidderGeneralDocVM>>(models);
        //    return modelVms;
        //}
        //public async Task<List<DropdownVM>> GetDropdown()
        //{
        //    var models = await _unitOfWork.BidderGeneralDocRepo.GetActive().ConfigureAwait(false);
        //    if (models == null || models.Count <= 0) return null;
        //    var modelVms = _mapper.Map<List<DropdownVM>>(models);
        //    if (modelVms == null || modelVms.Count <= 0) return null;
        //    return modelVms;
        //}
    }
}
