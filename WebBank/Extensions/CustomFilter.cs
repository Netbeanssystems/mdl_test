using Application.Dtos;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Shyjus.BrowserDetection;

namespace WebBank.Extensions
{
    public class CustomFilter
    {
    }
    public class AuthorizeActionFilter : IAuthorizationFilter
    {

        private readonly IHttpClientService _httpClient;
        private readonly IFileService _fileService;
        private readonly ICommon _common;
        private readonly IBrowserDetector _browserDetector;
        private readonly IHttpContextAccessor _httpContext;
        public AuthorizeActionFilter(IBrowserDetector browserDetector, IHttpContextAccessor httpContext, IHttpClientService httpClient, IFileService fileService, ICommon common)
        {
            _httpClient = httpClient;
            _fileService = fileService;
            _common = common;
            _browserDetector = browserDetector;
            _httpContext = httpContext;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string userId = _httpContext.HttpContext.Session.GetString("Id");
            if (userId != null)
            {
                CheckIpAndBrowser(context, userId);
            }
            else
            {
                context.HttpContext.Session.Clear();
                context.HttpContext.Session.Remove(".AspNetCore.Session");
                foreach (var cookie in context.HttpContext.Request.Cookies.Keys)
                    context.HttpContext.Response.Cookies.Delete(cookie);
                context.HttpContext.Response.Redirect("/Account/Login");
            }
        }
        private async void CheckIpAndBrowser(AuthorizationFilterContext context, string userId)
        {

            string CurrentIP = context.HttpContext.Connection.RemoteIpAddress.ToString();
            var Currentbrowser = _browserDetector.Browser.Name;
            string Result = "Invalid";
            var SessionResult = await _httpClient.GetAsync("SessionManage/GetuserId", false, userId).ConfigureAwait(false);
            SessionManageDTO obj = new SessionManageDTO();
            obj = !string.IsNullOrEmpty(SessionResult) ? JsonConvert.DeserializeObject<SessionManageDTO>(SessionResult) : null;
            //obj = JsonConvert.DeserializeObject<SessionManageDTO>(SessionResult);
            //................Check If User Stored BrowserName and Ip is Same for Current IP and BrowserName........
            if (Currentbrowser == obj.BrowserName && CurrentIP == obj.IPAddress)
                Result = "Valid";
            if (Result == "Invalid")
            {
                context.HttpContext.Session.Clear();
                context.HttpContext.Session.Remove(".AspNetCore.Session");
                foreach (var cookie in context.HttpContext.Request.Cookies.Keys)
                    context.HttpContext.Response.Cookies.Delete(cookie);
                context.HttpContext.Response.Redirect("/Account/Login");
            }
            else
            {

            }



        }

        public class SessionManageAttribute : TypeFilterAttribute
        {
            public SessionManageAttribute() : base(typeof(AuthorizeActionFilter)) { }
        }
    }
}

//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;
//using System;
//using Shyjus.BrowserDetection;
//using Microsoft.AspNetCore.Http;
//using Application.Helpers;
//using Application.Dtos;
//using System.Net.Http;
//using AutoMapper.Configuration;
//using Application.ServiceInterfaces;
//using Newtonsoft.Json;
//using System.Threading.Tasks;

//namespace WebBank.Extensions
//{
//    public class CustomFilter
//    {
//    }
//    public class AuthorizeActionFilter : IAuthorizationFilter
//    {

//        private readonly IHttpClientService _httpClient;
//        private readonly IHttpContextAccessor _httpContextAccessor;
//        private readonly IConfiguration _config;
//        private readonly IHttpClientFactory httpClientFactory;
//        private readonly IBrowserDetector _browserDetector;
//        private readonly IHttpContextAccessor _httpContext;
//        public AuthorizeActionFilter(IHttpClientService httpClient,
//            IHttpContextAccessor httpContextAccessor,
//            IConfiguration config,
//            IHttpClientFactory _httpClientFactory, IBrowserDetector browserDetector, IHttpContextAccessor httpContext)
//        {
//            _browserDetector = browserDetector;
//            _httpContext = httpContext;
//            _httpClient = httpClient;
//            _httpContextAccessor = httpContextAccessor;
//            _config = config;
//            httpClientFactory = _httpClientFactory;
//        }
//        public void OnAuthorization(AuthorizationFilterContext context)
//        {
//            string userId = _httpContext.HttpContext.Session.GetString("Id");
//            if (userId != null)
//            {
//                var CheckIpAndBrowser = "";
//                // Logic for checking the browser and ip goes here.  
//                //if (!CheckIpAndBrowser(context, userId))
//                if (!CheckIpAndBrowser(context, userId))
//                {
//                    context.HttpContext.Session.Clear();
//                    context.HttpContext.Session.Remove(".AspNetCore.Session");
//                    foreach (var cookie in context.HttpContext.Request.Cookies.Keys)
//                        context.HttpContext.Response.Cookies.Delete(cookie);
//                    context.HttpContext.Response.Redirect("/Index/Home");
//                }
//            }
//            else
//            {
//                context.HttpContext.Session.Clear();
//                context.HttpContext.Session.Remove(".AspNetCore.Session");
//                foreach (var cookie in context.HttpContext.Request.Cookies.Keys)
//                    context.HttpContext.Response.Cookies.Delete(cookie);
//                context.HttpContext.Response.Redirect("/Index/Home");
//            }
//        }
//        // private bool CheckIpAndBrowser(AuthorizationFilterContext context, string userId)
//        public async Task<string> CheckIpAndBrowser(AuthorizationFilterContext context, string userId)
//        {
//            string CurrentIP = context.HttpContext.Connection.RemoteIpAddress.ToString();
//            var Currentbrowser = _browserDetector.Browser.Name;
//            bool Result = false;
//            var SessionResult = await _httpClient.GetAsync("SessionManage/GetbyuserId", false, userId).ConfigureAwait(false);
//            SessionManageDTO obj = new SessionManageDTO();
//            obj = JsonConvert.DeserializeObject<SessionManageDTO>(SessionResult);
//            //................Check If User Stored BrowserName and Ip is Same for Current IP and BrowserName........
//            if (Currentbrowser == obj.BrowserName && CurrentIP == obj.IPAddress)
//                Result = true;
//            return SessionResult;
//        }

//        public class SessionManageAttribute : TypeFilterAttribute
//        {
//            public SessionManageAttribute() : base(typeof(AuthorizeActionFilter)) { }
//        }
//    }
//}
