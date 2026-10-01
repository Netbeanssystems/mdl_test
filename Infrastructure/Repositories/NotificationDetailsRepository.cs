using Application.Helpers;
using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Infrastructure.Repositories
{
    public class NotificationDetailsRepository : Repository<NotificationDetails>, INotificationDetailsRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public NotificationDetailsRepository(AppDbContext context) : base(context)
        {
        }
        public Task<List<NotificationDetails>> GetAll()
        {
            return DbContext.NotificationDetails
                .Where(x =>
                    x.IsActive)
                .OrderByDescending(c => c.Id)
                .ToListAsync();
        }
        public Task<List<NotificationDetails>> GetByUser(string unm)
        {
            return DbContext.NotificationDetails
                .Where(x =>
                    x.IsActive &&
                    x.UserName == unm)
                .Distinct()
                .OrderByDescending(c => c.Id)
                .ToListAsync();
        }
        public Task<List<NotificationDetails>> GetByNotificationId(int nid, string unm)
        {
            var predicate = PredicateBuilder.True<NotificationDetails>();
            if (!string.IsNullOrEmpty(unm))
                predicate = predicate.And(i => i.UserName == unm);
            if (nid > 0)
                predicate = predicate.And(i => i.NotificationId == nid);
            return DbContext.NotificationDetails.Where(predicate).Distinct().OrderByDescending(c => c.Id).ToListAsync();
        }
        public Task<NotificationDetails> GetByUrlUser(string url, string unm)
        {
            return DbContext.NotificationDetails
                .FirstOrDefaultAsync(x =>
                    x.TargetUrl == url &&
                    x.CreatedBy == unm);
        }
    }
}
