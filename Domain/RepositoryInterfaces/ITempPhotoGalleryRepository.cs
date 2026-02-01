using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryInterfaces
{
    public interface ITempPhotoGalleryRepository : IRepository<TempPhotoGallery>
    {
        void CreateAudit(TempPhotoGallery model);
        Task<List<TempPhotoGallery>> GetByAction(string status);
    }
}
