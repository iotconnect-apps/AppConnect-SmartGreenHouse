using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace component.authentication
{
    public class CustomStartup
    {
        public static void ConfigureService(IServiceCollection services)
        {
            services.AddSession();
            var idsAuthentication = GetSection<IdsAuthentication>("IdsAuthentication");

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme).AddIdentityServerAuthentication(o =>
            {
                o.Authority = idsAuthentication.AuthorityUrl;
                o.ApiName = idsAuthentication.ApiAuthentication.ApiName;
                o.ApiSecret = idsAuthentication.ApiAuthentication.ApiSecret;
                o.EnableCaching = idsAuthentication.ApiAuthentication.EnableCaching;
                o.CacheDuration = TimeSpan.FromMinutes(idsAuthentication.ApiAuthentication.CacheDuration);
                o.RequireHttpsMetadata = idsAuthentication.ApiAuthentication.RequireHttpsMetadata;
            });
        }

        public static void Configure(IApplicationBuilder app)
        {
            app.UseAuthentication();
        }

        private static T GetSection<T>(string key)
        {
            return _getConfiguration().GetSection(key).Get<T>();
        }

        private static IConfigurationRoot _getConfiguration()
        {
            return new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", true, true).Build();
        }
    }
}
