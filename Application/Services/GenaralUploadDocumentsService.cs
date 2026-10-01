using Application.Dtos;
using Application.ServiceInterfaces;
using Application.ViewModels;
using AutoMapper;
using Domain.Models;
using Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Application.Services
{
    public class GenaralUploadDocumentsService : IGenaralUploadDocumentsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GenaralUploadDocumentsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        //Common Methods
        public async Task<List<GenaralUploadDocumentsVM>> Get()
        {
            var models = await _unitOfWork.GenaralUploadDocumentsRepo.Get().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVms = _mapper.Map<List<GenaralUploadDocumentsVM>>(models);
            if (modelVms == null || modelVms.Count <= 0) return null;
            return modelVms;
        }
        public async Task<GenaralUploadDocumentsDTO> Get(int id)
        {
            var model = await _unitOfWork.GenaralUploadDocumentsRepo.Get(id).ConfigureAwait(false);
            if (model == null) return null;
            var modelDto = _mapper.Map<GenaralUploadDocumentsDTO>(model);
            return modelDto;
        }

        public async Task<List<GenaralUploadDocumentsVM>> Getbycreatedby(string createdby)
        {

            var models = await _unitOfWork.GenaralUploadDocumentsRepo.Getbycreatedby(createdby).ConfigureAwait(false);
            if (models == null) return null;
            var modelVms = _mapper.Map<List<GenaralUploadDocumentsVM>>(models);
            if (modelVms == null || modelVms.Count <= 0) return null;
            return modelVms;
        }
        public async Task<GenaralUploadDocumentsDTO> Create(GenaralUploadDocumentsDTO modelDto)
        {
            if (modelDto == null) return null;
            var model = _mapper.Map<GenaralUploadDocuments>(modelDto);
            _unitOfWork.GenaralUploadDocumentsRepo.Create(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            modelDto = _mapper.Map<GenaralUploadDocumentsDTO>(model);
            return rowsChanged > 0 ? modelDto : null;
        }
        public async Task<GenaralUploadDocumentsDTO> Update(GenaralUploadDocumentsDTO modelDto)
        {
            if (modelDto == null) return null;
            _unitOfWork.GenaralUploadDocumentsRepo.Update(_mapper.Map<GenaralUploadDocuments>(modelDto));
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? modelDto : null;
        }
        public async Task<int> Delete(int id)
        {
            var model = await _unitOfWork.GenaralUploadDocumentsRepo.Get(id).ConfigureAwait(false);
            if (model == null) return -1;
            _unitOfWork.GenaralUploadDocumentsRepo.Delete(model);
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
        public async Task<List<GenaralUploadDocumentsDTO>> CreateRange(List<GenaralUploadDocumentsDTO> modelDtos)
        {
            if (modelDtos == null || modelDtos.Count <= 0) return null;
            var models = _mapper.Map<List<GenaralUploadDocuments>>(modelDtos);
            _unitOfWork.GenaralUploadDocumentsRepo.CreateRange(models);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            modelDtos = _mapper.Map<List<GenaralUploadDocumentsDTO>>(models);
            return rowsChanged > 0 ? modelDtos : null;
        }
        public async Task<List<GenaralUploadDocumentsDTO>> Upsert(List<GenaralUploadDocumentsDTO> modelDtos)
        {
            if (modelDtos == null || modelDtos.Count <= 0) return null;
            var models = _mapper.Map<List<GenaralUploadDocuments>>(modelDtos);
            foreach (var row in models)
            {
                if (_unitOfWork.GenaralUploadDocumentsRepo.GetEntityState(row) != EntityState.Detached) continue;
                var ExistingRow = await _unitOfWork.GenaralUploadDocumentsRepo.Get(row.Id).ConfigureAwait(false);
                if (ExistingRow != null)
                {
                    var attachedEntry = _unitOfWork.GenaralUploadDocumentsRepo.GetEntityEntry(ExistingRow);
                    attachedEntry.CurrentValues.SetValues(row);
                }
                else
                {
                    _unitOfWork.GenaralUploadDocumentsRepo.Create(row);
                }
            }
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            modelDtos = _mapper.Map<List<GenaralUploadDocumentsDTO>>(models);
            return rowsChanged > 0 ? modelDtos : null;
        }
        public async Task<int> DeleteRange(List<GenaralUploadDocumentsDTO> modelDtos)
        {
            if (modelDtos == null || modelDtos.Count <= 0) return -1;
            _unitOfWork.GenaralUploadDocumentsRepo.DeleteRange(_mapper.Map<List<GenaralUploadDocuments>>(modelDtos));
            return await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }
        //Custom Methods
        public async Task<List<DropdownVM>> GetDropdown()
        {
            var models = await _unitOfWork.GenaralUploadDocumentsRepo.GetActive().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVms = _mapper.Map<List<DropdownVM>>(models);
            if (modelVms == null || modelVms.Count <= 0) return null;
            ////Move India to first position
            //var index = modelVms.FindIndex(x => x.Text == "India");
            //var item = modelVms[index];
            //modelVms[index] = modelVms[0];
            //modelVms[0] = item;
            return modelVms;
        }

        public async Task<GeneraluploadURLDTO> CreateURLsTiming(GeneraluploadURLDTO modelDto)
        {
            if (modelDto == null) return null;
            var model = _mapper.Map<GeneraluploadURL>(modelDto);
            _unitOfWork.GeneraluploadURLRepo.Create(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            modelDto = _mapper.Map<GeneraluploadURLDTO>(model);
            return rowsChanged > 0 ? modelDto : null;
        }
        public async Task<GeneraluploadURLDTO> CreateGeneralURL(GeneraluploadURLDTO modelDto)
        {
            if (modelDto == null) return null;
            var model = _mapper.Map<GeneraluploadURL>(modelDto);
            _unitOfWork.GeneraluploadURLRepo.Create(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            modelDto = _mapper.Map<GeneraluploadURLDTO>(model);
            return rowsChanged > 0 ? modelDto : null;
        }

        public async Task<List<GeneraluploadURLDTO>> GetURLsTiming()
        {
            var models = await _unitOfWork.GeneraluploadURLRepo.Get().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            //var model = models.Where(x => x.Url == URL).OrderByDescending(x => x.Id).Take(1).FirstOrDefault();
            var modelVm = _mapper.Map<List<GeneraluploadURLDTO>>(models);
            if (modelVm == null) return null;
            return modelVm;
        }
    }
}
