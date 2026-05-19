using Application.Dtos;
using Application.ServiceInterfaces;
using Application.ViewModels;
using AutoMapper;
using Domain.Models;
using Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class DocumentsService : IDocumentsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public DocumentsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        //Common Methods
        public async Task<List<DocumentsVM>> Get()
        {
            var models = await _unitOfWork.DocumentsRepo.Get().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVms = _mapper.Map<List<DocumentsVM>>(models);
            if (modelVms == null || modelVms.Count <= 0) return null;
            return modelVms;
        }
        public async Task<DocumentsDTO> Get(int id)
        {
            var model = await _unitOfWork.DocumentsRepo.Get(id).ConfigureAwait(false);
            if (model == null) return null;
            var modelDto = _mapper.Map<DocumentsDTO>(model);
            return modelDto;
        }

        public async Task<List<DocumentsVM>> Getbycreatedby(string createdby)
        {
            var models = await _unitOfWork.DocumentsRepo.Getbycreatedby(createdby).ConfigureAwait(false);
            if (models == null) return null;
            var modelVms = _mapper.Map<List<DocumentsVM>>(models);
            if (modelVms == null || modelVms.Count <= 0) return null;
            return modelVms;
        }

        public async Task<List<DocumentsVM>> GetByYearAndDescription(int? year, int? urlsTimingId)
        {
            var models = await _unitOfWork.DocumentsRepo.GetByYearAndDescription(year, urlsTimingId).ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVms = _mapper.Map<List<DocumentsVM>>(models);
            if (modelVms == null || modelVms.Count <= 0) return null;
            return modelVms;
        }

        public async Task<List<DocumentsVM>> GetByURLsTimingAndDateRange(int? urlsTimingId, DateTime? fromDate, DateTime? toDate)
        {
            var models = await _unitOfWork.DocumentsRepo.GetByURLsTimingAndDateRange(urlsTimingId, fromDate, toDate).ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVms = _mapper.Map<List<DocumentsVM>>(models);
            if (modelVms == null || modelVms.Count <= 0) return null;
            return modelVms;
        }

        public async Task<List<DocumentsVM>> GetGroupedByURLsTiming()
        {
            var models = await _unitOfWork.DocumentsRepo.GetGroupedByURLsTiming().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVms = _mapper.Map<List<DocumentsVM>>(models);
            if (modelVms == null || modelVms.Count <= 0) return null;

            // Group by URLsTimingId and add count
            var grouped = modelVms.GroupBy(x => x.URLsTimingId)
                .SelectMany(g => g.Select(item =>
                {
                    item.DocumentCount = g.Count();
                    return item;
                })).ToList();

            return grouped;
        }

        public async Task<DocumentsDTO> Create(DocumentsDTO modelDto)
        {
            if (modelDto == null) return null;
            var model = _mapper.Map<Documents>(modelDto);
            _unitOfWork.DocumentsRepo.Create(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            modelDto = _mapper.Map<DocumentsDTO>(model);
            return rowsChanged > 0 ? modelDto : null;
        }
        public async Task<DocumentsDTO> Update(DocumentsDTO modelDto)
        {
            if (modelDto == null) return null;
            _unitOfWork.DocumentsRepo.Update(_mapper.Map<Documents>(modelDto));
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? modelDto : null;
        }
        public async Task<int> Delete(int id)
        {
            var model = await _unitOfWork.DocumentsRepo.Get(id).ConfigureAwait(false);
            if (model == null) return -1;
            _unitOfWork.DocumentsRepo.Delete(model);
            return await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }
        public async Task<List<DocumentsDTO>> CreateRange(List<DocumentsDTO> modelDtos)
        {
            if (modelDtos == null || modelDtos.Count <= 0) return null;
            var models = _mapper.Map<List<Documents>>(modelDtos);
            _unitOfWork.DocumentsRepo.CreateRange(models);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            modelDtos = _mapper.Map<List<DocumentsDTO>>(models);
            return rowsChanged > 0 ? modelDtos : null;
        }
        public async Task<List<DocumentsDTO>> Upsert(List<DocumentsDTO> modelDtos)
        {
            if (modelDtos == null || modelDtos.Count <= 0) return null;
            var models = _mapper.Map<List<Documents>>(modelDtos);
            foreach (var row in models)
            {
                if (_unitOfWork.DocumentsRepo.GetEntityState(row) != EntityState.Detached) continue;
                var ExistingRow = await _unitOfWork.DocumentsRepo.Get(row.Id).ConfigureAwait(false);
                if (ExistingRow != null)
                {
                    var attachedEntry = _unitOfWork.DocumentsRepo.GetEntityEntry(ExistingRow);
                    attachedEntry.CurrentValues.SetValues(row);
                }
                else
                {
                    _unitOfWork.DocumentsRepo.Create(row);
                }
            }
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            modelDtos = _mapper.Map<List<DocumentsDTO>>(models);
            return rowsChanged > 0 ? modelDtos : null;
        }
        public async Task<int> DeleteRange(List<DocumentsDTO> modelDtos)
        {
            if (modelDtos == null || modelDtos.Count <= 0) return -1;
            _unitOfWork.DocumentsRepo.DeleteRange(_mapper.Map<List<Documents>>(modelDtos));
            return await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }
        //Custom Methods
        public async Task<List<DropdownVM>> GetDropdown()
        {
            var models = await _unitOfWork.DocumentsRepo.GetActive().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVms = _mapper.Map<List<DropdownVM>>(models);
            if (modelVms == null || modelVms.Count <= 0) return null;
            return modelVms;
        }

        public async Task<URLsTimingDTO> CreateURLsTiming(URLsTimingDTO modelDto)
        {
            if (modelDto == null) return null;
            var model = _mapper.Map<URLsTiming>(modelDto);
            _unitOfWork.URLsTimingRepo.Create(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            modelDto = _mapper.Map<URLsTimingDTO>(model);
            return rowsChanged > 0 ? modelDto : null;
        }

        public async Task<List<URLsTimingVM>> GetURLsTiming()
        {
            var models = await _unitOfWork.URLsTimingRepo.Get().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVm = _mapper.Map<List<URLsTimingVM>>(models);
            if (modelVm == null) return null;
            return modelVm;
        }

        public async Task<List<URLsTimingVM>> GetActiveURLsTiming()
        {
            var models = await _unitOfWork.URLsTimingRepo.GetActive().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVm = _mapper.Map<List<URLsTimingVM>>(models);
            if (modelVm == null) return null;
            return modelVm;
        }

        public async Task<URLsTimingVM> GetCurrentActiveURLsTiming(string url)
        {
            var models = await _unitOfWork.URLsTimingRepo.GetActive().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            
            var model = models.Where(x => x.Url == url)
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();
            
            if (model == null) return null;
            
            var modelVm = _mapper.Map<URLsTimingVM>(model);
            
            // Check if current time is within the window
            var now = DateTime.Now;
            if (now >= model.FromTime && now <= model.ToTime)
            {
                return modelVm;
            }
            
            return null;
        }

        public async Task<List<int>> GetDocumentsYears()
        {
            var models = await _unitOfWork.DocumentsRepo.Get().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            
            var years = models.Select(x => x.CreatedDate.Year)
                .Distinct()
                .OrderByDescending(x => x)
                .ToList();
            
            return years;
        }

        public async Task<List<ClosedWindowsVM>> GetClosedWindows()
        {
            var result = await _unitOfWork.URLsTimingRepo.GetClosed().ConfigureAwait(false); 

            if (result == null || result.Count <= 0) return null;

            var modelVm =  _mapper.Map<List<ClosedWindowsVM>>(result);

            return modelVm;

        }

        public async Task<bool> UpdateMultipleWindowsVisibility(List<ClosedWindowsDTO> dtos)
        {
            // 1. Extract all IDs from the incoming DTO list
            var idsToUpdate = dtos.Select(d => d.Id).ToList();

            // 2. Fetch all matching records from the database in a single query
            var existingRecords = await _unitOfWork.URLsTimingRepo.GetByMultipleIds(idsToUpdate).ConfigureAwait(false);

            if (existingRecords == null || !existingRecords.Any())
            {
                return false; // No matching records found in the system
            }

            // 3. Loop through and map each DTO properties onto its corresponding tracked database entity
            foreach (var dto in dtos)
            {
                var recordToUpdate = existingRecords.FirstOrDefault(r => r.Id == dto.Id);
                if (recordToUpdate != null)
                {
                    // AutoMapper applies changes directly onto the EF-tracked entity
                    _mapper.Map(dto, recordToUpdate);

                    // Mark the entity as modified in the repository wrapper
                    await _unitOfWork.URLsTimingRepo.Update(recordToUpdate).ConfigureAwait(false);
                }
            }

            // 4. Commit all changes to the database in a single round-trip save operation
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

            return true;
        }


        //public async Task<List<DocumentsVM>> GetDocumentsIsShow()
        //{
        //    var documents = await _unitOfWork.DocumentsRepo.GetDocumentsIsShow().ConfigureAwait(false);

        //    if (documents == null || !documents.Any())
        //    {
        //        return new List<DocumentsVM>();
        //    }
        //    return  _mapper.Map<List<DocumentsVM>>(documents);

        //}

        //public async Task<List<DocumentsVM>> GetDocumentsIsShow()
        //{
        //    // 1. Fetch the URLsTiming list (which contains the nested Documents collections)
        //    var urlsTimings = await _unitOfWork.DocumentsRepo.GetDocumentsIsShow().ConfigureAwait(false);

        //    if (urlsTimings == null || !urlsTimings.Any())
        //    {
        //        return new List<DocumentsVM>();
        //    }

        //    // 2. Flatten the nested Documents collections into a single list of DocumentsVM
        //    var modelVm = urlsTimings
        //        .Where(ut => ut.Documents != null) // Ensure there are documents to map
        //        .SelectMany(ut => ut.Documents.Select(doc => new DocumentsVM
        //        {
        //            Id = doc.Id,
        //            DocumentName = doc.DocumentName,
        //       //     DocumentNameDecrypted = doc.DocumentNameDecrypted,
        //            BankName = doc.BankName,
        //            BranchName = doc.BranchName,
        //            Description = doc.Description,
        //            URLsTimingId = doc.URLsTimingId,
        //      //      DocumentCount = doc.DocumentCount,

        //            // Correctly reference the parent 'ut' (URLsTiming) object here
        //            URLsTiming = new URLsTimingVM
        //            {
        //                Id = ut.Id,
        //         //       WindowDescription = ut.WindowDescription, // Map other properties from parent as needed
        //                FromTime = ut.FromTime,
        //                ToTime = ut.ToTime
        //            }
        //        }))
        //        .ToList();

        //    return modelVm;
        //}
        public async Task<List<URLsTimingVM>> GetDocumentsIsShow()
        {
            var urlsTimings = await _unitOfWork.DocumentsRepo.GetDocumentsIsShow().ConfigureAwait(false);

            if (urlsTimings == null || !urlsTimings.Any())
            {
                return new List<URLsTimingVM>();
            }

            // Directly map URLsTiming entities to URLsTimingVM instances
            var modelVm = urlsTimings.Select(ut => new URLsTimingVM
            {
                Id = ut.Id,
                Url = ut.Url,
                FromTime = ut.FromTime,
                ToTime = ut.ToTime,
                Description = ut.Description,
                URLsTimingId = ut.Id.ToString() // Matching your virtual string ID requirement if needed
            }).ToList();

            return modelVm;
        }


        public async Task<List<DocumentsVM>> GetDocumentsList(int? urlsTimingId, DateTime? fromDate, DateTime? toDate)
        {
            var models = await _unitOfWork.DocumentsRepo.GetDocumentsList(urlsTimingId, fromDate, toDate).ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVms = _mapper.Map<List<DocumentsVM>>(models);
            if (modelVms == null || modelVms.Count <= 0) return null;
            return modelVms;
        }

        public async Task<bool> SaveDownloadLog(DocumentDownloadLog logEntity)
        {
            if (logEntity == null) return false;

            var result = await _unitOfWork.DocumentsRepo.SaveDownloadLog(logEntity);
            return result;

        }
    }
}
