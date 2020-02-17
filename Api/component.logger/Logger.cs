using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace component.logger
{
    public class Logger : ILogger
    {
        private readonly ILogger<Logger> _logger;

        public Logger(ILogger<Logger> logger)
        {
            _logger = logger;
        }

        public async Task Error(string actiontext, string error)
        {
            await Task.Run(() => _logger.LogError(error));
        }

        public async Task Error(string actiontext, System.Exception ex)
        {
            await Task.Run(() => _logger.LogError(ex.Message));
        }

        public async Task Error(string error)
        {
            await Task.Run(() => _logger.LogError(error));
        }

        public async Task Information(string information)
        {
            await Task.Run(() => _logger.LogInformation(information));
        }

        public async Task Information(string text, string information)
        {
            await Task.Run(() => _logger.LogInformation(information));
        }

        public async Task Trace(string message)
        {
            await Task.Run(() => _logger.LogTrace(message));
        }
    }
}
