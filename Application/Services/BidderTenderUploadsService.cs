using System;
using Application.Dtos;
using Application.ServiceInterfaces;
using Application.ViewModels;
using AutoMapper;
using DocumentFormat.OpenXml.ExtendedProperties;
using Domain.Models;
using Domain.RepositoryInterfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class BidderTenderUploadsService : IBidderTenderUploadsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BidderTenderUploadsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<BidderTenderUploadsVM>> Get()
        {
            var categories = await _unitOfWork.BidderTenderUploadRepo.Get().ConfigureAwait(false);
            if (categories == null || categories.Count <= 0) return null;
            var categoryDtos = _mapper.Map<List<BidderTenderUploadsVM>>(categories);
            if (categoryDtos == null || categoryDtos.Count <= 0) return null;
            categoryDtos = categoryDtos.OrderByDescending(x => x.Id).ToList();
            foreach (var tender in categoryDtos)
            {
                var corrigendum = await _unitOfWork.BidderCorrigendumRepo.GetListAsync(c => c.TenderId == tender.Id.ToString() && c.IsActive).ConfigureAwait(false);
                var corrigendumDtos = _mapper.Map<List<BidderTenderCorrigendumVM>>(corrigendum);
                corrigendumDtos = corrigendumDtos.OrderByDescending(x => x.Id).ToList();
                tender.TenderCorrigendums = corrigendumDtos;

                var docs = await _unitOfWork.BidderTenderUploadDocumentsRepo.GetListAsync(d => d.TenderId == tender.Id && d.IsActive).ConfigureAwait(false);
                if (docs != null && docs.Any())
                {
                    tender.TenderDocuments = _mapper.Map<List<BidderTenderUploadDocumentsVM>>(docs);
                }
                tender.ForeignBidderId = tender.ForeignBidderId;


                var projectids = tender.ProjectId.Split(",").ToList();
                var projects = new List<string>();
                foreach (var project in projectids)
                {
                    var p = await _unitOfWork.ProjectRepo.Get(project.ToString()).ConfigureAwait(false);
                    projects.Add(p.ProjectName);
                }
                tender.ProjectName = string.Join(", ", projects);

                var yardids = tender.YardId.Split(",").ToList();
                var yardNumbers = new List<string>();
                foreach (var yardid in yardids)
                {
                    var yards = await _unitOfWork.YardRepo.Get(yardid).ConfigureAwait(false);
                    if (yards != null) yardNumbers.Add(yards.YardNumber);
                }
                tender.YardId = string.Join(", ", yardNumbers);
            }

                return categoryDtos;
        }
        public async Task<BidderTenderUploadsDTO> Add(BidderTenderUploadsDTO modelDto)
        {
            if (modelDto == null) return null;
            var heading = _mapper.Map<BidderTenderUploads>(modelDto);
            _unitOfWork.BidderTenderUploadRepo.Create(heading);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            modelDto.Id = heading.Id;

            // Save Tender Documents if present
            if (modelDto.TenderDocuments != null && modelDto.TenderDocuments.Any())
            {
                foreach (var doc in modelDto.TenderDocuments)
                {
                    var docEntity = _mapper.Map<BidderTenderUploadDocuments>(doc);
                    docEntity.TenderId = heading.Id;
                    docEntity.CreatedBy = modelDto.CreatedBy ?? "MDL";
                    docEntity.CreatedDate = DateTime.Now;
                    docEntity.IsActive = true;
                    _unitOfWork.BidderTenderUploadDocumentsRepo.Create(docEntity);
                }
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }

            return modelDto;
        }
        public async Task<BidderTenderUploadsDTO> Get(int id)
        {
            var category = await _unitOfWork.BidderTenderUploadRepo.Get(id).ConfigureAwait(false);
            if (category == null) return null;
            var categoryDto = _mapper.Map<BidderTenderUploadsDTO>(category);

            var docs = await _unitOfWork.BidderTenderUploadDocumentsRepo.GetListAsync(d => d.TenderId == id && d.IsActive).ConfigureAwait(false);
            if (docs != null && docs.Any())
            {
                categoryDto.TenderDocuments = _mapper.Map<List<BidderTenderUploadDocumentsDTO>>(docs);
            }

            var corrigendum = await _unitOfWork.BidderCorrigendumRepo.GetListAsync(c => c.TenderId == id.ToString() && c.IsActive).ConfigureAwait(false);
            if (corrigendum != null && corrigendum.Any())
            {
                var latestCor = corrigendum.OrderByDescending(x => x.Id).FirstOrDefault();
                categoryDto.TenderCorrigendums = _mapper.Map<BidderTenderCorrigendumDto>(latestCor);
            }

            return categoryDto;
        }
        public async Task<BidderTenderUploadsDTO> Update(BidderTenderUploadsDTO modelDto)
        {
            if (modelDto == null) return null;

            // Map and update main Tender entity
            var tenderEntity = _mapper.Map<BidderTenderUploads>(modelDto);
            _unitOfWork.BidderTenderUploadRepo.Update(tenderEntity);

            // Handle Tender Documents additions
            if (modelDto.TenderDocuments != null && modelDto.TenderDocuments.Any())
            {
                foreach (var doc in modelDto.TenderDocuments)
                {
                    if (doc.Id == 0)
                    {
                        var docEntity = _mapper.Map<BidderTenderUploadDocuments>(doc);
                        docEntity.TenderId = modelDto.Id;
                        docEntity.CreatedBy = modelDto.ModifiedBy ?? "MDL";
                        docEntity.CreatedDate = DateTime.Now;
                        docEntity.IsActive = true;
                        _unitOfWork.BidderTenderUploadDocumentsRepo.Create(docEntity);
                    }
                }
            }

            // Handle DeletedDocIds
            if (!string.IsNullOrEmpty(modelDto.DeletedDocIds))
            {
                var deleteIds = modelDto.DeletedDocIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => int.TryParse(id.Trim(), out var parsed) ? parsed : 0)
                    .Where(id => id > 0).ToList();

                foreach (var delId in deleteIds)
                {
                    var existingDoc = await _unitOfWork.BidderTenderUploadDocumentsRepo.Get(delId).ConfigureAwait(false);
                    if (existingDoc != null)
                    {
                        existingDoc.IsActive = false;
                        _unitOfWork.BidderTenderUploadDocumentsRepo.Update(existingDoc);
                    }
                }
            }

            // Handle DeletedCorrigendumDocIds (Soft Delete Corrigendums)
            if (!string.IsNullOrEmpty(modelDto.DeletedCorrigendumDocIds))
            {
                var deleteCorrigendumIds = modelDto.DeletedCorrigendumDocIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => int.TryParse(id.Trim(), out var parsed) ? parsed : 0)
                    .Where(id => id > 0).ToList();

                foreach (var delCorId in deleteCorrigendumIds)
                {
                    var existingCor = await _unitOfWork.BidderCorrigendumRepo.GetFirstOrDefaultAsync(c => c.Id == delCorId).ConfigureAwait(false);
                    if (existingCor != null)
                    {
                        existingCor.IsActive = false;
                        existingCor.ModifiedDate = DateTime.Now;
                        existingCor.ModifiedBy = !string.IsNullOrWhiteSpace(modelDto.ModifiedBy) ? modelDto.ModifiedBy : "MDL";
                        _unitOfWork.BidderCorrigendumRepo.Update(existingCor);
                    }
                }
            }

            // Update Corrigendum if present
            if (modelDto.TenderCorrigendums != null && (!string.IsNullOrEmpty(modelDto.TenderCorrigendums.CorrigendumDescription) || !string.IsNullOrEmpty(modelDto.TenderCorrigendums.CorrigendumDoc) || !string.IsNullOrEmpty(modelDto.TenderCorrigendums.HashedFileName)))
            {
                var existingCorrigendum = await _unitOfWork.BidderCorrigendumRepo
                    .GetFirstOrDefaultAsync(c => c.Id == modelDto.TenderCorrigendums.Id);

                if (existingCorrigendum != null)
                {
                    // Map updates
                    existingCorrigendum.CorrigendumDescription = modelDto.TenderCorrigendums.CorrigendumDescription;
                    existingCorrigendum.CorrigendumDoc = modelDto.TenderCorrigendums.CorrigendumDoc;
                    existingCorrigendum.OriginalFileName = modelDto.TenderCorrigendums.OriginalFileName;
                    existingCorrigendum.HashedFileName = modelDto.TenderCorrigendums.HashedFileName;
                    existingCorrigendum.FileSizeInBytes = modelDto.TenderCorrigendums.FileSizeInBytes;
                    existingCorrigendum.ExtendedDate = modelDto.TenderCorrigendums.ExtendedDate;
                    existingCorrigendum.ModifiedBy = !string.IsNullOrWhiteSpace(modelDto.TenderCorrigendums.ModifiedBy) ? modelDto.TenderCorrigendums.ModifiedBy : (!string.IsNullOrWhiteSpace(modelDto.ModifiedBy) ? modelDto.ModifiedBy : "MDL");
                    existingCorrigendum.ModifiedDate = DateTime.Now;

                    _unitOfWork.BidderCorrigendumRepo.Update(existingCorrigendum);
                }
                else
                {
                    modelDto.TenderCorrigendums.TenderId = modelDto.Id.ToString();
                    var newCorrigendum = _mapper.Map<BidderTenderCorrigendum>(modelDto.TenderCorrigendums);
                    newCorrigendum.CreatedBy = !string.IsNullOrWhiteSpace(modelDto.TenderCorrigendums.CreatedBy) ? modelDto.TenderCorrigendums.CreatedBy : (!string.IsNullOrWhiteSpace(modelDto.CreatedBy) ? modelDto.CreatedBy : "MDL");
                    newCorrigendum.CreatedDate = DateTime.Now;
                    newCorrigendum.IsActive = true;
                    _unitOfWork.BidderCorrigendumRepo.Create(newCorrigendum);
                }
            }

            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return modelDto;
        }

        public async Task<int> Remove(int id)
        {
            var category = await _unitOfWork.BidderTenderUploadRepo.Get(id).ConfigureAwait(false);
            if (category == null) return -1;
            _unitOfWork.BidderTenderUploadRepo.Delete(category);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? rowsChanged : -1;
        }

        public async Task<bool> DeleteCorrigendum(int id)
        {
            var item = await _unitOfWork.BidderCorrigendumRepo.GetFirstOrDefaultAsync(c => c.Id == id).ConfigureAwait(false);
            if (item == null) return false;
            item.IsActive = false;
            item.ModifiedDate = DateTime.Now;
            _unitOfWork.BidderCorrigendumRepo.Update(item);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }
        public async Task<BidderTenderUploadsVM> CheckTender(BidderTenderUploadsDTO modelDto)
        {
            if (modelDto == null) return null;
            var model = await _unitOfWork.BidderTenderUploadRepo.CheckTender(_mapper.Map<BidderTenderUploads>(modelDto));
            if (model == null) return null;
            if (model.TenderCorrigendums != null)
            {
                model.TenderCorrigendums = model.TenderCorrigendums.Where(c => c.IsActive).ToList();
            }
            return _mapper.Map<BidderTenderUploadsVM>(model);
        }
        public async Task<BidderTenderUploadsVM> GetByTenderNo(string tenderNo)
        {
            if (string.IsNullOrEmpty(tenderNo)) return null;
            var model = await _unitOfWork.BidderTenderUploadRepo.GetFirstOrDefaultAsync(c => c.TenderNo == tenderNo);
            if (model == null) return null;
            var tenderVM = _mapper.Map<BidderTenderUploadsVM>(model);
            if (tenderVM != null)
            {
                var corrigendum = await _unitOfWork.BidderCorrigendumRepo.GetListAsync(c => c.TenderId == model.Id.ToString() && c.IsActive).ConfigureAwait(false);
                if (corrigendum != null && corrigendum.Any())
                {
                    var corrigendumDtos = _mapper.Map<List<BidderTenderCorrigendumVM>>(corrigendum);
                    tenderVM.TenderCorrigendums = corrigendumDtos.OrderByDescending(x => x.Id).ToList();
                    var latest = tenderVM.TenderCorrigendums.Where(c => c.ExtendedDate.HasValue).Select(c => c.ExtendedDate.Value).DefaultIfEmpty().Max();
                    if (latest != default) tenderVM.LatestExtendedDate = latest;
                }
            }
            return tenderVM;
        }
    }
}