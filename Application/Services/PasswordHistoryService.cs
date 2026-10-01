using Application.ServiceInterfaces;
using Application.ViewModels;
using AutoMapper;
using Domain.Models;
using Domain.RepositoryInterfaces;
using System;
using System.Threading.Tasks;

namespace Application.Services
{
    public class PasswordHistoryService : IPasswordHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public PasswordHistoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PasswordHistoryVM> GetByUsername(string username)
        {
            var model = await _unitOfWork.passhistoryRepo.GetByUsername(username).ConfigureAwait(false);
            if (model == null) return null;
            var modelDto = _mapper.Map<PasswordHistoryVM>(model);
            return modelDto;
        }

        public async Task<PasswordHistoryVM> GetByUsernamePwd(string username, string pwd)
        {
            var model = await _unitOfWork.passhistoryRepo.GetByUsernamePwd(username, pwd).ConfigureAwait(false);
            if (model == null) return null;
            var modelDto = _mapper.Map<PasswordHistoryVM>(model);
            return modelDto;
        }


        public async Task<PasswordHistoryVM> CreateOrUpdate(PasswordHistoryVM argModelDto)
        {
            var rowsChanged = 0;
            if (argModelDto == null) return null;
            var model = await _unitOfWork.passhistoryRepo.GetByUsername(argModelDto.Username).ConfigureAwait(false);

            if (model == null)
            {
                var insmodel = _mapper.Map<PasswordHistory>(argModelDto);
                _unitOfWork.passhistoryRepo.Create(insmodel);
                rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                argModelDto.Id = insmodel.Id;
            }
            else
            {
                var modelbyid = await _unitOfWork.passhistoryRepo.Get(model.Id).ConfigureAwait(false);

                if (modelbyid.Pwd1 == null)
                {
                    modelbyid.Pwd1 = argModelDto.Pwd1 ?? string.Empty;
                    modelbyid.Pwd2 = string.Empty;
                    modelbyid.Pwd3 = string.Empty;
                    modelbyid.LastUpdated = DateTime.Now;
                }
                else if (modelbyid.Pwd2 == null)
                {
                    modelbyid.Pwd2 = argModelDto.Pwd1 ?? string.Empty;
                    modelbyid.LastUpdated = DateTime.Now;
                }
                else if (modelbyid.Pwd3 == null)
                {
                    modelbyid.Pwd3 = argModelDto.Pwd1 ?? string.Empty;
                    modelbyid.LastUpdated = DateTime.Now;
                }
                else
                {
                    modelbyid.Pwd1 = modelbyid.Pwd2 ?? string.Empty;
                    modelbyid.Pwd2 = modelbyid.Pwd3 ?? string.Empty;
                    modelbyid.Pwd3 = argModelDto.Pwd1 ?? string.Empty;
                    modelbyid.LastUpdated = DateTime.Now;
                }

                _unitOfWork.passhistoryRepo.Update(modelbyid);
                rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }

            return rowsChanged > 0 ? argModelDto : null;
        }
    }
}
