using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace WebSite.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;
        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            IEmailService emailService,
            IConfiguration config
            )
        {
            _next = next;
            _emailService = emailService;
            _config = config;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context).ConfigureAwait(false);
            }
            catch (Exception)
            {
                ////Send email
                //var EmailVm = new EmailVM
                //{
                //    ToAddresses = new List<string> { _config["ErrorEmail"] },
                //    Subject = "MDL Site Notification",
                //    Body = MessageBuilder.BuildExceptionMessage(context, exception)
                //};
                //await _emailService.SendEmailAsync(EmailVm).ConfigureAwait(false);

                //Redirect to Error Page
                context.Response.Redirect("/Errors/Exception");
            }
        }
    }
}
