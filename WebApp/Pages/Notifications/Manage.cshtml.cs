using Application.Dtos;
using Application.Extensions;
using Application.Helpers;
using Application.ServiceInterfaces;
using Application.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Pages.Notifications
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class ManageModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly INotyfService _notyf;
        public ManageModel(
            IHttpClientService httpClient,
            INotyfService notyf)
        {
            _httpClient = httpClient;
            _notyf = notyf;
        }
        [FromRoute] public int? id { get; set; }
        [BindProperty] public NotificationsDTO Notification { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(256, ErrorMessage = "{1} characters max")]
        [BindProperty] public string Text { get; set; }
        [StringLength(128, ErrorMessage = "{1} characters max")]
        [BindProperty] public string TargetUrl { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        [BindProperty] public string[] Roles { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        [BindProperty] public string[] Users { get; set; }
        public bool IsNew => Notification == null;
        public async Task<IActionResult> OnGet()
        {
            //get all the roles for dropdown
            var rolesResult = await _httpClient.GetAsync("Roles/Get", true).ConfigureAwait(false);
            if (rolesResult == "unauthorized") return RedirectToPage("/Account/Login");
            var roles = !string.IsNullOrEmpty(rolesResult) ? JsonConvert.DeserializeObject<List<RoleDTO>>(rolesResult) : null;
            if (roles == null || roles.Count <= 0)
            {
                _notyf.Error("Record not found");
                return Page();
            }
            roles.Remove(roles.Find(x => x.Name == "Anonymous"));
            ViewData["Roles"] = new SelectList(roles, "Name", "Name");
            if (!id.HasValue) return Page();
            var Result = await _httpClient.GetAsync("Notifications/Get", true, id).ConfigureAwait(false);
            if (Result == "unauthorized")
            {
                _notyf.Information("Please login");
                return RedirectToPage("/Account/Login");
            }
            Notification = !string.IsNullOrEmpty(Result) ? JsonConvert.DeserializeObject<NotificationsDTO>(Result) : null;
            if (Notification == null || Notification.NotificationDetails?.Count <= 0) return Page();
            Text = Notification.NotificationDetails.First().Text;
            TargetUrl = Notification.NotificationDetails.First().TargetUrl;
            Roles = Notification.NotificationDetails.Select(x => x.RoleName).ToArray();
            var UserRoleVms = Notification.NotificationDetails.Select(x => new UserRoleVM
            {
                Role = $"{x.RoleName}-{x.UserName}",
                UserName = x.UserName
            }).ToList();
            Users = UserRoleVms.Select(x => x.Role).ToArray();
            ViewData["Users"] = new SelectList(UserRoleVms, "Role", "UserName");
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            var action = (id == null || IsNew) ? "Create" : "Edit";
            var NotificationDetailDtos = new List<NotificationDetailsDTO>(Users.Length);
            foreach (var user in Users)
            {
                var NotificationDetailDto = new NotificationDetailsDTO
                {
                    NotificationId = Notification.Id,
                    Text = Text,
                    RoleName = user.Split("-")[0],
                    UserName = user.Split("-")[1],
                    TargetUrl = TargetUrl,
                    IsActive = true
                };
                NotificationDetailDto = ModelAuditor<NotificationDetailsDTO>.SetAudit(User.Identity.Name, "Create", HttpContext.Connection.RemoteIpAddress.ToString(), NotificationDetailDto);
                NotificationDetailDtos.Add(NotificationDetailDto);
            }
            Notification.NotificationDetails = NotificationDetailDtos;
            Notification = ModelAuditor<NotificationsDTO>.SetAudit(User.Identity.Name, action, HttpContext.Connection.RemoteIpAddress.ToString(), Notification);
            if (!ModelState.IsValid) { _notyf.Error(ModelState.GetErrorMessageString()); return Page(); }
            var response = id == null || IsNew
                ? await _httpClient.PostAsync("Notifications/Create", true, Notification).ConfigureAwait(false)
                : await _httpClient.PutAsync("Notifications/Edit", true, Notification.Id, Notification)
                    .ConfigureAwait(false);
            if (response == "unauthorized")
            {
                _notyf.Information("Please login");
                return RedirectToPage("/Account/Login");
            }
            Notification = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<NotificationsDTO>(response) : null;
            if (Notification == null)
                _notyf.Error("Save failed");
            else
                _notyf.Success("Saved successfully");
            return RedirectToPage("Index");
        }
        public async Task<IActionResult> OnPostDelete(int Id)
        {
            var DeleteResult = await _httpClient.DeleteAsync("Notifications/Delete", true, Id).ConfigureAwait(false);
            if (DeleteResult == "unauthorized")
            {
                _notyf.Information("Please login");
                return RedirectToPage("/Account/Login");
            }
            var RowsChanged = !string.IsNullOrEmpty(DeleteResult) && Convert.ToInt32(DeleteResult) > 0;
            if (RowsChanged)
                _notyf.Success("Deleted successfully");
            else
                _notyf.Error("Delete failed. There might be active child records.");
            return RedirectToPage("Index");
        }
        public async Task<JsonResult> OnGetUsersByRoles(string roles)
        {
            var RolesList = roles.Split(',').ToList();
            var Result = await _httpClient.PostAsync("Users/GetByRoles", true, RolesList).ConfigureAwait(false);
            var userRoleVms = !string.IsNullOrEmpty(Result) ? JsonConvert.DeserializeObject<List<UserRoleVM>>(Result) : null;
            return new JsonResult(userRoleVms);
        }
    }
}