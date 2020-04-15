using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace host.iot.solution
{
    public static class AddCorsMiddlewareExtension
    {
        public static IServiceCollection AddCorsMiddleware
            (this IServiceCollection services, IConfiguration configuration)
        {
            var allowedOrigins = configuration.GetSection("AppSettings:AllowOrigins").Get<string>();
            services.AddCors(x => x.AddPolicy("AllowSpecificOrigin", new CorsPolicy
            {
                Origins = { allowedOrigins },
                Headers = { "*" },
                Methods = { "*" },
                SupportsCredentials = false
            }));
            return services;
        }

        public static IApplicationBuilder UseCorsMiddleware(this IApplicationBuilder app)
        {
            return app.UseCors("AllowSpecificOrigin");
        }
    }
}
