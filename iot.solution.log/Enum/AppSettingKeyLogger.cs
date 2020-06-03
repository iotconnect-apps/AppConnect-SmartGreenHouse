using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace component.logger.data.log.Enum
{
    public enum AppSettingKeyLogger
    {
        /// <summary>
        /// The from email address
        /// </summary>
        [Description("BlobDomainUrl")]
        BlobDomainUrl,

        /// <summary>
        /// The from email address
        /// </summary>
        [Description("FatalEmailTemplate")]
        FatalEmailTemplate,

        /// <summary>
        /// The fatal email addresses
        /// </summary>
        [Description("FatalEmailAddresses")]
        FatalEmailAddresses,

        /// <summary>
        /// The from email address
        /// </summary>
        [Description("FromEmailAddress")]
        FromEmailAddress,

        /// <summary>
        /// The send grid api key
        /// </summary>
        [Description("SendGridAPIKey")]
        SendGridAPIKey
    }
}
