using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Infrastructure.Repositories
{
    public class GenaralUploadDocumentsRepository : Repository<GenaralUploadDocuments>, IGenaralUploadDocumentsRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public GenaralUploadDocumentsRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<List<GenaralUploadDocuments>> Getbycreatedby(string createdby)
        {

            // return await DbContext.Events.ToListAsync();


            return await DbContext.GenaralUploadDocuments
    .Where(c => c.CreatedBy == createdby)  // Ensure 'createdby' is defined
    .OrderBy(c => c.CreatedBy)
    .ToListAsync();
        }
    }
}
