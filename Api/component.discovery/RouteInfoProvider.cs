using component.discovery.Interface;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Linq;

namespace component.discovery
{
    public class RouteInfoProvider : IRouteInfoProvider
    {
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

        public RouteInfoProvider(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
        }

        public IRouteInfo[] Get() => _actionDescriptorCollectionProvider
            .ActionDescriptors
            .Items
            .Where(ad => ad.AttributeRouteInfo != null)
            .Select(RouteInfo.Create).ToArray();
    }
}
