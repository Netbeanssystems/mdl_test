using Application.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Application.ServiceInterfaces
{
    public interface IRefreshTokenService
    {
        //Common Methods
        Task<List<RefreshTokenDTO>> Get();
        Task<RefreshTokenDTO> Get(int id);
        Task<RefreshTokenDTO> Create(RefreshTokenDTO modelDto);
        Task<RefreshTokenDTO> Update(RefreshTokenDTO modelDto);
        Task<int> Delete(int id);
        //Custom Methods
    }
}
