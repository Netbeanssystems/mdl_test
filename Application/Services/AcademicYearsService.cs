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
    public class AcademicYearsService : IAcademicYearsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AcademicYearsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        //Common Methods
        public async Task<List<AcademicYearsVM>> Get()
        {
            var models = await _unitOfWork.AcademicYearsRepo.Get().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVms = _mapper.Map<List<AcademicYearsVM>>(models);
            if (modelVms == null || modelVms.Count <= 0) return null;
            return modelVms;
        }
        public async Task<AcademicYearsDTO> Get(int id)
        {
            var model = await _unitOfWork.AcademicYearsRepo.Get(id).ConfigureAwait(false);
            if (model == null) return null;
            var modelDto = _mapper.Map<AcademicYearsDTO>(model);
            return modelDto;
        }
        public async Task<bool> CheckDuplicate(AcademicYearsDTO argModelDto)
        {
            var model = _mapper.Map<AcademicYears>(argModelDto);
            var duplicate = await _unitOfWork.AcademicYearsRepo.CheckDuplicate(model).ConfigureAwait(false);
            return duplicate != null;
        }
        public async Task<AcademicYearsDTO> Create(AcademicYearsDTO argModelDto)
        {
            if (argModelDto == null) return null;
            var model = _mapper.Map<AcademicYears>(argModelDto);
            _unitOfWork.AcademicYearsRepo.Create(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? argModelDto : null;
        }
        public async Task<AcademicYearsDTO> Update(AcademicYearsDTO argModelDto)
        {
            if (argModelDto == null) return null;
            var model = _mapper.Map<AcademicYears>(argModelDto);
            _unitOfWork.AcademicYearsRepo.Update(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? argModelDto : null;
        }
        public async Task<int> Delete(int id)
        {
            var model = await _unitOfWork.AcademicYearsRepo.Get(id).ConfigureAwait(false);
            if (model == null) return -1;
            _unitOfWork.AcademicYearsRepo.Delete(model);
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
        public async Task<List<AcademicYearsVM>> GetWithAll()
        {
            var models = await _unitOfWork.AcademicYearsRepo.GetWithAll().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVms = _mapper.Map<List<AcademicYearsVM>>(models);
            return modelVms;
        }
        public async Task<List<DropdownVM>> GetDropdown()
        {
            var models = await _unitOfWork.AcademicYearsRepo.GetActive().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVms = _mapper.Map<List<DropdownVM>>(models);
            if (modelVms == null || modelVms.Count <= 0) return null;
            return modelVms;
        }
    }
}
