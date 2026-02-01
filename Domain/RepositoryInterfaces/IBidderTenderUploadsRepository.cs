using Domain.Models;
using System.Threading.Tasks;

namespace Domain.RepositoryInterfaces
{
    public interface IBidderTenderUploadsRepository : IRepository<BidderTenderUploads>
    {
        Task<BidderTenderCorrigendumUploads> CheckTender(BidderTenderUploads model);
    }
}

