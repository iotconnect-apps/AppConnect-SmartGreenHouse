using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace component.swagger
{
    //public static class CustomStartup
    //{
    //    public static void ConfigureService(this IServiceCollection services)
    //    {
    //        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
    //        services.AddSwaggerGen(options =>
    //        {
    //            options.OperationFilter<ExamplesOperationFilter>();
    //            options.AddSecurityDefinition("Bearer", new ApiKeyScheme { In = "header", Description = "Please enter JWT with Bearer into field", Name = "Authorization", Type = "apiKey" });
    //            options.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> { { "Bearer", Enumerable.Empty<string>() }, });
    //            var xmlpath = Path.Combine(System.AppContext.BaseDirectory, XmlCommentsFilePath);
    //            options.IncludeXmlComments(xmlpath);
    //            options.DescribeAllEnumsAsStrings();
    //            //options.OperationFilter<ExplictObsoleteRoutes>();
    //            //options.SchemaFilter<SwaggerExcludeFilter>();
    //            //options.EnableAnnotations();
    //        });
    //    }

    //    public static void ConfigureServiceWithoutVersion(this IServiceCollection services)
    //    {
    //        services.AddSwaggerGen(options =>
    //        {
    //            options.OperationFilter<ExamplesOperationFilter>();
    //            options.SwaggerDoc("v1.1", new Info { Title = ConfigurationHelper.GetSection<string>("SwashbuckleSpec:Title"), Version = "v1.1" });
    //            options.AddSecurityDefinition("Bearer", new ApiKeyScheme { In = "header", Description = "Please enter JWT with Bearer into field", Name = "Authorization", Type = "apiKey" });
    //            options.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> { { "Bearer", Enumerable.Empty<string>() }, });
    //            var xmlpath = Path.Combine(System.AppContext.BaseDirectory, XmlCommentsFilePath);
    //            options.IncludeXmlComments(xmlpath);
    //            options.DescribeAllEnumsAsStrings();
    //            //options.OperationFilter<ExplictObsoleteRoutes>();
    //            //options.SchemaFilter<SwaggerExcludeFilter>();
    //            //options.EnableAnnotations();
    //        });
    //    }

    //    public static void Configure(this IApplicationBuilder app, IApiVersionDescriptionProvider provider, bool ignoreV1 = true)
    //    {
    //        app.UseSwagger();
    //        app.UseSwaggerUI(options =>
    //        {
    //            foreach (var description in provider.ApiVersionDescriptions)
    //            {
    //                if (description.GroupName.ToUpperInvariant() != "V1" || !ignoreV1)
    //                {
    //                    options.SwaggerEndpoint(
    //                        $"/swagger/{description.GroupName}/swagger.json",
    //                        $"{ConfigurationHelper.GetSection<string>("SwashbuckleSpec:Title")} {description.GroupName.ToUpperInvariant()}");
    //                    options.DocExpansion(DocExpansion.None);
    //                }
    //            }
    //        });
    //    }

    //    public static void ConfigureWithoutVersion(this IApplicationBuilder app)         
    //    {
    //        app.UseSwagger();
    //        app.UseSwaggerUI(options =>
    //        {
    //            options.SwaggerEndpoint("../swagger/v1.1/swagger.json", ConfigurationHelper.GetSection<string>("SwashbuckleSpec:Title"));
    //        });
    //    }

    //    private static string XmlCommentsFilePath
    //    {
    //        get
    //        {
    //            var rootAssembly = Assembly.GetEntryAssembly();
    //            var hostName = rootAssembly.GetName().Name;
    //            return hostName + ".xml";
    //        }
    //    }

    //}

    //public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    //{
    //    readonly IApiVersionDescriptionProvider _provider;

    //    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    //    {
    //        _provider = provider;
    //    }

    //    public void Configure(SwaggerGenOptions options)
    //    {
    //        foreach (var description in _provider.ApiVersionDescriptions)
    //        {
    //            if (description.GroupName.ToUpperInvariant() != "V1")
    //            {
    //                options.SwaggerDoc(
    //              description.GroupName,
    //                new Info()
    //                {
    //                    Title = ConfigurationHelper.GetSection<string>("SwashbuckleSpec:Title"),
    //                    Version = description.ApiVersion.ToString(),
    //                    Description = ConfigurationHelper.GetSection<string>("SwashbuckleSpec:Description")
    //                });
    //            }
    //        }
    //    }

    //    public class ExplictObsoleteRoutes : IOperationFilter
    //    {
    //        public void Apply(Operation operation, OperationFilterContext context)
    //        {
    //            var apiVersion = context.ApiDescription.GetApiVersion();
    //            var model = context.ApiDescription.ActionDescriptor.GetApiVersionModel(ApiVersionMapping.Explicit | ApiVersionMapping.Implicit);
    //            operation.Deprecated = model.DeprecatedApiVersions.Contains(apiVersion);

    //            if (context.ApiDescription.RelativePath.Contains("login"))
    //                operation.Parameters.Add(new NonBodyParameter
    //                {
    //                    Name = "solution-key",
    //                    In = "header",
    //                    Type = "string",
    //                    Required = true
    //                });
    //        }
    //    }
    //}

    //public static class ConfigurationHelper
    //{
    //    public static T GetSection<T>(string key)
    //    {
    //        return _getConfiguration().GetSection(key).Get<T>();
    //    }

    //    public static IConfigurationRoot _getConfiguration()
    //    {
    //        return new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", true, true).Build();

    //    }
    //}

    //public static class ApiVersioningExtention
    //{
    //    public static void AddVersioning(this IServiceCollection services, ApiVersion defaultApiVersion = null)
    //    {
    //        services.AddApiVersioning();
    //        services.AddVersionedApiExplorer(options =>
    //        {
    //            options.GroupNameFormat = "'v'VVV";
    //            options.SubstituteApiVersionInUrl = true;
    //            options.AssumeDefaultVersionWhenUnspecified = false;
    //            options.DefaultApiVersion = defaultApiVersion ?? new ApiVersion(1, 1);
    //        });
    //    }
    //}

    public static class CustomStartup
    {
        public static void ConfigureService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(c =>
            {
                c.OperationFilter<ExplictObsoleteRoutes>();
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Description = @"Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below. \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });

                //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //c.IncludeXmlComments(xmlPath);

                c.SwaggerDoc("v1", new OpenApiInfo { Title = configuration.GetSection("SwashbuckleSpec:Title").Value, Version = "v1" });
            });
        }

        public static void Configure(this IApplicationBuilder app)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                //c.SwaggerEndpoint("../swagger/v1/swagger.json", "Iot Solutions");
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

        }
    }

    public class ExplictObsoleteRoutes : IOperationFilter
    {
        private string[] solutionKeyHeaderRequiredPaths = { "/account/refreshtoken", "/subscriber", "/account/login", "/account/adminlogin" };

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (!solutionKeyHeaderRequiredPaths.Any(x => context.ApiDescription.RelativePath.Contains(x)))
            {
                operation.Parameters.Add(new OpenApiParameter { Name = "company-id", In = ParameterLocation.Header, Required = false });
            }
        }
    }
}

