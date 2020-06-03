using System;
using System.Collections.Generic;
using System.Text;

namespace component.logger.data.log
{
    public class AppJsonConfig
    {
        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public static Microsoft.Extensions.Configuration.IConfigurationRoot Configuration { get; set; }
    }
}
