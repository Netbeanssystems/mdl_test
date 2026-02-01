using Application.Dtos;
using Application.ServiceInterfaces;
using AutoMapper;
using Domain.Models;
using Domain.RepositoryInterfaces;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AuthenticationTicketsBankService : IAuthenticationTicketsBankService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AuthenticationTicketsBankService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        //Common Methods
        public async Task<AuthenticationTicketsBankDTO> Get(string id)
        {
            var model = await _unitOfWork.AuthenticationTicketsBankRepo.GetByIdStr(id).ConfigureAwait(false);
            if (model == null) return null;
            var modelDto = _mapper.Map<AuthenticationTicketsBankDTO>(model);
            return modelDto;
        }
        public async Task<AuthenticationTicketsBankDTO> Create(AuthenticationTicketsBankDTO modelDto)
        {
            if (modelDto == null) return null;
            var model = _mapper.Map<AuthenticationTicketsBank>(modelDto);
            _unitOfWork.AuthenticationTicketsBankRepo.Create(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            modelDto = _mapper.Map<AuthenticationTicketsBankDTO>(model);
            return rowsChanged > 0 ? modelDto : null;
        }
        public async Task<AuthenticationTicketsBankDTO> Update(AuthenticationTicketsBankDTO modelDto)
        {
            if (modelDto == null) return null;
            _unitOfWork.AuthenticationTicketsBankRepo.Update(_mapper.Map<AuthenticationTicketsBank>(modelDto));
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? modelDto : null;
        }
        public async Task<int> Delete(string id)
        {
            var model = await _unitOfWork.AuthenticationTicketsBankRepo.GetByIdStr(id).ConfigureAwait(false);
            if (model == null) return -1;
            _unitOfWork.AuthenticationTicketsBankRepo.Delete(model);
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
        public async Task<AuthenticationTicketsBankDTO> Upsert(AuthenticationTicketsBankDTO modelDto)
        {
            if (modelDto == null) return null;
            var row = _mapper.Map<AuthenticationTicketsBank>(modelDto);
            //if (_unitOfWork.AuthenticationTicketsBankRepo.GetEntityState(row) != EntityState.Detached) continue;
            var ExistingRow = await _unitOfWork.AuthenticationTicketsBankRepo.GetByUserId(row.UserId).ConfigureAwait(false);
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
                _unitOfWork.AuthenticationTicketsBankRepo.Update(ExistingRow);
                //var attachedEntry = _unitOfWork.AuthenticationTicketsBankRepo.GetEntityEntry(ExistingRow);
                //attachedEntry.CurrentValues.SetValues(row);
            }
            else
            {
                _unitOfWork.AuthenticationTicketsBankRepo.Create(row);
            }
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            modelDto = _mapper.Map<AuthenticationTicketsBankDTO>(row);
            return rowsChanged > 0 ? modelDto : null;
        }
    }
}
