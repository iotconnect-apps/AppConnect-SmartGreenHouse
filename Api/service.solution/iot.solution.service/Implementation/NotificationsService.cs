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
using iot.solution.entity;

namespace iot.solution.service.Data
{
    public class NotificationsService : INotificationsService
    {
        private readonly INotificationsRepository _inotificationsrepository;
        private readonly IotConnectClient _iotConnectClient;
        private readonly ILogger _logger;

        public NotificationsService(INotificationsRepository notificationsRepository, ILogger logger)
        {
            _logger = logger;
            _inotificationsrepository = notificationsRepository;
            _iotConnectClient = new IotConnectClient(AppConfig.BearerToken, AppConfig.EnvironmentCode, AppConfig.SolutionKey);
        }

        public Entity.SearchResult<List<Entity.AllRuleResponse>> List(Entity.SearchRequest request)
        {
            Entity.SearchResult<List<Entity.AllRuleResponse>> result = new SearchResult<List<AllRuleResponse>>();
            try
            {
                IOT.DataResponse<List<IOT.AllRuleResult>> rules = _iotConnectClient.Rule.All(new IoTConnect.Model.PagingModel() { PageNo = 1, PageSize = 1000 }).Result;
                if (rules.status && rules.data != null)
                {
                    result = new Entity.SearchResult<List<Entity.AllRuleResponse>>()
                    {
                        Items = rules.data.Select(p => Mapper.Configuration.Mapper.Map<Entity.AllRuleResponse>(p)).ToList(),
                        Count = rules.data.Count
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, $"NotificationsService.List, Error: {ex.Message}");
                return new Entity.SearchResult<List<Entity.AllRuleResponse>>();
            }
            return result;
        }

        public Entity.ActionStatus Manage(Entity.NotificationAddRequest request)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            var _list = new List<string>();
            try
            {
                if (request.Guid == null || request.Guid == Guid.Empty)
                {
                    
                    _list.Add("Email");
                    request.DeliveryMethod = _list;
                    var addRuleResult = _iotConnectClient.Rule.Add(Mapper.Configuration.Mapper.Map<IOT.AddRuleModel>(request)).Result;

                    if (addRuleResult != null && !addRuleResult.status)
                    {
                        _logger.Error($"Notification is not added in iotconnect, Error: {addRuleResult.message}");
                        actionStatus.Success = false;
                        actionStatus.Message = addRuleResult.errorMessages[0].Message;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(addRuleResult.data.ruleGuid))
                        {
                            actionStatus.Data = Get(Guid.Parse(addRuleResult.data.ruleGuid.ToUpper()));
                        }
                    }
                }
                else
                {
                    _list.Add("Email");
                    request.DeliveryMethod = _list;
                    var updateRuleResult = _iotConnectClient.Rule.Update(request.Guid.ToString(), Mapper.Configuration.Mapper.Map<IOT.UpdateRuleModel>(request)).Result;
                    if (updateRuleResult != null && !updateRuleResult.status)
                    {
                        _logger.Error($"Notification is not updated in iotconnect, Error: {updateRuleResult.message}");
                        actionStatus.Success = false;
                        actionStatus.Message = updateRuleResult.errorMessages[0].Message;
                    }
                    else
                    {
                        actionStatus.Data = Get(request.Guid.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "NotificationService.Manage " + ex);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }
        public Entity.ActionStatus Delete(Guid id)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                var singleRule = _iotConnectClient.Rule.Single(id.ToString()).Result;

                if (singleRule.data != null && singleRule.status)
                {
                    var deleteRule = _iotConnectClient.Rule.Delete(id.ToString()).Result;

                    if (deleteRule.status)
                    {
                        actionStatus.Success = deleteRule.status;
                        actionStatus.Message = deleteRule.message;
                    }
                    else
                        throw new NotFoundCustomException(deleteRule.errorMessages[0].Message);
                }
                else
                    throw new NotFoundCustomException(singleRule.errorMessages[0].Message);
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "Notification.Delete " + ex);
                return new Entity.ActionStatus
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            return actionStatus;
        }
        public Entity.ActionStatus UpdateStatus(Guid id, bool status)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                var singleRule = _iotConnectClient.Rule.Single(id.ToString()).Result;
                if (singleRule == null)
                {
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : Notification");
                }
                else
                {
                    var iotRuleDetail = _iotConnectClient.Rule.UpdateRuleStatus(id.ToString(), status).Result;
                    actionStatus.Success = iotRuleDetail.status;
                    actionStatus.Message = iotRuleDetail.message;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "Notification.UpdateStatus " + ex);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }
        public Entity.SingleRuleResponse Get(Guid id)
        {
            Entity.SingleRuleResponse result = null;
            try
            {
                var response = _iotConnectClient.Rule.Single(Convert.ToString(id)).Result;
                if (response != null && response.data != null)
                {
                    result = Mapper.Configuration.Mapper.Map<IoTConnect.Model.SingleRuleResult, Entity.SingleRuleResponse>(response.data);
                }
            }
            catch (Exception ex)
            {

                _logger.Error(Constants.ACTION_EXCEPTION, "NotificationService.Get " + ex);
                return result;
            }
            return result;
        }

    }
}