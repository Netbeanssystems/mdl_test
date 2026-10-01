using Application.Dtos;
using Application.Extensions;
using Application.ServiceInterfaces;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebForeignBidder.Pages.Admin.CommercialEx
{
    [Authorize(Roles = "SuperAdmin,BidderSuperAdmin")]
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
        [FromRoute] public string id { get; set; }
        [BindProperty] public RegisterDTO user { get; set; }
        public bool IsNew => user == null;
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

            if (User.IsInRole("BankAdmin")) roles = roles?.Where(role => role.Name.ToLower().Contains("bank")).ToList();

            ViewData["Roles"] = new SelectList(roles, "Name", "Name");

            //get all the projects for dropdown
            var projectsResult = await _httpClient.GetAsync("Projects/Get", true).ConfigureAwait(false);
            if (projectsResult == "unauthorized") return RedirectToPage("/Account/Login");
            var projects = !string.IsNullOrEmpty(projectsResult) ? JsonConvert.DeserializeObject<List<BidderProjectsDTO>>(projectsResult) : null;
            if (projects == null || projects.Count <= 0)
            {
                _notyf.Error("Record not found");
                return Page();
            }

            ViewData["Projects"] = new SelectList(projects, "Id", "ProjectName");

            if (string.IsNullOrEmpty(id)) return Page();
            var userResult = await _httpClient.GetAsync("Users/GetById", true, id).ConfigureAwait(false);
            if (userResult == "unauthorized")
            {
                _notyf.Information("Please login");
                return RedirectToPage("/Account/Login");
            }
            user = !string.IsNullOrEmpty(userResult) ? JsonConvert.DeserializeObject<RegisterDTO>(userResult) : null;
            user.ProjectIds = user.ProjectId
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(id => int.Parse(id.Trim()))
                .ToList();
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid) { _notyf.Error(ModelState.GetErrorMessageString()); return Page(); }

            string result = "";
            foreach (var item in user.ProjectIds) result += item + ",";
            if (result.EndsWith(",")) result = result.Substring(0, result.Length - 1);
            user.ProjectId = result;

            result = "";
            foreach (var item in user.YardIds) result += item + ",";
            if (result.EndsWith(",")) result = result.Substring(0, result.Length - 1);
            user.YardId = result;

            var userResult = string.IsNullOrEmpty(user.Id) || IsNew
                ? await _httpClient.PostAsync("Users/Create", true, user).ConfigureAwait(false)
                : await _httpClient.PutAsync("Users/Edit", true, user.Id, user).ConfigureAwait(false);
            if (userResult == "unauthorized")
            {
                _notyf.Information("Please login");
                return RedirectToPage("/Account/Login");
            }
            user = !string.IsNullOrEmpty(userResult) ? JsonConvert.DeserializeObject<RegisterDTO>(userResult) : null;
            if (user == null)
                _notyf.Error("Save failed");
            else
                _notyf.Success("Saved successfully");
            return RedirectToPage("Index");
        }
        //public async Task<IActionResult> OnPostDelete(string Id)
        //{
        //    var DeleteResult = await _httpClient.DeleteAsync("Users/Delete", true, Id).ConfigureAwait(false);
        //    if (DeleteResult == "unauthorized")
        //    {
        //        _notyf.Information("Please login");
        //        return RedirectToPage("/Account/Login");
        //    }
        //    var RowsChanged = !string.IsNullOrEmpty(DeleteResult) && Convert.ToBoolean(DeleteResult);
        //if (RowsChanged)
        //    _notyf.Success("Deleted successfully");
        //else
        //    _notyf.Error("Delete failed. There might be active child records.");
        //    return RedirectToPage("Index");
        //}
        public async Task<IActionResult> OnGetCheckEmail(string email)
        {
            var response = await _httpClient.GetAsync("Auth/CheckEmail", false, email).ConfigureAwait(false);
            var userExists = !string.IsNullOrEmpty(response) && Convert.ToBoolean(response);
            return new JsonResult(userExists);
        }

        public async Task<JsonResult> OnGetGetYardsByProject(string projectId)
        {
            var yardsResult = await _httpClient.GetAsync("Projects/GetYard", true, projectId).ConfigureAwait(false);
            if (yardsResult == "unauthorized") return new JsonResult(new { error = "unauthorized" });

            var yards = !string.IsNullOrEmpty(yardsResult)
                ? JsonConvert.DeserializeObject<List<BidderYardsDTO>>(yardsResult)
                : new List<BidderYardsDTO>();
            if (yards == null || yards.Count <= 0) return new JsonResult(new { error = "Record Not Found" });

            return new JsonResult(yards);
        }
    }
}