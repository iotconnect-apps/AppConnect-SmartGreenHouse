using System;
using System.Collections.Generic;

namespace iot.solution.entity
{
    public class ProvisionKitRequest
    {
        public string KitCode { get; set; }
        public Guid GreenHouseGuid { get; set; }
        public List<string> KitDevices { get; set; }
    }
}
