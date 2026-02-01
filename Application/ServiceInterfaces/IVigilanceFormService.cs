using Application.Dtos;
using System.Threading.Tasks;

namespace Application.ServiceInterfaces
{
    public interface IVigilanceFormService
    {
        Task<VigilanceFormDTO> Create(VigilanceFormDTO argModelDto);
    }
}
