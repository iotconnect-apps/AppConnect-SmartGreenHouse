using component.logger.data.log.Context;
using component.logger.data.log.Interface;
using component.logger.data.log.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace component.logger.data.log.Repositories
{
    /// <summary>
    /// AppSettingRepository
    /// </summary>
    /// <seealso cref="component.logger.data.log.Interface.IAppSettingRepository" />
    public class AppSettingRepository : IAppSettingRepository
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly LogDataContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppSettingRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <exception cref="System.ArgumentNullException">context</exception>
        public AppSettingRepository(LogDataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Gets the specified application setting keys.
        /// </summary>
        /// <param name="appSettingKeys">The application setting keys.</param>
        /// <returns></returns>
        public async Task<List<AppSetting>> Get(List<string> appSettingKeys)
        {
            return _context.AppSetting
                    .Where(x => x.Key != null && x.Key != string.Empty && appSettingKeys.Contains(x.Key.ToLower()))
                    .Select(x => x).ToList();
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns></returns>
        public async Task<List<AppSetting>> GetAll()
        {
            return _context.AppSetting.Where(x => x.Key != null && x.Key != string.Empty).Select(x => x).ToList();
        }
    }
}
