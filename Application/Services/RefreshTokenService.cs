using Application.Dtos;
using Application.ServiceInterfaces;
using AutoMapper;
using Domain.Models;
using Domain.RepositoryInterfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Application.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public RefreshTokenService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        //Common Methods
        public async Task<List<RefreshTokenDTO>> Get()
        {
            var refreshTokens = await _unitOfWork.RefreshTokensRepo.Get().ConfigureAwait(false);
            if (refreshTokens == null || refreshTokens.Count <= 0) return null;
            var refreshTokenDtos = _mapper.Map<List<RefreshTokenDTO>>(refreshTokens);
            if (refreshTokenDtos == null || refreshTokenDtos.Count <= 0) return null;
            return refreshTokenDtos;
        }
        public async Task<RefreshTokenDTO> Get(int id)
        {
            var refreshToken = await _unitOfWork.RefreshTokensRepo.Get(id).ConfigureAwait(false);
            if (refreshToken == null) return null;
            var refreshTokenDto = _mapper.Map<RefreshTokenDTO>(refreshToken);
            return refreshTokenDto;
        }
        public async Task<RefreshTokenDTO> Create(RefreshTokenDTO modelDto)
        {
            if (modelDto == null) return null;
            var refreshToken = _mapper.Map<RefreshToken>(modelDto);
            _unitOfWork.RefreshTokensRepo.Create(refreshToken);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? modelDto : null;
        }
        public async Task<RefreshTokenDTO> Update(RefreshTokenDTO modelDto)
        {
            if (modelDto == null) return null;
            var refreshToken = _mapper.Map<RefreshToken>(modelDto);
            _unitOfWork.RefreshTokensRepo.Update(refreshToken);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? modelDto : null;
        }
        public async Task<int> Delete(int id)
        {
            var refreshToken = await _unitOfWork.RefreshTokensRepo.Get(id).ConfigureAwait(false);
            if (refreshToken == null) return -1;
            _unitOfWork.RefreshTokensRepo.Delete(refreshToken);
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
    }
}
