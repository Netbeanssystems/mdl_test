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
    public class NotificationsRepository : Repository<Notifications>, INotificationsRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public NotificationsRepository(AppDbContext context) : base(context)
        {
        }
        public Task<List<Notifications>> GetAll()
        {
            return DbContext.Notifications
                .Where(x =>
                    x.IsActive)
                .OrderByDescending(c => c.Id)
                .ToListAsync();
        }
        public Task<List<Notifications>> GetByUser(string unm)
        {
            return DbContext.Notifications
                .Where(x => x.IsActive && x.NotificationDetails.Any(c => c.UserName == unm && c.IsActive))
                .Distinct()
                .OrderByDescending(c => c.Id)
                .ToListAsync();
        }
        public void UpdateWithChildren(Notifications model)
        {
            //Get and Update the parent
            var existingParent = DbContext.Notifications
                .Include(x => x.NotificationDetails)
                .FirstOrDefault(x => x.Id == model.Id);
            if (existingParent == null) return;
            DbContext.Entry(existingParent).CurrentValues.SetValues(model);
            // Delete all children
            foreach (var existingChild in existingParent.NotificationDetails)
                DbContext.NotificationDetails.Remove(existingChild);
            DbContext.SaveChanges();
            //Insert children
            foreach (var childModel in model.NotificationDetails)
            {
                // Insert child
                var newChild = new NotificationDetails
                {
                    NotificationId = childModel.NotificationId,
                    Text = childModel.Text,
                    TargetUrl = childModel.TargetUrl,
                    RoleName = childModel.RoleName,
                    UserName = childModel.UserName,
                    CreatedDate = childModel.CreatedDate,
                    CreatedBy = childModel.CreatedBy,
                    IsActive = childModel.IsActive,
                    ModifiedDate = childModel.ModifiedDate,
                    ModifiedBy = childModel.ModifiedBy,
                    IP = childModel.IP
                };
                existingParent.NotificationDetails.Add(newChild);
            }
        }
        public void CreateWithChildren(Notifications model)
        {
            //Get and Update the parent
            var existingParent = DbContext.Notifications.FirstOrDefault(x => x.Title == model.Title);
            if (existingParent != null)
            {
                //Insert children
                for (var i = 0; i < model.NotificationDetails.Count; i++)
                {
                    model.NotificationDetails[i].NotificationId = existingParent.Id;
                    var existingChild = existingParent.NotificationDetails.FirstOrDefault(x =>
                        x.Text == model.NotificationDetails[i].Text &&
                        x.TargetUrl == model.NotificationDetails[i].TargetUrl &&
                        x.RoleName == model.NotificationDetails[i].RoleName &&
                        x.UserName == model.NotificationDetails[i].UserName &&
                        x.CreatedBy == model.NotificationDetails[i].CreatedBy
                    );
                    if (existingChild == null)
                        existingParent.NotificationDetails.Add(model.NotificationDetails[i]);
                    else
                    {
                        existingChild.CreatedDate = DateTime.UtcNow.AddHours(5.5);
                        existingChild.IsActive = true;
                    }
                }
            }
            else
            {
                DbContext.Notifications.Add(model);
            }
        }
    }
}
