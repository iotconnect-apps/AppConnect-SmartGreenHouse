using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace component.logger
{
    public static class CustomStartup
    {
        public static void ConfigureService(IServiceCollection services)
        {
            services.AddTransient<ILogger, Logger>();
        }

        public static void Configure(IServiceCollection services)
        {
            services.AddLogging(loggingBuilder => loggingBuilder.AddNLog());
        }

        public static void Configure(this ILoggerFactory loggerFactiry)
        {
            loggerFactiry.AddNLog();
        }
    }
}
