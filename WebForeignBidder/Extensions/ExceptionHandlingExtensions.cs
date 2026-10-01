using Microsoft.AspNetCore.Builder;
using WebForeignBidder.Middlewares;

namespace WebForeignBidder.Extensions
{
    public static class ExceptionHandlingExtensions
    {
        public static IApplicationBuilder UseCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
