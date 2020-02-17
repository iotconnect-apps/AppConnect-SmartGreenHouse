using component.discovery.Interface;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace component.discovery
{
    public class ApiInfoMiddleware
    {
        private readonly IApiInfoProvider _provider;
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public ApiInfoMiddleware(
            RequestDelegate _,
            IApiInfoProvider provider,
            JsonSerializerSettings jsonSerializerSettings)
        {
            _provider = provider;
            _jsonSerializerSettings = jsonSerializerSettings;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var apiInfo = (ApiInfo)_provider.Get();

            DiscoveryInfo discoveryInfo = new DiscoveryInfo();
            discoveryInfo.BaseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.PathBase}";
            discoveryInfo.Time = apiInfo.Time;
            discoveryInfo.Versions = _processData(apiInfo.Routes.OfType<RouteInfo>().ToList(), discoveryInfo.BaseUrl);

            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsync(
                JsonConvert.SerializeObject(discoveryInfo, _jsonSerializerSettings));
        }

        private List<VersionInfo> _processData(List<RouteInfo> routeInfos, string baseUrl)
        {
            var versions = routeInfos.Select(x => x.Version).Distinct().ToList();
            var versionInfos = new List<VersionInfo>();

            foreach (var item in versions)
            {
                versionInfos.Add(new VersionInfo()
                {
                    Version = item,
                    Endpoints = routeInfos.Where(x => x.Version == item).Select(a => new EndpointInfo()
                    {
                        Endpoint = $"{baseUrl}/{a.Template}",
                        HttpMethod = a.HttpMethod,
                        Key = a.Name
                    }).ToList()
                });
            }

            return versionInfos;
        }

    }
}
