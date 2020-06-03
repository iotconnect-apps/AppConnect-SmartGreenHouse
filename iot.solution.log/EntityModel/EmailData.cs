using System;
using System.Collections.Generic;
using System.Text;

namespace component.logger.data.log.EntityModel
{
    public class EmailData
    {
        public string FromEmailId { get; set; }
        public string ToEmailId { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string SendGridAPIKey { get; set; }
        public string FromName { get; set; }
    }
}
