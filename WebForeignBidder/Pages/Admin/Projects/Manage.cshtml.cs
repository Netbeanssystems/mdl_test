using Application.Dtos;
using Application.Extensions;
using Application.ServiceInterfaces;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace WebForeignBidder.Pages.Admin.Projects
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
        [BindProperty] public BidderProjectsDTO projects { get; set; }
        public bool IsNew => projects == null;
        public async Task<IActionResult> OnGet()
        {
            if (string.IsNullOrEmpty(id)) return Page();
            var userResult = await _httpClient.GetAsync("Projects/Get", true, id).ConfigureAwait(false);
            if (userResult == "unauthorized")
            {
                _notyf.Information("Please login");
                return RedirectToPage("/Account/Login");
            }
            projects = !string.IsNullOrEmpty(userResult) ? JsonConvert.DeserializeObject<BidderProjectsDTO>(userResult) : null;
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid) { _notyf.Error(ModelState.GetErrorMessageString()); return Page(); }
            var userResult = projects.Id == 0 || IsNew
                ? await _httpClient.PostAsync("Projects/Create", true, projects).ConfigureAwait(false)
                : await _httpClient.PostAsync("Projects/Create", true, projects).ConfigureAwait(false);

            //: await _httpClient.PutAsync("Projects/Create", true, projects.Id, projects).ConfigureAwait(false);
            if (userResult == "unauthorized")
            {
                _notyf.Information("Please login");
                return RedirectToPage("/Account/Login");
            }
            projects = !string.IsNullOrEmpty(userResult) ? JsonConvert.DeserializeObject<BidderProjectsDTO>(userResult) : null;
            if (projects == null)
                _notyf.Error("Save failed or yard id has in use");
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
    }
}