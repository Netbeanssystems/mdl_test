using Microsoft.AspNetCore.Builder;
using WebBank.Middlewares;

namespace WebBank.Extensions
{
    public static class ExceptionHandlingExtensions
    {
        public static IApplicationBuilder UseCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
