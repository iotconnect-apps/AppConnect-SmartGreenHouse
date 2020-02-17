using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace component.swashbuckle
{
    public class CustomStartup
    {
        private static ApiAuthentication _apiAuthentication;
        private static ApiVersion _apiVersion;

        private static string XmlCommentsFilePath
        {
            get
            {
                var rootAssembly = Assembly.GetEntryAssembly();
                var hostName = rootAssembly.GetName().Name;
                return hostName + ".xml";
            }
        }

        public static void ConfigureService(IServiceCollection services, bool IsAuthEnable = true)
        {
            _apiAuthentication = GetSection<ApiAuthentication>("SwashbuckleSpec:Authentication");
            _apiVersion = GetSection<ApiVersion>("SwashbuckleSpec:Version");
            services.AddVersionedApiExplorer(
                option =>
                {
                    option.GroupNameFormat = "'v'VVV";
                    option.SubstituteApiVersionInUrl = true;
                });

            services.AddApiVersioning(option => option.ReportApiVersions = true);

            services.AddSwaggerGen(s =>
            {

                s.SwaggerDoc(_apiVersion.VersionName, new Swashbuckle.AspNetCore.Swagger.Info()
                {
                    Title = _apiVersion.Title,
                    Version = _apiVersion.VersionNumber,
                    Description = _apiVersion.Description
                });
                s.OperationFilter<SwashbuckleDefaultValues>();
                s.OperationFilter<FileUploadOperation>();

                var filePath = Path.Combine(System.AppContext.BaseDirectory, XmlCommentsFilePath);
                s.IncludeXmlComments(filePath);

                if (IsAuthEnable)
                {
                    var seurity = new Dictionary<string, IEnumerable<string>> { { "oauth2", new string[] { } } };
                    s.AddSecurityDefinition("oauth2", new OAuth2Scheme
                    {
                        Type = "oauth2",
                        Flow = "implicit",
                        AuthorizationUrl = _apiAuthentication.AuthorityUrl + "/connect/authorize",
                        Scopes = _apiAuthentication.Scopes,
                    });
                    s.AddSecurityRequirement(seurity);
                }
            });
        }

        public static void Configure(IApplicationBuilder app, bool IsAuthEnable = true)
        {
            app.UseSwagger();
            app.UseSwaggerUI(s =>
            {
                s.SwaggerEndpoint(string.Format("{0}/swagger/{1}/swagger.json", string.IsNullOrEmpty(s.RoutePrefix) ? "." : "..", _apiVersion.VersionName), _apiVersion.Title + " - " + _apiVersion.VersionNumber);
                if (IsAuthEnable)
                {
                    s.OAuthClientId(_apiAuthentication.ClientId);
                    s.OAuthRealm(_apiAuthentication.Realm);
                    s.OAuthAppName(_apiAuthentication.ApiName);
                    if (_apiAuthentication.AdditionalQueryStringParams != null && _apiAuthentication.AdditionalQueryStringParams.Any())
                    {
                        s.OAuthAdditionalQueryStringParams(_apiAuthentication.AdditionalQueryStringParams);
                    }
                }
            });

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
