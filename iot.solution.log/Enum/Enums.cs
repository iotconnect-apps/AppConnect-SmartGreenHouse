using System;
using System.Collections.Generic;
using System.Text;

namespace component.logger.data.log.Enum
{
    public enum DataResponseStatus
    {
        /// <summary>
        /// The success
        /// </summary>
        Success = 1,

        /// <summary>
        /// The already exists
        /// </summary>
        AlreadyExists = -1,

        /// <summary>
        /// The no data found
        /// </summary>
        NoDataFound = -2,

        /// <summary>
        /// The invaliddata
        /// </summary>
        Invaliddata = -3,

        /// <summary>
        /// The can not delete
        /// </summary>
        CanNotDelete = -4,

        /// <summary>
        /// The duplicate data
        /// </summary>
        DuplicateData = -5
    }
}
