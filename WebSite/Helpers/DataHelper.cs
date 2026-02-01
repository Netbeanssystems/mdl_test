using Application.ServiceInterfaces;
using Application.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace WebSite.Helpers
{
    public static class DataHelper
    {
        /// <summary>
        /// Gets Dropdown List of the entity passed
        /// </summary>
        /// <param name="HttpClient">Http Client Service</param>
        /// <param name="EntityName">Name of the entity</param>
        /// <param name="secure">Add Authentication Header</param>
        /// <returns>Drop Down List with int Id and string Text</returns>
        public static async Task<List<DropdownVM>> GetDropdown(IHttpClientService HttpClient, string EntityName, bool secure = true)
        {
            var response = await HttpClient.GetAsync($"{EntityName}/GetDropdown", secure).ConfigureAwait(false);
            return !string.IsNullOrEmpty(response) && response != "unauthorized" ? JsonConvert.DeserializeObject<List<DropdownVM>>(response) : null;
        }

        /// <summary>
        /// Gets Dropdown List of the entity passed
        /// </summary>
        /// <param name="HttpClient">Http Client Service</param>
        /// <param name="EntityName">Name of the entity</param>
        /// <param name="secure">Add Authentication Header</param>
        /// <returns>Drop Down List with string Id and string Text</returns>
        public static async Task<List<DropdownStrVM>> GetDropdownStr(IHttpClientService HttpClient, string EntityName, bool secure = true)
        {
            var response = await HttpClient.GetAsync($"{EntityName}/GetDropdown", secure).ConfigureAwait(false);
            return !string.IsNullOrEmpty(response) && response != "unauthorized" ? JsonConvert.DeserializeObject<List<DropdownStrVM>>(response) : null;
        }

        /// <summary>
        /// Gets Dropdown List of the entity passed by academic year
        /// </summary>
        /// <param name="HttpClient">Http Client Service</param>
        /// <param name="EntityName">Name of the entity</param>
        /// <param name="AcademicYearId">Academic year Id (can be taken from session)</param>
        /// <param name="secure">Add Authentication Header</param>
        /// <returns>Drop Down List with int Id and string Text</returns>
        public static async Task<List<DropdownVM>> GetDropdownByAcademicYear(IHttpClientService HttpClient, string EntityName, int AcademicYearId, bool secure = true)
        {
            var response = await HttpClient.GetAsync($"{EntityName}/GetDropdownByYear", secure, AcademicYearId).ConfigureAwait(false);
            return !string.IsNullOrEmpty(response) && response != "unauthorized" ? JsonConvert.DeserializeObject<List<DropdownVM>>(response) : null;
        }

        /// <summary>
        /// Gets children entity's dropdown str by parent entity's Id
        /// </summary>
        /// <param name="HttpClient">Http Client Service</param>
        /// <param name="Children">Name of chid entity (plural)</param>
        /// <param name="Parent">Name of parent entity (singular)</param>
        /// <param name="ParentId">Id of the parent entity</param>
        /// <param name="secure">Add Authentication Header</param>
        /// <returns>List of Children DropdownStr By Parent Id</returns>
        public static async Task<List<DropdownStrVM>> GetChildrenDropdownStrByParentId(IHttpClientService HttpClient, string Children, string Parent, int ParentId, bool secure = true)
        {
            var response = await HttpClient.GetAsync($"{Children}/GetDropdownStrBy{Parent}", secure, ParentId).ConfigureAwait(false);
            return !string.IsNullOrEmpty(response) && response != "unauthorized" ? JsonConvert.DeserializeObject<List<DropdownStrVM>>(response) : null;
        }

        /// <summary>
        /// Gets children entity's dropdown by parent entity's Id
        /// </summary>
        /// <param name="HttpClient">Http Client Service</param>
        /// <param name="Children">Name of chid entity (plural)</param>
        /// <param name="Parent">Name of parent entity (singular)</param>
        /// <param name="ParentId">Id of the parent entity</param>
        /// <param name="secure">Add Authentication Header</param>
        /// <returns>List of Children Dropdown By Parent Id</returns>
        public static async Task<List<DropdownVM>> GetChildrenDropdownByParentId(IHttpClientService HttpClient, string Children, string Parent, int ParentId, bool secure = true)
        {
            var response = await HttpClient.GetAsync($"{Children}/GetDropdownBy{Parent}", secure, ParentId).ConfigureAwait(false);
            return !string.IsNullOrEmpty(response) && response != "unauthorized" ? JsonConvert.DeserializeObject<List<DropdownVM>>(response) : null;
        }

        /// <summary>
        /// Gets children entity's dropdown by parent entity's Id
        /// </summary>
        /// <param name="HttpClient">Http Client Service</param>
        /// <param name="Children">Name of chid entity (plural)</param>
        /// <param name="Parent">Name of parent entity (singular)</param>
        /// <param name="ParentName">Name of the parent entity</param>
        /// <param name="secure">Add Authentication Header</param>
        /// <returns>List of Children Dropdown By Parent Id</returns>
        public static async Task<List<DropdownVM>> GetChildrenDropdownByParentName(IHttpClientService HttpClient, string Children, string Parent, string ParentName, bool secure = true)
        {
            var response = await HttpClient.GetAsync($"{Children}/GetDropdownBy{Parent}Name", secure, ParentName).ConfigureAwait(false);
            return !string.IsNullOrEmpty(response) && response != "unauthorized" ? JsonConvert.DeserializeObject<List<DropdownVM>>(response) : null;
        }

        /// <summary>
        /// Gets children entity's dropdown by parent entity's Id
        /// </summary>
        /// <param name="HttpClient">Http Client Service</param>
        /// <param name="Children">Name of chid entity (plural)</param>
        /// <param name="Parent">Name of parent entity (singular)</param>
        /// <param name="ParentId">Id of the parent entity</param>
        /// <param name="SecParentId">Nullable id of the second parent entity</param>
        /// <param name="secure">Add Authentication Header</param>
        /// <returns>List of Children Dropdown By Parent Id</returns>
        public static async Task<List<DropdownVM>> GetChildrenDropdownByParentId(IHttpClientService HttpClient, string Children, string Parent, int ParentId, int? SecParentId, bool secure = true)
        {
            var response = await HttpClient.GetAsync($"{Children}/GetDropdownBy{Parent}", secure, ParentId, SecParentId).ConfigureAwait(false);
            return !string.IsNullOrEmpty(response) && response != "unauthorized" ? JsonConvert.DeserializeObject<List<DropdownVM>>(response) : null;
        }

        ///// <summary>
        ///// Gets Users By Entity Name from UserMappings
        ///// </summary>
        ///// <param name="HttpClient">Http Client Service</param>
        ///// <param name="ename">Name of the entity</param>
        ///// <param name="secure">Add Authentication Header</param>
        ///// <returns>List of UserMappingsVM</returns>
        //public static async Task<List<UserMappingsVM>> GetUsersByEntityName(IHttpClientService HttpClient, string ename, bool secure = true)
        //{
        //    var response = await HttpClient.GetAsync("UserMappings/GetUsersByEntityName", secure, ename).ConfigureAwait(false);
        //    return !string.IsNullOrEmpty(response) && response != "unauthorized" ? JsonConvert.DeserializeObject<List<UserMappingsVM>>(response) : null;
        //}

        ///// <summary>
        ///// Gets Users By Entity Name and Id
        ///// </summary>
        ///// <param name="HttpClient">Http Client Service</param>
        ///// <param name="ename">Name of the entity</param>
        ///// <param name="eid">MapId of the entity</param>
        ///// <param name="secure">Add Authentication Header</param>
        ///// <returns>List of UserMappingsVM</returns>
        //public static async Task<List<UserMappingsVM>> GetUsersByEntityNameId(IHttpClientService HttpClient, string ename, int eid, bool secure = true)
        //{
        //    var modelResponse = await HttpClient.GetAsync("UserMappings/GetUsersByEntityNameId", secure, ename, eid).ConfigureAwait(false);
        //    return !string.IsNullOrEmpty(modelResponse) && modelResponse != "unauthorized" ? JsonConvert.DeserializeObject<List<UserMappingsVM>>(modelResponse) : null;
        //}

        ///// <summary>
        ///// Gets Users By Entity Name and Id
        ///// </summary>
        ///// <param name="HttpClient">Http Client Service</param>
        ///// <param name="ename">Name of the entity</param>
        ///// <param name="uname">Name of the user</param>
        ///// <param name="secure">Add Authentication Header</param>
        ///// <returns>List of UserMappingsVM</returns>
        //public static async Task<List<UserMappingsVM>> GetByEntityNameUserName(IHttpClientService HttpClient, string ename, string uname, bool secure = true)
        //{
        //    var modelResponse = await HttpClient.GetAsync("UserMappings/GetByEntityNameUserName", secure, ename, uname).ConfigureAwait(false);
        //    return !string.IsNullOrEmpty(modelResponse) && modelResponse != "unauthorized" ? JsonConvert.DeserializeObject<List<UserMappingsVM>>(modelResponse) : null;
        //}

        /// <summary>
        /// Gets Users By Role
        /// </summary>
        /// <param name="HttpClient">Http Client Service</param>
        /// <param name="role">Name of the role</param>
        /// <param name="secure">Add Authentication Header</param>
        /// <returns>List of Users by Role</returns>
        public static async Task<List<UserVM>> GetUsersByRole(IHttpClientService HttpClient, string role, bool secure = true)
        {
            var modelResponse = await HttpClient.GetAsync("Users/GetByRole", secure, role).ConfigureAwait(false);
            return !string.IsNullOrEmpty(modelResponse) && modelResponse != "unauthorized" ? JsonConvert.DeserializeObject<List<UserVM>>(modelResponse) : null;
        }

        /// <summary>
        /// Gets Current Academic Year Id
        /// </summary>
        /// <param name="HttpClient"></param>
        /// <param name="secure"></param>
        /// <returns>Current Academic Year Id or null</returns>
        public static async Task<int> GetCurrentAcademicYearId(IHttpClientService HttpClient, bool secure = false)
        {
            var modelResponse = await HttpClient.GetAsync("AcademicYears/GetDropdown", secure).ConfigureAwait(false);
            var academicYears = !string.IsNullOrEmpty(modelResponse) && modelResponse != "unauthorized" ? JsonConvert.DeserializeObject<List<DropdownVM>>(modelResponse) : null;
            if (academicYears == null || academicYears.Count <= 0) return 0;
            var CurrentYearStr = $"{DateTime.UtcNow.AddHours(5.5).Year}-{DateTime.UtcNow.AddHours(5.5).Year + 1}";
            var AcademicYear = academicYears.FirstOrDefault(x => x.Text == CurrentYearStr);
            return AcademicYear?.Id ?? 0;
        }

        /// <summary>
        /// Checks if an entity already exists having the given property
        /// </summary>
        /// <param name="HttpClient"></param>
        /// <param name="Controller"></param>
        /// <param name="Property"></param>
        /// <param name="Value"></param>
        /// <param name="secure"></param>
        /// <returns></returns>
        public static async Task<bool> CheckExistingProperty(IHttpClientService HttpClient, string Controller, string Property, string Value, bool secure = false)
        {
            var response = await HttpClient.GetAsync($"{Controller}/CheckExisting{Property}", false, Value).ConfigureAwait(false);
            return !string.IsNullOrEmpty(response) && Convert.ToBoolean(response);
        }

        /// <summary>
        /// Gets the Mappings from mid Claim for the given user
        /// </summary>
        /// <param name="User"></param>
        /// <returns>Mappings</returns>
        public static Dictionary<string, List<int>> GetMappings(ClaimsPrincipal User)
        {
            var Mappings = User.Claims.FirstOrDefault(c => c.Type == "mid");
            return Mappings != null ? JsonConvert.DeserializeObject<Dictionary<string, List<int>>>(Mappings.Value) : null;
        }

        /// <summary>
        /// Gets the UserId of the given User
        /// </summary>
        /// <param name="User"></param>
        /// <returns>User Id</returns>
        public static string GetUserId(ClaimsPrincipal User)
        {
            var UserId = User.Claims.FirstOrDefault(c => c.Type == "uid");
            return UserId?.Value;
        }

        /// <summary>
        /// Gets Role for the given User
        /// </summary>uid
        /// <param name="User"></param>
        /// <returns>Role</returns>
        public static string GetUserRole(ClaimsPrincipal User)
        {
            var Role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            return Role?.Value;
        }

        public static string GetUserName(ClaimsPrincipal User)
        {
            var UserName = User.Claims.FirstOrDefault(c => c.Type == "UserName");
            return UserName?.Value;
        }

        public static string GetSellerId(ClaimsPrincipal User)
        {
            var SellerId = User.Claims.FirstOrDefault(c => c.Type == "SellerId");
            return SellerId?.Value;
        }

        public static string GetBuyerId(ClaimsPrincipal User)
        {
            var BuyerId = User.Claims.FirstOrDefault(c => c.Type == "BuyerId");
            return BuyerId?.Value;
        }


        /// <summary>
        /// Gets Nick Name of the given User or "Guest User"
        /// </summary>
        /// <param name="User"></param>
        /// <returns>Name</returns>
        public static string GetUserNickName(ClaimsPrincipal User)
        {
            var NickName = User.Claims.FirstOrDefault(c => c.Type == "nam");
            return NickName?.Value;
        }

        /// <summary>
        /// Gets Profile Image of the given User or "default_user100.png"
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        public static string GetUserProfileImage(ClaimsPrincipal User)
        {
            var ProfileImage = User.Claims.FirstOrDefault(c => c.Type == "img");
            return ProfileImage?.Value;
        }
    }
}