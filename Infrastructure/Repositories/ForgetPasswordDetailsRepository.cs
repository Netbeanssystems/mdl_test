using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ForgetPasswordDetailsRepository : Repository<ForgetPasswordDetails>, IForgetPasswordDetailsRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public ForgetPasswordDetailsRepository(AppDbContext context) : base(context)
        {
        }

        public Task<List<ForgetPasswordDetails>> CheckUserDetails(string userid, string date)
        {
            return DbContext.ForgetPasswordDetails.Where(x => x.UserId == userid && x.CreatedDate == Convert.ToDateTime(date)).ToListAsync();

        }


    }
}
