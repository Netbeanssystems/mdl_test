using Application.Dtos;
using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
namespace Infrastructure.Repositories
{
    public class AuthRepository : Repository<ResetPasswordDTO>, IAuthRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public AuthRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<bool> ResetPassword(string username, string newPassword)
        {
            var user = await DbContext.AspNetUsers.Where(x => x.Id == username).FirstOrDefaultAsync();
            user.Pwd_SHA512 = newPassword;
            DbContext.SaveChanges();
            return true;
        }
    }
}
