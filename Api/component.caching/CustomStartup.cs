using Microsoft.Extensions.DependencyInjection;

namespace component.caching
{
    public static class CustomStartup
    {
        public static void ConfigureService(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton<ICacheAccessTokenHelper, CacheAccessTokenHelper>();
            services.AddSingleton<ICacheManager, CacheManager>();
        }
    }
}
