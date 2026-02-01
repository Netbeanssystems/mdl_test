using Application.Dtos;
using Application.ViewModels;
using System.Collections.Generic;

namespace Application.Helpers
{
    public static class Notifier
    {
        public static NotificationsDTO Notify(NotificationVM NotificationVm)
        {
            var NotificationDetailDtos = new List<NotificationDetailsDTO>(NotificationVm.UserNames.Count);
            foreach (var username in NotificationVm.UserNames)
            {
                var NotificationDetailDto = new NotificationDetailsDTO
                {
                    Text = NotificationVm.Text,
                    RoleName = NotificationVm.Role,
                    UserName = username,
                    TargetUrl = NotificationVm.TargetUrl,
                    IsActive = true
                };
                NotificationDetailDto = ModelAuditor<NotificationDetailsDTO>.SetAudit(NotificationVm.UserName, "Create", NotificationVm.Ip, NotificationDetailDto);
                NotificationDetailDtos.Add(NotificationDetailDto);
            }
            var notificationDto = new NotificationsDTO
            {
                Icon = string.IsNullOrEmpty(NotificationVm.IconClass) ? "far fa-bell" : NotificationVm.IconClass,
                Title = NotificationVm.Title,
                IsActive = true,
                NotificationDetails = NotificationDetailDtos
            };
            return ModelAuditor<NotificationsDTO>.SetAudit(NotificationVm.UserName, "Create", NotificationVm.Ip, notificationDto);
        }
    }
}
