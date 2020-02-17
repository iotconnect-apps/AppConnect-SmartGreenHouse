using AutoMapper;
using engine.iot.solution.Data;
using engine.iot.solution.Interface;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace host.iot.solution.Extension
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddContext(this IServiceCollection services)
        {
            //services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            //return services;
            return null;
        }
        public static void AddAutoMapper(this IServiceCollection services)
        {
            //Mapper.Initialize(cfg => { cfg.AddProfile<EngineMapperProfile>(); });
        }

        public static IServiceCollection AddEngine(this IServiceCollection services)
        {
            services.AddTransient<IUserManager,IUserManager>();
            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            //services.AddTransient<IConfigurationService, ConfigurationService>();
            //return services;
            return null;
        }

        public static IServiceCollection AddAccess(this IServiceCollection services)
        {
            //services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
            //return services;
            return null;
        }

        public static IServiceCollection AddSessionToken(this IServiceCollection services)
        {
            //services.AddTransient<ISessionTokenHandler, SessionTokenHandler>();
            //return services;
            return null;
        }
    }
}
