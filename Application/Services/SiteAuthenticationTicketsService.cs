using Application.Dtos;
using Application.ServiceInterfaces;
using AutoMapper;
using Domain.Models;
using Domain.RepositoryInterfaces;
using System.Threading.Tasks;

namespace Application.Services
{
    public class SiteAuthenticationTicketsService : ISiteAuthenticationTicketsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public SiteAuthenticationTicketsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        //Common Methods
        public async Task<SiteAuthenticationTicketsDTO> Get(string id)
        {
            var model = await _unitOfWork.SiteAuthenticationTicketsRepo.GetByIdStr(id).ConfigureAwait(false);
            if (model == null) return null;
            var modelDto = _mapper.Map<SiteAuthenticationTicketsDTO>(model);
            return modelDto;
        }
        public async Task<SiteAuthenticationTicketsDTO> Create(SiteAuthenticationTicketsDTO modelDto)
        {
            if (modelDto == null) return null;
            var model = _mapper.Map<SiteAuthenticationTickets>(modelDto);
            _unitOfWork.SiteAuthenticationTicketsRepo.Create(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            modelDto = _mapper.Map<SiteAuthenticationTicketsDTO>(model);
            return rowsChanged > 0 ? modelDto : null;
        }
        public async Task<SiteAuthenticationTicketsDTO> Update(SiteAuthenticationTicketsDTO modelDto)
        {
            if (modelDto == null) return null;
            _unitOfWork.SiteAuthenticationTicketsRepo.Update(_mapper.Map<SiteAuthenticationTickets>(modelDto));
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? modelDto : null;
        }
        public async Task<int> Delete(string id)
        {
            var model = await _unitOfWork.SiteAuthenticationTicketsRepo.GetByIdStr(id).ConfigureAwait(false);
            if (model == null) return -1;
            _unitOfWork.SiteAuthenticationTicketsRepo.Delete(model);
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
        public async Task<SiteAuthenticationTicketsDTO> Upsert(SiteAuthenticationTicketsDTO modelDto)
        {
            if (modelDto == null) return null;
            var row = _mapper.Map<SiteAuthenticationTickets>(modelDto);
            //if (_unitOfWork.SiteAuthenticationTicketsRepo.GetEntityState(row) != EntityState.Detached) continue;
            var ExistingRow = await _unitOfWork.SiteAuthenticationTicketsRepo.GetByUserId(row.UserId).ConfigureAwait(false);
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
                _unitOfWork.SiteAuthenticationTicketsRepo.Update(ExistingRow);
                //var attachedEntry = _unitOfWork.SiteAuthenticationTicketsRepo.GetEntityEntry(ExistingRow);
                //attachedEntry.CurrentValues.SetValues(row);
            }
            else
            {
                _unitOfWork.SiteAuthenticationTicketsRepo.Create(row);
            }
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            modelDto = _mapper.Map<SiteAuthenticationTicketsDTO>(row);
            return rowsChanged > 0 ? modelDto : null;
        }
    }
}
