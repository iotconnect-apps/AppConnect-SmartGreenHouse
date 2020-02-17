using component.caching;
using component.discoveryclient;
using component.exception;
using component.helper.Interface;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace component.helper
{
    public class DiscoveryHelper : IDiscoveryHelper
    {
        private readonly string _productCode = "all";
        private readonly ICacheManager _cacheManager;

        public DiscoveryHelper(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public async Task<string> GetEndpointUrl(string apiRoute, string apiKey)
        {
            var cacheKey = $"{apiRoute}-{apiKey}";
            var discoveryEndPoint = _cacheManager.Get<string>(_productCode, cacheKey);
            if (!string.IsNullOrEmpty(discoveryEndPoint))
            {
                return discoveryEndPoint;
            }
            var discoveryUrl = "";//await _adminSettingHelper.GetAsync(apiRoute);
            if (string.IsNullOrEmpty(discoveryUrl))
                throw new NotFoundCustomException($"Admin setting not found for Rout: {apiRoute} and Api key : {apiKey}");

            var discoveryModel = DiscoveryClient.Get(discoveryUrl);
            if (discoveryModel.HasError)
                throw new GenericCustomException(discoveryModel.Error);

            if (discoveryModel.Versions == null || !discoveryModel.Versions.Any())
                throw new NotFoundCustomException($"Version not found for URL: {discoveryUrl} Rout: {apiRoute} and Api key : {apiKey}");

            var endpoints = discoveryModel.Versions.FirstOrDefault().Endpoints;
            if (endpoints == null || !endpoints.Any(e => e.Key.Equals(apiKey, StringComparison.InvariantCultureIgnoreCase)))
                throw new NotFoundCustomException($"Endpoint not found for URL: {discoveryUrl} Rout: {apiRoute} and Api key : {apiKey}");

            discoveryEndPoint = endpoints.FirstOrDefault(e => e.Key == apiKey)
                .Endpoint;

            _cacheManager.Add(_productCode, cacheKey, discoveryEndPoint);

            return discoveryEndPoint;
        }
    }
}
