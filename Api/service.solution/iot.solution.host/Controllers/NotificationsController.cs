using iot.solution.entity.Structs.Routes;
using iot.solution.service.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using Entity = iot.solution.entity;
using host.iot.solution.Controllers;

namespace iot.solution.host.Controllers
{
    [Route(NotificationsRoute.Route.Global)]
    [ApiController]
    public class NotificationsController : BaseController
    {
        private readonly INotificationsService _service;

        public NotificationsController(INotificationsService notificationsService)
        {
            _service = notificationsService;
        }

        [HttpPost]
        [Route(NotificationsRoute.Route.Manage, Name = NotificationsRoute.Name.Add)]
        public Entity.BaseResponse<Entity.SingleRuleResponse> Manage(Entity.NotificationAddRequest request)
        {
            Entity.BaseResponse<Entity.SingleRuleResponse> response = new Entity.BaseResponse<Entity.SingleRuleResponse>(true);
            try
            {
                var status = _service.Manage(request);
                response.IsSuccess = status.Success;
                response.Message = status.Message;
                response.Data = status.Data;
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<Entity.SingleRuleResponse>(false, ex.Message);
            }
            return response;
        }

        [HttpPut]
        [Route(NotificationsRoute.Route.Delete, Name = NotificationsRoute.Name.Delete)]
        public Entity.BaseResponse<bool> Delete(Guid id)
        {
            Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(true);
            try
            {
                var status = _service.Delete(id);
                response.IsSuccess = status.Success;
                response.Message = status.Message;
                response.Data = status.Success;
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<bool>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(NotificationsRoute.Route.BySearch, Name = NotificationsRoute.Name.BySearch)]
        public Entity.BaseResponse<Entity.SearchResult<List<Entity.AllRuleResponse>>> GetBySearch(string searchText = "")
        {
            Entity.BaseResponse<Entity.SearchResult<List<Entity.AllRuleResponse>>> response = new Entity.BaseResponse<Entity.SearchResult<List<Entity.AllRuleResponse>>>(true);
            try
            {
                response.Data = _service.List(new Entity.SearchRequest()
                {
                    SearchText = searchText
                });
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<Entity.SearchResult<List<Entity.AllRuleResponse>>>(false, ex.Message);
            }
            return response;
        }
        [HttpPost]
        [Route(NotificationsRoute.Route.UpdateStatus, Name = NotificationsRoute.Name.UpdateStatus)]
        public Entity.BaseResponse<bool> UpdateStatus(Guid id, bool status)
        {
            Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(true);
            try
            {
                Entity.ActionStatus result = _service.UpdateStatus(id, status);
                response.IsSuccess = result.Success;
                response.Message = result.Message;
                response.Data = result.Success;
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<bool>(false, ex.Message);
            }
            return response;
        }
        [HttpGet]
        [Route(NotificationsRoute.Route.GetById, Name = NotificationsRoute.Name.GetById)]
        public Entity.BaseResponse<Entity.SingleRuleResponse> Get(Guid id)
        {
            Entity.BaseResponse<Entity.SingleRuleResponse> response = new Entity.BaseResponse<Entity.SingleRuleResponse>(true);
            try
            {
                response.Data = _service.Get(id);
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<Entity.SingleRuleResponse>(false, ex.Message);
            }
            return response;
        }
    }
}