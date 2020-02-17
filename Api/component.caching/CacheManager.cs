using component.logger;
using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace component.caching
{
    public class CacheManager : ICacheManager
    {

        private readonly string allCacheKey = "all";
        private string _adminApiRoot;
        private readonly string _flushCacheAction = "/api/cache/getbyproductcode";
        private readonly string _getAdminKeyAction = "/api/adminsetting/key/{key}";
        private DateTime _lastPollingTime = DateTime.MinValue;
        private bool _expirationValidationInProcess;
        private int _pollingMinutes = 5; // default cache polling interval

        private readonly IMemoryCache _memoryCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        private readonly ICacheAccessTokenHelper _accessTokenHelper;
        private readonly IConfiguration _configuration;

        public CacheManager(IMemoryCache memoryCache, IHttpContextAccessor httpContextAccessor, ILogger logger, IConfiguration configuration, ICacheAccessTokenHelper accessTokenHelper)
        {
            _memoryCache = memoryCache;
            _httpContextAccessor = httpContextAccessor;
            _accessTokenHelper = accessTokenHelper;
            _logger = logger;
            _configuration = configuration;
            _initializeCacheManager();
        }

        public bool Add<T>(string productCode, string key, T value)
        {
            if (!_validateApplicationKeyAndKey(productCode, key))
            {
                return false;
            }
            string cacheKey = _getCacheKey(productCode, key);
            var cancellationTokenSource = _memoryCache.Get<CancellationTokenSource>(productCode);
            if (cancellationTokenSource == null)
            {
                cancellationTokenSource = new CancellationTokenSource();
                _memoryCache.Set(productCode, cancellationTokenSource);
            }

            var allCancellationTokenSource = _memoryCache.Get<CancellationTokenSource>(allCacheKey);
            if (allCancellationTokenSource == null)
            {
                allCancellationTokenSource = new CancellationTokenSource();
                _memoryCache.Set(allCacheKey, allCancellationTokenSource);
            }

            var existingCache = _memoryCache.Get<T>(cacheKey);
            if (existingCache != null)
            {
                _memoryCache.Remove(existingCache);
            }
            var cacheOptions = new MemoryCacheEntryOptions();
            cacheOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));
            cacheOptions.AddExpirationToken(new CancellationChangeToken(allCancellationTokenSource.Token));
            cacheOptions.RegisterPostEvictionCallback(EvictionCallback, this);

            _memoryCache.Set(cacheKey, value, cacheOptions);

            return true;
        }

        private void EvictionCallback(object key, object value, EvictionReason reason, object state)
        {
            var message = $"'{key}':'{value}' was evicted because: {reason}";
            _logger.Error(message);
        }

        public T Get<T>(string productCode, string key)
        {
            _cacheExpirationValidation(productCode);

            string cacheKey = _getCacheKey(productCode, key);
            return _memoryCache.Get<T>(cacheKey);
        }

        public bool Remove(string productCode, string key)
        {
            string cacheKey = _getCacheKey(productCode, key);
            _memoryCache.Remove(productCode);
            return true;
        }

        public bool Flush(string productCode)
        {
            if (string.IsNullOrEmpty(productCode))
            {
                productCode = allCacheKey;
            }
            var cancellationTokenSource = _memoryCache.Get<CancellationTokenSource>(productCode);
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                _memoryCache.Remove(productCode);
            }
            return true;
        }

        private void _initializeCacheManager()
        {
            _adminApiRoot = _configuration.GetValue<string>("AdminSetting:ApiUrl");

            var cachePollInterval = _configuration.GetValue<int>("AdminSetting:CachePollInterval");
            if (cachePollInterval > 0)
                _pollingMinutes = cachePollInterval;
        }

        private string _getCacheKey(string productCode, string key)
        {
            return string.Format("{0}-{1}", productCode, key);
        }

        private bool _validateApplicationKeyAndKey(string productCode, string key)
        {
            if (string.IsNullOrWhiteSpace(productCode) || string.IsNullOrWhiteSpace(key))
            {
                return false;
            }
            return true;
        }

        private bool _cacheExpirationValidation(string productCode)
        {
            if (_lastPollingTime.AddMinutes(_pollingMinutes).CompareTo(DateTime.Now) > 0)
            {
                return true;
            }
            if (_expirationValidationInProcess)
            {
                _expirationValidationInProcess = false;
                return true;
            }
            _expirationValidationInProcess = true;

            var response = _isCacheExpired(productCode, _lastPollingTime);
            if (response != null && response.FirstOrDefault().Value)
            {
                _lastPollingTime = DateTime.Now;
                _expirationValidationInProcess = false;
                return true;
            }
            if (response != null && response.FirstOrDefault().Key == allCacheKey)
            {
                productCode = allCacheKey;
            }

            Flush(productCode);

            _lastPollingTime = DateTime.Now;
            _expirationValidationInProcess = false;

            return true;
        }

        private Dictionary<string, bool> _isCacheExpired(string productCode, DateTime lastCacheRefreshTime)
        {
            var dictionayData = new Dictionary<string, bool>();

            var response = _getLastPollInfo(productCode);
            if (response == null)
            {
                dictionayData.Add(string.Empty, true);
                return dictionayData;
            }
            var responseProductCode = response.ProductCode ?? string.Empty;
            if (response.NextCacheRefresh.CompareTo(DateTime.MinValue) > 0)
            {
                if (response.NextCacheRefresh.AddHours(10).CompareTo(DateTime.Now) < 0)
                {
                    dictionayData.Add(responseProductCode, true);
                    return dictionayData;
                }
                if (response.NextCacheRefresh.CompareTo(lastCacheRefreshTime) >= 0)
                {
                    dictionayData.Add(responseProductCode, false);
                    return dictionayData;
                }
                dictionayData.Add(responseProductCode, true);
                return dictionayData;
            }
            dictionayData.Add(responseProductCode, true);
            return dictionayData;
        }

        private CachePollInfo _getLastPollInfo(string productCode)
        {
            var accessToken = _accessTokenHelper.GetAccessTokenAsync().Result;
            string url = $"{_adminApiRoot}{_flushCacheAction}?productCode={productCode}";
            return _get<CachePollInfo>(url, accessToken);
        }

        private string _getCachePollInterval()
        {
            var accessToken = _accessTokenHelper.GetAccessTokenAsync().Result;
            string url = $"{_adminApiRoot}{_getAdminKeyAction.Replace("{key}", "CachePollInterval")}";
            return _get<string>(url, accessToken);
        }

        private T _get<T>(string url, string token, Dictionary<string, string> requestHeaderKeyValue = null)
        {
            using (var httpClient = new HttpClient())
            {
                if (!string.IsNullOrWhiteSpace(token))
                {
                    httpClient.SetBearerToken(token);
                }

                if (requestHeaderKeyValue != null && requestHeaderKeyValue.Count > 0)
                {
                    foreach (var item in requestHeaderKeyValue)
                    {
                        httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }
                }

                using (var response = httpClient.GetAsync(url).Result)
                {
                    if ((int)response.StatusCode == (int)HttpStatusCode.OK)
                    {
                        if (typeof(T) == typeof(string))
                            return (T)(object)response.Content.ReadAsStringAsync().Result;
                        return JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);
                    }
                    _logger.Error($"Http client request failed with status code : {response.StatusCode.ToString()} for url : {url}");
                }
            }
            return default(T);
        }
    }
}
