using component.discovery.Interface;
using System;

namespace component.discovery
{
    public class ApiInfo : IApiInfo
    {
        public string Version { get; }
        public DateTime Time { get; }
        public IRouteInfo[] Routes { get; }

        public ApiInfo(DateTime time, IRouteInfo[] routes)
        {
            Time = time;
            Routes = routes;
        }
    }
}
