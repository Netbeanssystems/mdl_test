using Application.Dtos;
using System.Threading.Tasks;

namespace Application.ServiceInterfaces
{
    public interface IContactUsService
    {
        Task<ContactUsDTO> Create(ContactUsDTO argModelDto);
    }
}
