using Application.Dtos;
using System.Threading.Tasks;

namespace Application.ServiceInterfaces
{
    public interface IGrievanceService
    {
        Task<GrievanceDTO> Create(GrievanceDTO argModelDto);
    }
}
