using Microsoft.AspNetCore.Builder;

namespace component.discovery
{
    public static class ApiInfoMiddlewareExtensions
    {
        public static IApplicationBuilder UseApiInfo(this IApplicationBuilder builder)
        => builder.UseMiddleware<ApiInfoMiddleware>();
    }
}
