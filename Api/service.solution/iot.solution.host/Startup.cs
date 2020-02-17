using component.cors;
using component.logger;
using iot.solution.service.IocConfig;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mapper = iot.solution.service.Mapper;
using host.iot.solution.Middleware;
using Microsoft.Extensions.FileProviders;
using System.IO;
using component.common.model;

namespace host.iot.solution
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            component.common.model.AppConfig.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCorsMiddleware(Configuration);

            services.AddAuthentication(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration.GetValue<string>("Token:Issuer").ToLower(),
                    ValidAudience = Configuration.GetValue<string>("Token:Audience").ToLower(),
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(Configuration.GetValue<string>("Token:SecurityKey")))
                };
            });

            Mapper.Configuration.Initialize();
            component.swagger.CustomStartup.ConfigureService(services, Configuration);
            component.logger.CustomStartup.ConfigureService(services);
            ConfigureServicesCollection(services);
            ConfigureMessaging(services);
            services.AddControllers();
        }

        //public IServiceProvider ConfigureServices(IServiceCollection services)
        //{
        //    return ConfigureServicesCollection(services);
        //}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCorsMiddleware();
            component.swagger.CustomStartup.Configure(app);
            component.errorhandler.CustomStartup.Configure(app);
            loggerFactory.Configure();

            //app.UseHttpsRedirection();
            app.UseStaticFiles(); // For the wwwroot folder
            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    FileProvider = new PhysicalFileProvider(
            //        Path.Combine(Directory.GetCurrentDirectory()+"//"+AppConfig.UploadBasePath.TrimEnd('/'), AppConfig.CropImageBasePath.TrimEnd('/'))),
            //    RequestPath = "/"+AppConfig.CropImageBasePath.TrimEnd('/')
            //});

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHeaderkeyAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void ConfigureServicesCollection(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            //services.AddScoped<IUnitOfWork, UnitOfWork>();
            //services.AddScoped<DbContext, qagreenhouseContext>();
            IocConfigurations.Initialize(services);
        }

        private void ConfigureMessaging(IServiceCollection services)
        {
            component.messaging.CustomStartup.AddIOTConnectSyncManager(services, Configuration.GetValue<string>("ConnectionStrings:DefaultConnection"),
                Configuration.GetValue<string>("Messaging:ServicebusEndPoint"),
                Configuration.GetValue<string>("Messaging:TopicName"),
                Configuration.GetValue<string>("Messaging:SubscriptionName"));
        }
    }
}
