using System;

namespace component.caching
{
    public class CachePollInfo
    {
        public string ProductCode { get; set; }
        public DateTime NextCacheRefresh { get; set; }
    }
}
