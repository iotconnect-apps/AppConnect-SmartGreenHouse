using component.caching;
using component.exception;
using component.logger;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;


namespace component.helper
{
    public class AccessTokenHelper : Interface.IAccessTokenHelper
    {
        private readonly string _productCode = "all";
        private readonly string _accessTokenKey = "access_token";
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICacheManager _cacheManager;
        private readonly IConfiguration _configuration;
        public AccessTokenHelper(ILogger logger, IHttpContextAccessor httpContextAccessor, ICacheManager cacheManager, IConfiguration configuration)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _cacheManager = cacheManager;
            _configuration = configuration;
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
            var token = string.Empty;
            var accessTokenCacheKey = _configuration.GetValue<string>("IdsAuthentication:ClientAuthentication:AccessTokenCacheKey").ToUpper();
            if (!string.IsNullOrEmpty(accessTokenCacheKey))
            {
                token = _cacheManager.Get<string>(_productCode, accessTokenCacheKey);
                if (!string.IsNullOrEmpty(token) && _validatetoken(token))
                {
                    return token;
                }
            }

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

            token = tokenResponse.AccessToken;
            if (!string.IsNullOrEmpty(accessTokenCacheKey) && !string.IsNullOrEmpty(token))
            {
                _cacheManager.Add<string>(_productCode, accessTokenCacheKey, token);
            }
            return token;
        }

        private bool _validatetoken(string token)
        {
            var httpClient = new HttpClient();
            var request = new TokenIntrospectionRequest
            {
                Address = $"{_configuration.GetValue<string>("IdsAuthentication:AuthorityUrl").ToLower()}/connect/introspect",
                ClientId = _configuration.GetValue<string>("IdsAuthentication:ClientAuthentication:ApiName"),
                ClientSecret = _configuration.GetValue<string>("IdsAuthentication:ClientAuthentication:ApiSecret"),
                Token = token
            };

            var response = httpClient.IntrospectTokenAsync(request).Result;
            if (response == null || response.IsError)
            {
                _logger.Error(response.Error);
                throw new UnauthorizedCustomException(response.Error);
            }
            return response.IsActive;
        }
    }
}
