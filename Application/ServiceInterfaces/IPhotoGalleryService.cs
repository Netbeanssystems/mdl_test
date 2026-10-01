using Application.Dtos;
using Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.ServiceInterfaces
{
    public interface IPhotoGalleryService
    {
        List<PhotoGalleryVM> Get();
        Task<List<PhotoGalleryVM>> GetbyPriorty();
        Task<List<PhotoGalleryVM>> GetbyPriortyForAwardAccolades();
        Task<PhotoGalleryDTO> Get(int Id);
        Task<PhotoGalleryDTO> Add(PhotoGalleryDTO videoDto);
        Task<PhotoGalleryDTO> Update(PhotoGalleryDTO videoDto);
        Task<int> Remove(int id);
        Task<List<PhotoGalleryVM>> Getorder(string Key, int PageNo, int PageSize);
        Task<List<PhotoGalleryVM>> GetorderHindi(string Key, int PageNo, int PageSize);
        Task<List<PhotoGalleryVM>> GetorderPriortyForAwardAccolades(string Key, int PageNo, int PageSize);
        Task<List<PhotoGalleryVM>> GetorderPriortyForAwardAccoladeshindi(string Key, int PageNo, int PageSize);
        Task<List<PhotoGalleryVM>> GetCategory(string Key);
        List<TopEventsVM> GetTopEvents();
        List<DropdownVM> GetYears();
        List<TopEventsVM> GetEventsByYear(int year);
        List<EventPhotosVM> GetPhotos(int id);
    }
}
