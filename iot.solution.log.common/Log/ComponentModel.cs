using System;

namespace component.logger.platform.common.Log
{
    /// <summary>
    /// ComponentModel
    /// </summary>
    public class ComponentModel
    {
        /// <summary>
        /// Gets or sets the component identifier.
        /// </summary>
        /// <value>
        /// The component identifier.
        /// </value>
        public Guid ComponentId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
    }
}
