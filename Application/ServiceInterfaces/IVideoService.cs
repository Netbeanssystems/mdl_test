using Application.Dtos;
using Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.ServiceInterfaces
{
    public interface IVideoService
    {
        Task<List<VideoVM>> Get();
        //Task<List<VideoVM>> GetLastest();
        Task<VideoDTO> Get(int Id);
        Task<VideoDTO> Add(VideoDTO videoDto);
        Task<VideoDTO> Update(VideoDTO videoDto);
        Task<int> Remove(int id);
    }
}
