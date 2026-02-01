using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryInterfaces
{
    public interface IMediaRepository : IRepository<Media>
    {
        Task<Media> GetMenus(int Id);
        Task<Media> Getlastrecord();
        Task<List<Media>> Getorderdesc();
        Task<List<Media>> Getnews();
        Task<List<Media>> GetNewsData(int Heading);
        //Task<List<int>> GetNewsYear();
        Task<List<Media>> Getorder();

        Task<List<MediaNewData>> GetMedia();            //---------Karn 
    }
}
