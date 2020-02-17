using component.discovery.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace component.discovery
{
    public static class CustomStartup
    {
        private static readonly string _defaultPath = "/.discovery-configuration";
        public static void ConfigureService(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });
            services.AddSingleton<IRouteInfoProvider, RouteInfoProvider>();
            services.AddSingleton<IApiInfoProvider, ApiInfoProvider>();
        }

        public static void Configure(IApplicationBuilder app)
        {
            Configure(app, _defaultPath);
        }

        public static void Configure(IApplicationBuilder app, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                path = _defaultPath;
            }
            app.MapWhen(ctx => ctx.Request.Path == path, c => c.UseApiInfo());
        }
    }
}
