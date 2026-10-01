using Application.Dtos;
using Application.ServiceInterfaces;
using AutoMapper;
using Domain.Models;
using Domain.RepositoryInterfaces;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AuthenticationTicketsService : IAuthenticationTicketsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AuthenticationTicketsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        //Common Methods
        public async Task<AuthenticationTicketsDTO> Get(string id)
        {
            var model = await _unitOfWork.AuthenticationTicketsRepo.GetByIdStr(id).ConfigureAwait(false);
            if (model == null) return null;
            var modelDto = _mapper.Map<AuthenticationTicketsDTO>(model);
            return modelDto;
        }
        public async Task<AuthenticationTicketsDTO> Create(AuthenticationTicketsDTO modelDto)
        {
            if (modelDto == null) return null;
            var model = _mapper.Map<AuthenticationTickets>(modelDto);
            _unitOfWork.AuthenticationTicketsRepo.Create(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            modelDto = _mapper.Map<AuthenticationTicketsDTO>(model);
            return rowsChanged > 0 ? modelDto : null;
        }
        public async Task<AuthenticationTicketsDTO> Update(AuthenticationTicketsDTO modelDto)
        {
            if (modelDto == null) return null;
            _unitOfWork.AuthenticationTicketsRepo.Update(_mapper.Map<AuthenticationTickets>(modelDto));
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? modelDto : null;
        }
        public async Task<int> Delete(string id)
        {
            var model = await _unitOfWork.AuthenticationTicketsRepo.GetByIdStr(id).ConfigureAwait(false);
            if (model == null) return -1;
            _unitOfWork.AuthenticationTicketsRepo.Delete(model);
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
        public async Task<AuthenticationTicketsDTO> Upsert(AuthenticationTicketsDTO modelDto)
        {
            if (modelDto == null) return null;
            var row = _mapper.Map<AuthenticationTickets>(modelDto);
            //if (_unitOfWork.AuthenticationTicketsRepo.GetEntityState(row) != EntityState.Detached) continue;
            var ExistingRow = await _unitOfWork.AuthenticationTicketsRepo.GetByUserId(row.UserId).ConfigureAwait(false);
            if (ExistingRow != null)
            {
                ExistingRow.UserId = modelDto.UserId;
                ExistingRow.UserName = modelDto.UserName;
                ExistingRow.Value = modelDto.Value;
                ExistingRow.LastActivity = modelDto.LastActivity;
                ExistingRow.Expires = modelDto.Expires;
                ExistingRow.RemoteIpAddress = modelDto.RemoteIpAddress;
                ExistingRow.OperatingSystem = modelDto.OperatingSystem;
                ExistingRow.UserAgentFamily = modelDto.UserAgentFamily;
                ExistingRow.UserAgentVersion = modelDto.UserAgentVersion;
                _unitOfWork.AuthenticationTicketsRepo.Update(ExistingRow);
                //var attachedEntry = _unitOfWork.AuthenticationTicketsRepo.GetEntityEntry(ExistingRow);
                //attachedEntry.CurrentValues.SetValues(row);
            }
            else
            {
                _unitOfWork.AuthenticationTicketsRepo.Create(row);
            }
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            modelDto = _mapper.Map<AuthenticationTicketsDTO>(row);
            return rowsChanged > 0 ? modelDto : null;
        }
    }
}
