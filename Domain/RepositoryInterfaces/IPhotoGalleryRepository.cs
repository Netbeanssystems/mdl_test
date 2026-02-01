using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryInterfaces
{
    public interface IPhotoGalleryRepository : IRepository<PhotoGallery>
    {
        Task<List<PhotoGallery>> Getorder(string Key);
        Task<List<PhotoGallery>> GetorderHindi(string Key);
        Task<List<PhotoGallery>> GetorderPriortyForAwardAccolades(string Key);
        Task<List<PhotoGallery>> GetorderPriortyForAwardAccoladeshindi(string Key);
        Task<List<PhotoGallery>> GetCategory(string Key);
        Task<List<PhotoGallery>> GetbyPriorty();
        Task<List<PhotoGallery>> GetbyPriortyForAwardAccolades();
        List<PhotoGalleryModel> GetEvents();
    }
}
