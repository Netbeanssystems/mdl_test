using Application.Extensions;
using Application.ServiceInterfaces;
using Application.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebForeignBidder.Helpers;

namespace WebForeignBidder.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly IConfiguration _config;
        private readonly INotyfService _notyf;
        public IndexModel(
            IHttpClientService httpClient,
            IConfiguration config,
            INotyfService notyf)
        {
            _httpClient = httpClient;
            _config = config;
            _notyf = notyf;
        }
        public int InternalStudentFormId { get; set; }
        public int InternalInstitutionId { get; set; }
        public bool ShowChangePasswordModal => HttpContext.Session.GetString("chn") == "Y";
        public Dictionary<string, List<int>> mappings => DataHelper.GetMappings(User);
        public DashboardAlertsVM DashboardAlertVm { get; set; }
        public IActionResult OnGet()
        {
            if (User.Identity?.IsAuthenticated == false) return RedirectToPage("/Account/Login");
            //Set Profile Image to session on login
            var ImageName = DataHelper.GetUserProfileImage(User);
            var ProfileImage = !string.IsNullOrEmpty(ImageName) ? ImageName : _config["DefaultUserImage"];
            HttpContext.Session.SetString("ProfileImage", ProfileImage);
            //var alertResponse = await _httpClient.GetAsync("DashboardAlerts/GetActiveAlert", false).ConfigureAwait(false);
            //DashboardAlertVm = !string.IsNullOrEmpty(alertResponse) ? JsonConvert.DeserializeObject<DashboardAlertsVM>(alertResponse) : null;
            return Page();
        }
        public async Task<IActionResult> OnGetSetPasswordStatus()
        {
            var uid = DataHelper.GetUserId(User);
            var result = await _httpClient.GetAsync("Users/UpdatePasswordStatus", true, uid, 0).ConfigureAwait(false);
            if (result == "unauthorized")
            {
                _notyf.Information("Please login");
                return RedirectToPage("/Account/Login");
            }
            var success = !string.IsNullOrEmpty(result) && Convert.ToBoolean(result);
            if (!success)
                _notyf.Error("Password status couldn't be updated");
            HttpContext.Session.SetString("chn", "N");
            return Page();
        }
        public IActionResult OnGetChangePasswordLater()
        {
            HttpContext.Session.SetString("chn", "N");
            return Page();
        }
        public IActionResult OnGetChangeAcademicYear(int IdPP)
        {
            HttpContext.Session.SetObjectToSession("AcademicYearId", IdPP);
            return new JsonResult(true);
        }
        //public async Task<IActionResult> OnGetDashboardStats(string table)
        //{
        //    var response = await _httpClient.GetAsync("DashBoard/GetDashboardStats", true, table).ConfigureAwait(false);
        //    if (response == "unauthorized") return new JsonResult("unauthorized");
        //    var Stats = !string.IsNullOrEmpty(response) && response != "unauthorized" ? JsonConvert.DeserializeObject<List<DashBoardChartStatVM>>(response) : null;
        //    if (Stats == null || Stats.Count <= 0)
        //        return new JsonResult("");
        //    return new JsonResult(Stats);
        //}
        //public async Task<IActionResult> OnGetDashboardTables()
        //{
        //    var AcademicYearId = !string.IsNullOrEmpty(HttpContext.Session.GetObjectFromSession<string>("AcademicYearId"))
        //        ? Convert.ToInt32(HttpContext.Session.GetObjectFromSession<string>("AcademicYearId"))
        //        : await DataHelper.GetCurrentAcademicYearId(_httpClient);
        //    var response = await _httpClient.GetAsync("DashBoard/GetStats", true, AcademicYearId).ConfigureAwait(false);
        //    if (response == "unauthorized") return new JsonResult("unauthorized");
        //    var Stats = !string.IsNullOrEmpty(response) && response != "unauthorized" ? JsonConvert.DeserializeObject<List<DashboardStatsVM>>(response) : null;
        //    if (Stats == null) return new JsonResult("");
        //    var ViewResultTR = new PartialViewResult
        //    {
        //        ViewName = "_DashboardCardPartial",
        //        ViewData = new ViewDataDictionary<List<DashboardStatsVM>>(ViewData, Stats)
        //    };
        //    return ViewResultTR;
        //}
    }
}
