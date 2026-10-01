using Application.Dtos;
using System.Threading.Tasks;

namespace Application.ServiceInterfaces
{
    public interface IEnquiryService
    {
        Task<EnquiryDTO> Create(EnquiryDTO argModelDto);
    }
}
