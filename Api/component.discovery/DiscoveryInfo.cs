using System;
using System.Collections.Generic;

namespace component.discovery
{
    public class DiscoveryInfo
    {
        public DateTime Time { get; set; }
        public string BaseUrl { get; set; }
        public List<VersionInfo> Versions { get; set; }
    }
}
