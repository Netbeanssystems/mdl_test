using Application.Dtos;
using Application.ServiceInterfaces;
using Application.ViewModels;
using AutoMapper;
using Domain.Models;
using Domain.RepositoryInterfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Application.Services
{
    public class TempOtherLinkHeadingService : ITempOtherLinkHeadingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public TempOtherLinkHeadingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<TempOtherLinkHeadingVM>> Get()
        {
            var models = await _unitOfWork.TempOtherLinkHeadingRepo.Get().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var tempModelVms = _mapper.Map<List<TempOtherLinkHeadingVM>>(models);
            if (tempModelVms == null || tempModelVms.Count <= 0) return null;
            return tempModelVms.OrderBy(x => x.Priority).ToList();
        }

        public async Task<TempOtherLinkHeadingDTO> Get(int id)
        {
            var tempModels = await _unitOfWork.TempOtherLinkHeadingRepo.Get(id).ConfigureAwait(false);
            if (tempModels == null) return null;
            var tempModelsDto = _mapper.Map<TempOtherLinkHeadingDTO>(tempModels);
            return tempModelsDto;
        }

        public async Task<TempOtherLinkHeadingDTO> Add(TempOtherLinkHeadingDTO modelDto)
        {
            if (modelDto == null) return null;
            var tempModel = _mapper.Map<TempOtherLinkHeading>(modelDto);
            _unitOfWork.TempOtherLinkHeadingRepo.Create(tempModel);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? modelDto : null;
        }

        public async Task<TempOtherLinkHeadingDTO> Update(TempOtherLinkHeadingDTO modelDto)
        {
            if (modelDto == null) return null;
            var tempModel = _mapper.Map<TempOtherLinkHeading>(modelDto);
            _unitOfWork.TempOtherLinkHeadingRepo.Update(tempModel);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? modelDto : null;
        }

        public async Task<int> Remove(int id)
        {
            var tempModel = await _unitOfWork.TempOtherLinkHeadingRepo.Get(id).ConfigureAwait(false);
            if (tempModel == null) return -1;
            _unitOfWork.TempOtherLinkHeadingRepo.Delete(tempModel);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? rowsChanged : -1;
        }

        //Custom Methods
        public async Task<TempOtherLinkHeadingDTO> CreateAudit(TempOtherLinkHeadingDTO modelDto)
        {
            if (modelDto == null) return null;
            var tempModel = _mapper.Map<TempOtherLinkHeading>(modelDto);
            _unitOfWork.TempOtherLinkHeadingRepo.CreateAudit(tempModel);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? modelDto : null;
        }
        public async Task<List<TempOtherLinkHeadingVM>> GetByAction(string status)
        {
            var Tempproduct = await _unitOfWork.TempOtherLinkHeadingRepo.GetByAction(status).ConfigureAwait(false);
            if (Tempproduct == null || Tempproduct.Count <= 0) return null;
            var TempproductVms = _mapper.Map<List<TempOtherLinkHeadingVM>>(Tempproduct);
            return TempproductVms;
        }

    }
}
