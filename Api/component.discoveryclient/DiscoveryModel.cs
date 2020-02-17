using System;
using System.Collections.Generic;

namespace component.discoveryclient
{
    public class DiscoveryModel
    {
        public DateTime Time { get; set; }
        public string BaseUrl { get; set; }
        public List<VersionModel> Versions { get; set; }
        public bool HasError { get; set; }
        public string Error { get; set; }
    }
}
