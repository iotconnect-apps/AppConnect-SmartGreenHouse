using System.Collections.Generic;

namespace iot.solution.entity
{
    public class HardwareKitRequest
    {
        public string KitCode { get; set; }
        public List<KitDeviceRequest> KitDevices { get; set; }
    }



}
