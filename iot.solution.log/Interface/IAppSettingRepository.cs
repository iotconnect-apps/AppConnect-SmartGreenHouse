using component.logger.data.log.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace component.logger.data.log.Interface
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAppSettingRepository
    {
        /// <summary>
        /// Gets the specified application setting keys.
        /// </summary>
        /// <param name="appSettingKeys">The application setting keys.</param>
        /// <returns></returns>
        Task<List<AppSetting>> Get(List<string> appSettingKeys);

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns></returns>
        Task<List<AppSetting>> GetAll();
    }
}
