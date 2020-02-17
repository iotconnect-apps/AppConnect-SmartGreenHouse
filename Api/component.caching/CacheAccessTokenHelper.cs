using component.logger;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace component.caching
{
    public class CacheAccessTokenHelper : ICacheAccessTokenHelper
    {
        private readonly string _accessTokenKey = "access_token";
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        public CacheAccessTokenHelper(ILogger logger, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public CacheAccessTokenHelper(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            string accessToken = null;
            if (_httpContextAccessor != null && _httpContextAccessor.HttpContext != null)
                accessToken = await _httpContextAccessor?.HttpContext?.GetTokenAsync(_accessTokenKey);

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                accessToken = await _getClientCredentialsTokenAsync();
            }
            return accessToken;
        }

        private async Task<string> _getClientCredentialsTokenAsync()
        {
            var httpClient = new HttpClient();
            var discoveryResponse = await httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _configuration.GetValue<string>("IdsAuthentication:AuthorityUrl").ToLower(),
                Policy = { RequireHttps = false }
            });

            if (discoveryResponse.IsError)
                throw new System.Exception(discoveryResponse.Error);

            var tokenRequest = new ClientCredentialsTokenRequest
            {
                Address = discoveryResponse.TokenEndpoint,
                ClientId = _configuration.GetValue<string>("IdsAuthentication:ClientAuthentication:ClientId"),
                ClientSecret = _configuration.GetValue<string>("IdsAuthentication:ClientAuthentication:ClientSecret"),
                Scope = _configuration.GetValue<string>("IdsAuthentication:ClientAuthentication:Scopes")
            };
            var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(tokenRequest);

            if (tokenResponse.IsError)
            {
                throw new System.Exception(tokenResponse.Error);
            }
            return tokenResponse.AccessToken;
        }
    }
}
