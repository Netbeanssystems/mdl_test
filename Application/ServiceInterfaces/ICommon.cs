using Microsoft.AspNetCore.Http;

namespace Application.ServiceInterfaces
{
    public interface ICommon
    {
        bool CheckValidFile(IFormFile file);
        bool CheckValidJpgFile(IFormFile file);
    }
}
