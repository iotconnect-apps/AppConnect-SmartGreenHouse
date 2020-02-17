using Microsoft.AspNetCore.Builder;

namespace component.errorhandler
{
    public static class CustomStartup
    {
        public static void Configure(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalExceptionHandler>();
        }
    }
}
