using System.Collections.Generic;

namespace component.discoveryclient
{
    public class VersionModel
    {
        public string Version { get; set; }
        public List<EndpointModel> Endpoints { get; set; }
    }
}
