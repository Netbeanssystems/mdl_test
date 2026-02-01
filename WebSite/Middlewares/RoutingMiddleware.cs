using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace WebSite.Middlewares
{
    public class RoutingMiddleware
    {
        private RequestDelegate _nextDelegate;

        public RoutingMiddleware(RequestDelegate nextDelegate)
        {
            _nextDelegate = nextDelegate;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            string requestURL = httpContext.Request.Path.ToString().ToLower();

            Boolean result = requestURL.Contains("%7c");



            //redirect domain/ home / index and domain/ home to domain
            if (requestURL.Contains("/Financials.aspx", StringComparison.OrdinalIgnoreCase))
            {
                httpContext.Response.Redirect("/English/Pages/Financial");
            }
            else if (requestURL.Contains("/Quarterly-Reports.aspx", StringComparison.OrdinalIgnoreCase))
            {
                httpContext.Response.Redirect("/English/Pages/Quarterly-Reports");
            }
            else if (requestURL.Contains("/Intimation-to-Stock-Exchanges.aspx", StringComparison.OrdinalIgnoreCase))
            {
                httpContext.Response.Redirect("/English/Pages/Intimation-to-Stock-Exchanges");
            }
            else if (requestURL.Contains("/Newspaper-publications.aspx", StringComparison.OrdinalIgnoreCase))
            {
                httpContext.Response.Redirect("/English/Pages/Newspaper-publications");
            }
            else if (requestURL.Contains("/Annual-Return.aspx", StringComparison.OrdinalIgnoreCase))
            {
                httpContext.Response.Redirect("/English/Pages/Annual-Return");
            }
            else if (requestURL.Contains("/Contact.aspx", StringComparison.OrdinalIgnoreCase))
            {
                httpContext.Response.Redirect("/English/investor");
            }
            else if (requestURL.Contains("/Audio.aspx", StringComparison.OrdinalIgnoreCase))
            {
                httpContext.Response.Redirect("/English/pages/Audio");
            }
            else if (requestURL.Contains("/Transcript.aspx", StringComparison.OrdinalIgnoreCase))
            {
                httpContext.Response.Redirect("/English/pages/Transcript");
            }

            // Temp Redirections
            //else if (requestURL.Contains("/app/MDLJobPortal/Control/Login.aspx", StringComparison.OrdinalIgnoreCase))
            //{
            //    httpContext.Response.Redirect("/Errors/UnderMaintainence");
            //}
            //else if (requestURL.Contains("/app/MDLJobPortal/Login.aspx", StringComparison.OrdinalIgnoreCase))
            //{
            //    httpContext.Response.Redirect("/Errors/UnderMaintainence");
            //}
            //else if (requestURL.Contains("/app/mdlapprentice/Control/Login.aspx", StringComparison.OrdinalIgnoreCase))
            //{
            //    httpContext.Response.Redirect("/Errors/UnderMaintainence");
            //}
            //else if (requestURL.Contains("/app/mdlapprentice/Login.aspx", StringComparison.OrdinalIgnoreCase))
            //{
            //    httpContext.Response.Redirect("/Errors/UnderMaintainence");
            //}
            //else if (requestURL.Contains("/app/EmployeePortal/Control/Login.aspx", StringComparison.OrdinalIgnoreCase))
            //{
            //    httpContext.Response.Redirect("/Errors/UnderMaintainence");
            //}
            //else if (requestURL.Contains("/app/EmployeePortal/Login.aspx", StringComparison.OrdinalIgnoreCase))
            //{
            //    httpContext.Response.Redirect("/Errors/UnderMaintainence");
            //}
            //else if (requestURL.Contains("/app/OnlinePortal/Control/Login.aspx", StringComparison.OrdinalIgnoreCase))
            //{
            //    httpContext.Response.Redirect("/Errors/UnderMaintainence");
            //}
            //else if (requestURL.Contains("/app/Publish_MDL_E_Vendor/welcome.aspx", StringComparison.OrdinalIgnoreCase))
            //{
            //    httpContext.Response.Redirect("/Errors/UnderMaintainence");
            //}
            //else if (requestURL.Contains("/app/MDLJobPortal/Welcome.aspx", StringComparison.OrdinalIgnoreCase))
            //{
            //    httpContext.Response.Redirect("/Errors/UnderMaintainence");
            //}

            //else if (requestURL.Contains("/app/OnlinePortal/Login.aspx", StringComparison.OrdinalIgnoreCase))
            //{
            //    httpContext.Response.Redirect("/Errors/UnderMaintainence");
            //}
            //else if (requestURL.Contains("/app/BalanceConfirmation/Control/Login.aspx", StringComparison.OrdinalIgnoreCase))
            //{
            //    httpContext.Response.Redirect("/Errors/UnderMaintainence");
            //}
            //else if (requestURL.Contains("/app/BalanceConfirmation/Login.aspx", StringComparison.OrdinalIgnoreCase))
            //{
            //    httpContext.Response.Redirect("/Errors/UnderMaintainence");
            //}

            //else if (requestURL.Contains("/app/BillTrackingSystem/Control/Login.aspx", StringComparison.OrdinalIgnoreCase))
            //{
            //    httpContext.Response.Redirect("/Errors/UnderMaintainence");
            //}
            //else if (requestURL.Contains("/app/BillTrackingSystem/Login.aspx", StringComparison.OrdinalIgnoreCase))
            //{
            //    httpContext.Response.Redirect("/Errors/UnderMaintainence");
            //}
            //else if (requestURL.Contains("/app/Publish_MDL_E_Vendor/Control/Index.aspx", StringComparison.OrdinalIgnoreCase))
            //{
            //    httpContext.Response.Redirect("/Errors/UnderMaintainence");
            //}
            //else if (requestURL.Contains("/app/Publish_MDL_E_Vendor/index.aspx?msg=R", StringComparison.OrdinalIgnoreCase))
            //{
            //    httpContext.Response.Redirect("/Errors/UnderMaintainence");
            //}
            //else if (requestURL.Contains("/app/Publish_MDL_E_Vendor/index.aspx?msg=E", StringComparison.OrdinalIgnoreCase))
            //{
            //    httpContext.Response.Redirect("/Errors/UnderMaintainence");
            //}
            //else if (requestURL.Contains("/app/control/Login.aspx", StringComparison.OrdinalIgnoreCase))
            //{
            //    httpContext.Response.Redirect("/Errors/UnderMaintainence");
            //}

            else if (requestURL.Contains("/ITProc-Notification.aspx", StringComparison.OrdinalIgnoreCase))
            {
                httpContext.Response.Redirect("/Errors/PageNotFound");
            }
            else if (requestURL.Contains("/Estate-Notification.aspx", StringComparison.OrdinalIgnoreCase))
            {
                httpContext.Response.Redirect("/Errors/PageNotFound");
            }
            else if (requestURL.Contains("/MDC-Notification.aspx", StringComparison.OrdinalIgnoreCase))
            {
                httpContext.Response.Redirect("/Errors/PageNotFound");
            }
            else if (requestURL.Contains("/InfraProj-Notification.aspx", StringComparison.OrdinalIgnoreCase))
            {
                httpContext.Response.Redirect("/Errors/PageNotFound");
            }
            else if (requestURL.Contains("/TechS-Notification.aspx", StringComparison.OrdinalIgnoreCase))
            {
                httpContext.Response.Redirect("/Errors/PageNotFound");
            }
            else if (requestURL.Contains("/Submarine-Notification.aspx", StringComparison.OrdinalIgnoreCase))
            {
                httpContext.Response.Redirect("/Errors/PageNotFound");
            }
            else if (requestURL.Contains("/SBO-Notification.aspx", StringComparison.OrdinalIgnoreCase))
            {
                httpContext.Response.Redirect("/Errors/PageNotFound");
            }
            else if (requestURL.Contains("/SBPM-Notification.aspx", StringComparison.OrdinalIgnoreCase))
            {
                httpContext.Response.Redirect("/Errors/PageNotFound");
            }
            else if (requestURL.Contains("/IND-Notifications.aspx", StringComparison.OrdinalIgnoreCase))
            {
                httpContext.Response.Redirect("/Errors/PageNotFound");
            }
            else if (result)
            {
                httpContext.Response.Redirect("/Errors/PageNotFound");
            }
            else
            {
                await _nextDelegate.Invoke(httpContext);
            }

        }
    }
}
