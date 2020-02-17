using System.Collections.Generic;

namespace component.discovery
{
    public class VersionInfo
    {
        public string Version { get; set; }
        public List<EndpointInfo> Endpoints { get; set; }
    }
}
