using component.helper.Interface;
using component.logger;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Response = iot.solution.entity.Response;
using Entity = iot.solution.entity;

namespace component.helper
{
    public class SubscriberHelper : ISubscriberHelper
    {
        private readonly IHttpClientHelper _httpClientHelper;
        private static string _subcriptionAccessToken;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        string apiURL = String.Empty;

        public SubscriberHelper(ILogger logger, IConfiguration configuration)
        {
            _logger = logger;
            _httpClientHelper = new HttpClientHelper(logger);
            _configuration = configuration;
            apiURL = _configuration.GetSection("SubscriptionAPI").GetSection("BaseUrl").Value;
            InitSubscriptionToken();
        }
        public bool ValidateSubscriptionAccessToken()
        {
            if (!string.IsNullOrWhiteSpace(_subcriptionAccessToken))
            {
                JwtSecurityTokenHandler jwthandler = new JwtSecurityTokenHandler();
                Microsoft.IdentityModel.Tokens.SecurityToken jwttoken = jwthandler.ReadToken(_subcriptionAccessToken);

                if (jwttoken.ValidTo < DateTime.UtcNow.AddMinutes(5))
                    return false;
                else
                    return true;
            }

            return false;
        }
        public void InitSubscriptionToken()
        {
            if (!ValidateSubscriptionAccessToken())
            {
                Entity.TokenRequest request = new Entity.TokenRequest
                {
                    ClientID = _configuration.GetSection("SubscriptionAPI").GetSection("ClientID").Value,
                    ClientSecret = _configuration.GetSection("SubscriptionAPI").GetSection("ClientSecret").Value,
                    UserName = _configuration.GetSection("SubscriptionAPI").GetSection("UserName").Value
                };
                Entity.TokenResponse response = new Entity.TokenResponse();
                response = _httpClientHelper.Post<Entity.TokenRequest, Entity.TokenResponse>(apiURL + "subscription/token", request);
                _subcriptionAccessToken = response.accessToken;
            }
        }
        public Response.TimezoneResponse GetTimezoneData()
        {
            return _httpClientHelper.Get<Response.TimezoneResponse>(apiURL + "timezone?pageNo=1&pageSize=200", _subcriptionAccessToken);
        }
        public Response.CountryResponse GetCountryData()
        {
            return _httpClientHelper.Get<Response.CountryResponse>(apiURL + "country?pageNo=1&pageSize=1000", _subcriptionAccessToken);
        }
        public Response.StateResponse GetStateData(string countryID)
        {
            return _httpClientHelper.Get<Response.StateResponse>(apiURL + "state?pageNo=1&pageSize=1000" + "&countryID=" + countryID, _subcriptionAccessToken);
        }
        public Entity.SaveCompanyResponse CreateCompany(Entity.SaveCompanyRequest requestData)
        {
            return _httpClientHelper.Post<Entity.SaveCompanyRequest, Entity.SaveCompanyResponse>(string.Concat(apiURL, "solution/company"), requestData, _subcriptionAccessToken);
        }
        public Response.SubscriptionPlanResponse GetSubscriptionPlans(string solutionID)
        {
            return _httpClientHelper.Get<Response.SubscriptionPlanResponse>(string.Format("{0}solution/{1}/plans?pageNo=1&pageSize=1000", apiURL, solutionID), _subcriptionAccessToken);
        }
        public Entity.SearchResult<List<Entity.SubscriberData>> SubscriberList(string solutionID, Entity.SearchRequest request)
        {
            Entity.SearchResult<List<Entity.SubscriberData>> result = new Entity.SearchResult<List<Entity.SubscriberData>>();
            Entity.Subscriber response = _httpClientHelper.Get<Entity.Subscriber>(string.Format("{0}solution/subscriber?pageNo={2}&pageSize={3}&displayDataOf=0&productCode={1}", apiURL, solutionID,request.PageNumber,request.PageSize), _subcriptionAccessToken);
            result.Items = response.data;
            result.Count = response.@params.count;
            return result;
        }
        public Entity.SubsciberCompanyDetails GetSubscriberDetails(string solutionCode, string userEmail)
        {
            return _httpClientHelper.Get<Entity.SubsciberCompanyDetails>(string.Format("{0}subscriber/{1}/{2}/consumption/active", apiURL, solutionCode, userEmail), _subcriptionAccessToken);
        }
    }
}
