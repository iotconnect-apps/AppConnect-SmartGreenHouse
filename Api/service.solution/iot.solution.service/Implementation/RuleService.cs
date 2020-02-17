using component.common.model;
using component.common.model.CommonModel;
using component.exception;
using component.helper;
using component.logger;
using iot.solution.model.Repository.Interface;
using iot.solution.service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using Entity = iot.solution.entity;
using Model = iot.solution.model.Models;
using IOT = IoTConnect.Model;

namespace iot.solution.service.Implementation
{
    public class RuleService : IRuleService
    {
        private readonly IotConnectClient _iotConnectClient;
        private readonly ILogger _logger;

        public RuleService(ILogger logger)
        {
            _logger = logger;
            _iotConnectClient = new IotConnectClient(AppConfig.BearerToken, AppConfig.EnvironmentCode, AppConfig.SolutionKey);
        }

        public Entity.ActionStatus Delete(Guid id)
        {
            Entity.ActionStatus status = new Entity.ActionStatus();
            try
            {
                var result = _iotConnectClient.Rule.Delete(id.ToString()).Result;
                if (result != null && !result.status)
                {
                    _logger.Error($"Rule is not deleted from iotconnect");
                    status.Success = false;
                    status.Message = "Something Went Wrong!";
                }
                else
                {
                    status.Success = true;
                    status.Message = "Rule is deleted successfully.";
                }
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "RuleService.Delete " + ex);
                status.Message = ex.Message;
                return status;
            }
            return status;
        }

        public Entity.SingleRuleResponse Get(Guid id)
        {
            Entity.SingleRuleResponse result = null;
            try
            {
                var rule = _iotConnectClient.Rule.Single(Convert.ToString(id)).Result;
                if (rule != null && rule.data != null)
                {
                    result = Mapper.Configuration.Mapper.Map<IoTConnect.Model.SingleRuleResult, Entity.SingleRuleResponse>(rule.data);
                }
            }
            catch (Exception ex)
            {

                _logger.Error(Constants.ACTION_EXCEPTION, "RuleService.Get " + ex);
                return result;
            }
            return result;
        }

        public Entity.SearchResult<List<Entity.AllRuleResponse>> List(Entity.SearchRequest request)
        {
            Entity.SearchResult<List<Entity.AllRuleResponse>> result = new Entity.SearchResult<List<Entity.AllRuleResponse>>();
            try
            {
                var rules = _iotConnectClient.Rule.All(new IOT.PagingModel() { PageNo = request.PageNumber, PageSize = request.PageSize, SearchText = request.SearchText, SortBy = request.OrderBy }).Result;
                if (rules != null && rules.data != null && rules.data.Any())
                {
                    result = new Entity.SearchResult<List<Entity.AllRuleResponse>>()
                    {
                        Items = rules.data.Select(r => Mapper.Configuration.Mapper.Map<Entity.AllRuleResponse>(r)).ToList(),
                        Count = rules.data.Count
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, $"RuleService.List, Error: {ex.Message}");
                return result;
            }
            return result;
        }

        public Entity.ActionStatus Manage(Entity.Rule request)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                if (request.Guid == null || request.Guid == Guid.Empty)
                {
                    var addRuleResult = _iotConnectClient.Rule.Add(Mapper.Configuration.Mapper.Map<IOT.AddRuleModel>(request)).Result;

                    if (addRuleResult != null && !addRuleResult.status)
                    {
                        _logger.Error($"Rule is not added in iotconnect, Error: {addRuleResult.message}");
                        actionStatus.Success = false;
                        actionStatus.Message = "Something Went Wrong!";
                    }
                    else
                    {
                        if(!string.IsNullOrEmpty(addRuleResult.data.ruleGuid))
                        {
                            actionStatus.Data = Get(Guid.Parse(addRuleResult.data.ruleGuid.ToUpper()));
                        }
                    }
                }
                else
                {
                    var updateRuleResult = _iotConnectClient.Rule.Update(request.Guid.ToString(), Mapper.Configuration.Mapper.Map<IOT.UpdateRuleModel>(request)).Result;
                    if (updateRuleResult != null && !updateRuleResult.status)
                    {
                        _logger.Error($"Rule is not added in iotconnect, Error: {updateRuleResult.message}");
                        actionStatus.Success = false;
                        actionStatus.Message = "Something Went Wrong!";
                    }
                    else
                    {
                        actionStatus.Data = Get(request.Guid.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "RuleService.Manage " + ex);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }

        public Entity.ActionStatus UpdateStatus(Guid id, bool status)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                var updateRuleStatusResult = _iotConnectClient.Rule.UpdateRuleStatus(id.ToString(), status).Result;
                if (updateRuleStatusResult != null && !updateRuleStatusResult.status)
                {
                    _logger.Error($"Rule status is not updated in iotconnect, Error: {updateRuleStatusResult.message}");
                    actionStatus.Success = false;
                    actionStatus.Message = new UtilityHelper().IOTResultMessage(updateRuleStatusResult.errorMessages);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "RuleService.UpdateStatus " + ex);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }
    }
}
