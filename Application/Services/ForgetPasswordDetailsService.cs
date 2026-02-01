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
    public class ForgetPasswordDetailsService : IForgetPasswordDetailsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ForgetPasswordDetailsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        //Common Methods
        public async Task<List<ForgetPasswordDetailsVM>> Get()
        {
            var models = await _unitOfWork.ForgetPasswordDetailsRepo.Get().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVms = _mapper.Map<List<ForgetPasswordDetailsVM>>(models);
            if (modelVms == null || modelVms.Count <= 0) return null;
            return modelVms;
        }

        public async Task<ForgetPasswordDetailsDTO> Get(string id)
        {
            var model = await _unitOfWork.ForgetPasswordDetailsRepo.Get(id).ConfigureAwait(false);
            if (model == null) return null;
            var modelDto = _mapper.Map<ForgetPasswordDetailsDTO>(model);
            return modelDto;
        }

        public async Task<ForgetPasswordDetailsDTO> Create(ForgetPasswordDetailsDTO argModelDto)
        {
            if (argModelDto == null) return null;
            var model = _mapper.Map<ForgetPasswordDetails>(argModelDto);
            _unitOfWork.ForgetPasswordDetailsRepo.Create(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            argModelDto.Id = model.Id;
            return rowsChanged > 0 ? argModelDto : null;
        }
        public async Task<ForgetPasswordDetailsDTO> Update(ForgetPasswordDetailsDTO argModelDto)
        {
            if (argModelDto == null) return null;
            var model = _mapper.Map<ForgetPasswordDetails>(argModelDto);
            _unitOfWork.ForgetPasswordDetailsRepo.Update(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            argModelDto.Id = model.Id;
            return rowsChanged > 0 ? argModelDto : null;
        }

        public async Task<int> Delete(string id)
        {
            var model = await _unitOfWork.ForgetPasswordDetailsRepo.Get(id).ConfigureAwait(false);
            if (model == null) return -1;
            _unitOfWork.ForgetPasswordDetailsRepo.Delete(model);
            return await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<List<ForgetPasswordDetailsDTO>> CheckUserDetails(string userid, string date)
        {
            var category = await _unitOfWork.ForgetPasswordDetailsRepo.CheckUserDetails(userid, date).ConfigureAwait(false);
            if (category == null) return null;
            var categoryDto = _mapper.Map<List<ForgetPasswordDetailsDTO>>(category);
            return categoryDto;
        }
    }
}
