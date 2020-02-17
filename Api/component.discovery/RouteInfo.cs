using component.discovery.Interface;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Versioning;
using System.Collections.Generic;
using System.Linq;

namespace component.discovery
{
    public class RouteInfo : IRouteInfo
    {
        public string Name { get; private set; }
        public string Template { get; private set; }
        public string HttpMethod { get; private set; }
        public string Version { get; private set; }

        internal static IRouteInfo Create(ActionDescriptor ad) => new RouteInfo
        {
            Name = ad.AttributeRouteInfo.Name,
            Template = ad.AttributeRouteInfo.Template,
            HttpMethod = ad
                    .ActionConstraints
                    .OfType<HttpMethodActionConstraint>()
                    .SelectMany(c => c.HttpMethods).FirstOrDefault(),
            Version = $"{_getMajorVersion(ad.Properties)}.{_getMinorVersion(ad.Properties)}"
        };

        private static string _getMajorVersion(IDictionary<object, object> properties)
        {
            foreach (var item in properties.Where(p => typeof(ApiVersionModel).Equals(p.Key) && p.Value != null))
            {
                var apiVersion = ((ApiVersionModel)item.Value);
                if (apiVersion.IsApiVersionNeutral == false && apiVersion.ImplementedApiVersions.Any() && apiVersion.ImplementedApiVersions[0].MajorVersion.HasValue)
                    return apiVersion.ImplementedApiVersions[0].MajorVersion.Value.ToString();
            }
            return string.Empty;
        }

        private static string _getMinorVersion(IDictionary<object, object> properties)
        {

            foreach (var item in properties.Where(p => typeof(ApiVersionModel).Equals(p.Key) && p.Value != null))
            {
                var apiVersion = ((ApiVersionModel)item.Value);
                if (apiVersion.IsApiVersionNeutral == false && apiVersion.ImplementedApiVersions.Any() && apiVersion.ImplementedApiVersions[0].MinorVersion.HasValue)
                    return apiVersion.ImplementedApiVersions[0].MinorVersion.Value.ToString();
            }

            return string.Empty;
        }
    }
}
