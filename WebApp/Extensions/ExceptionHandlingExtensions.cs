using Microsoft.AspNetCore.Builder;
using WebApp.Middlewares;

namespace WebApp.Extensions
{
    public static class ExceptionHandlingExtensions
    {
        public static IApplicationBuilder UseCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
