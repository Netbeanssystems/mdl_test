//using Domain.Models;
//using Domain.RepositoryInterfaces;
//using Infrastructure.Context;

//namespace Infrastructure.Repositories
//{
//    public class VigilanceRepository : Repository<VigilanceForm>, IVigilanceRepository
//    {
//        private AppDbContext DbContext => _dbContext as AppDbContext;
//        public VigilanceRepository(AppDbContext context) : base(context)
//        {

//        }
//    }
//}
using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class VigilanceRepository : Repository<VigilanceForm>, IVigilanceRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;

        public VigilanceRepository(AppDbContext context) : base(context)
        {
        }

        // ✅ 24-hour rate limit check — DB se
        public async Task<bool> HasRecentComplaintByEmail(string email, int hours)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            var cutoff = DateTime.Now.AddHours(-hours);
            var emailLower = email.Trim().ToLower();

            return await DbContext.VigilanceForm
                .AnyAsync(x => x.Email.ToLower() == emailLower && x.SubmitOn >= cutoff)
                .ConfigureAwait(false);
        }
    }
}