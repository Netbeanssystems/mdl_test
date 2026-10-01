using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Domain.RepositoryInterfaces
{
    public interface IBidderGeneralDocRepository : IRepository<BidderGeneralDoc>
    {
        //Task<List<BidderGeneralDoc>> GetWithAll();
        //Task<BidderGeneralDoc> CheckDuplicate(BidderGeneralDoc model);
    }
}
