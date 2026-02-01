using Application.Helpers;
using Application.ServiceInterfaces;
using Application.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
namespace WebAPI.Middlewares
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
            catch (Exception exception)
            {
                if (_config["Environment"].ToString() == "Live") {
                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress(_config["SMTPFrom"]);
                    mail.To.Add(new MailAddress(_config["ErrorEmail"]));
                    mail.IsBodyHtml = true;
                    mail.Subject = "MDL Errors";
                    mail.Body = MessageBuilder.BuildExceptionMessage(context, exception);
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = _config["SMTPHost"];
                    smtp.Send(mail);
                } else if (_config["Environment"].ToString() == "Dev") {
                    //Send email
                    var EmailVm = new EmailVM
                    {
                        ToAddresses = new List<string> { _config["ErrorEmail"] },
                        Subject = "MDL Api Notification",
                        Body = MessageBuilder.BuildExceptionMessage(context, exception)
                    };
                    await _emailService.SendEmailAsync(EmailVm).ConfigureAwait(false);
                }

                //Write to the response
                const int statusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = statusCode;
                await context.Response.WriteAsync(new ApiResponse
                {
                    StatusCode = statusCode,
                    Message = exception.Message,
                    Description = exception.InnerException?.Message
                }.ToString()).ConfigureAwait(false);
            }
        }
    }
}
