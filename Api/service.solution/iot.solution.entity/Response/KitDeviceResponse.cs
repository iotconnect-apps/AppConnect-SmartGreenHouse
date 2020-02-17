using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity.Response
{
    public class KitDeviceResponse : KitDevice
    {
        public Guid TemplateGuid { get; set; }
    }
}
