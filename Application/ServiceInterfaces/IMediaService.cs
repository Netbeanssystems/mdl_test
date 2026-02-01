using Application.Dtos;
using Application.ViewModels;
using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.ServiceInterfaces
{
    public interface IMediaService
    {
        Task<List<MediaVM>> Get();
        Task<List<MediaVM>> Getorderdesc(int PageNo, int PageSize, int Minpage);
        Task<List<MediaVM>> Getorderdescfull();
        Task<List<MediaVM>> Getorder(string Key, int PageNo, int PageSize);
        Task<MediaDTO> Get(int Id);
        Task<MediaDTO> Add(MediaDTO pressReleaseDTO);
        Task<MediaDTO> Update(MediaDTO pressReleaseDTO);
        Task<int> Remove(int id);
        Task<MediaVM> GetMenus(int Id);
        Task<List<Media>> GetNewsData(int Heading);
        //Task<List<int>> GetNewsYear();
        Task<List<MediaVM>> Getnews();
        Task<MediaDTO> Getlastrecord();


        Task<List<MediaVMNewModal>> GetAllMedia();              //----------Karn
    }
}
