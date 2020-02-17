using component.discovery.Interface;
using System;

namespace component.discovery
{
    public class ApiInfoProvider : IApiInfoProvider
    {
        private readonly Func<DateTime> _timeFn;
        private readonly Lazy<IRouteInfo[]> _routeInfoFn;

        public ApiInfoProvider(IRouteInfoProvider routeInfoProvider)
        {
            _timeFn = () => DateTime.Now;
            _routeInfoFn = new Lazy<IRouteInfo[]>(routeInfoProvider.Get);
        }

        public IApiInfo Get() => new ApiInfo(
            _timeFn(),
            _routeInfoFn.Value);

    }
}