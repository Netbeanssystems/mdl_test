using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Domain.RepositoryInterfaces
{
    public interface IGenaralUploadDocumentsRepository : IRepository<GenaralUploadDocuments>
    {
        Task<List<GenaralUploadDocuments>> Getbycreatedby(string createdby);
    }
}
