using Application.Dtos;
using Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.ServiceInterfaces
{
    public interface ITempPhotoGalleryService
    {
        Task<List<TempPhotoGalleryVM>> Get();
        Task<TempPhotoGalleryDTO> Get(int Id);
        Task<TempPhotoGalleryDTO> Add(TempPhotoGalleryDTO TempVideoDto);
        Task<TempPhotoGalleryDTO> Update(TempPhotoGalleryDTO TempVideoDto);
        Task<List<TempPhotoGalleryVM>> GetByAction(string status);
        Task<int> Remove(int id);
        Task<TempPhotoGalleryDTO> CreateAudit(TempPhotoGalleryDTO TempVideoDto);

    }
}
