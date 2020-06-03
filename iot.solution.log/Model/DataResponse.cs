using component.logger.data.log.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace component.logger.data.log.Model
{
    /// <summary>
    /// DataResponse
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataResponse<T>
    {
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public DataResponseStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public T Data { get; set; }

        /// <summary>
        /// Gets or sets the extension.
        /// </summary>
        /// <value>
        /// The extension.
        /// </value>
        public Dictionary<string, object> Extension { get; set; }
    }
}
