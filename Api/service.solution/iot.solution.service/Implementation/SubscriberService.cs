using System;
using System.Collections.Generic;
using component.helper;
using component.helper.Interface;
using iot.solution.service.Interface;
using Entity = iot.solution.entity;
using Response = iot.solution.entity.Response;
using Request = iot.solution.entity.Request;
using component.logger;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using System.Linq;
using iot.solution.model.Repository.Interface;
using Model = iot.solution.model.Models;

namespace iot.solution.service.Implementation
{
    public class SubscriberService : ISubscriberService
    {
        private readonly IHttpClientHelper _httpClientHelper;
        private readonly ILogger _logger;
        private readonly ISubscriberHelper _subscriberHelper;
        private readonly IConfiguration _configuration;
        private readonly IHardwareKitRepository _hardwareKitRepository;
        private readonly ICompanyRepository _companyRepository;

        public SubscriberService(ILogger logger, IConfiguration configuration, IHardwareKitRepository hardwareKitRepository, ICompanyRepository companyRepository)
        {
            _logger = logger;
            _subscriberHelper = new SubscriberHelper(logger, configuration);
            _httpClientHelper = new HttpClientHelper(logger);
            _configuration = configuration;
            _hardwareKitRepository = hardwareKitRepository;
            _companyRepository = companyRepository;
        }
        public Response.CountryResponse GetCountryLookUp()
        {
            Response.CountryResponse response = _subscriberHelper.GetCountryData();
            return response;
        }
        public Response.StateResponse GetStateLookUp(string countryID)
        {
            Response.StateResponse response = _subscriberHelper.GetStateData(countryID);
            return response;
        }
        public Response.TimezoneResponse GetTimezoneLookUp()
        {
            Response.TimezoneResponse response = _subscriberHelper.GetTimezoneData();
            return response;
        }
        public Response.SubscriptionPlanResponse GetSubscriptionPlans(string solutionID)
        {
            Response.SubscriptionPlanResponse response = _subscriberHelper.GetSubscriptionPlans(solutionID);

            return response;
        }
        public Entity.ActionStatus SaveCompany(Entity.SaveCompanyRequest requestData)
        {
            Entity.ActionStatus response = new Entity.ActionStatus(true);
            try
            {
                Entity.SaveCompanyResponse saveResult = _subscriberHelper.CreateCompany(requestData);
                if (saveResult != null && saveResult.PaymentTransactionId != null)
                {
                    response.Data = saveResult;

                    Model.Company dbCompany = new Model.Company()
                    {
                        Guid = Guid.Parse(saveResult.IoTConnectCompanyGuid),
                        //GreenHouseGuid = Guid.Parse(saveResult.IoTConnectCompanyGuid),
                        //CpId
                        Name = requestData.User.CompanyName,
                        ContactNo = requestData.User.Phone,
                        Address = requestData.User.Address,
                        CountryGuid = Guid.Parse(requestData.User.CountryId),
                        TimezoneGuid = Guid.Parse(requestData.User.TimezoneId),
                        StateGuid = Guid.Parse(requestData.User.StateId),
                        City = requestData.User.CityName,
                        PostalCode = requestData.User.PostalCode
                    };
                    Entity.ActionStatus upStatus = _companyRepository.UpdateDetails(dbCompany);

                    response.Success = true;
                    response.Message = "";
                }
                else
                {
                    response.Data.Data = saveResult;
                    response.Success = false;
                    response.Message = "Something Went Wrong!";
                }
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "SubscriberManager.SaveCompany " + ex);
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
        public Entity.SearchResult<List<Entity.SubscriberData>> SubscriberList(string solutionID, Entity.SearchRequest request)
        {
            try
            {
                var result = _subscriberHelper.SubscriberList(solutionID, request);
                return new Entity.SearchResult<List<Entity.SubscriberData>>()
                {
                    Items = result.Items.Select(p => Mapper.Configuration.Mapper.Map<Entity.SubscriberData>(p)).ToList(),
                    Count = result.Count
                };
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "SubscriberService.List Error:" + ex);
                return new Entity.SearchResult<List<Entity.SubscriberData>>();
            }
        }
        public Entity.SubsciberCompanyDetails GetSubscriberDetails(string solutionCode, string userEmail)
        {
            return _subscriberHelper.GetSubscriberDetails(solutionCode, userEmail);
        }
        public Entity.SearchResult<List<Entity.HardwareKitResponse>> GetSubscriberKitDetails(string companyId, Entity.SearchRequest request, bool isAssigned)
        {
            var result = _hardwareKitRepository.List(request, isAssigned, companyId);

            return new Entity.SearchResult<List<Entity.HardwareKitResponse>>()
            {
                Items = result.Items.Select(p => Mapper.Configuration.Mapper.Map<Entity.HardwareKitResponse>(p)).ToList(),
                Count = result.Count
            };
        }
    }
}
