using iot.solution.entity.Structs.Routes;
using iot.solution.service.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using Entity = iot.solution.entity;

namespace host.iot.solution.Controllers
{
    [Route(RuleRoute.Route.Global)]
    public class RuleController : BaseController
    {
        private readonly IRuleService _service;

        public RuleController(IRuleService ruleService)
        {
            _service = ruleService;
        }

        [HttpGet]
        [Route(RuleRoute.Route.GetById, Name = RuleRoute.Name.GetById)]
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

        [HttpPost]
        [Route(RuleRoute.Route.Manage, Name = RuleRoute.Name.Manage)]
        public Entity.BaseResponse<Entity.SingleRuleResponse> Manage([FromBody]Entity.Rule request)
        {
            if (request == null)
            {
                return new Entity.BaseResponse<Entity.SingleRuleResponse>(false, "Invalid Request");
            }

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

        [HttpGet]
        [Route(RuleRoute.Route.List, Name = RuleRoute.Name.List)]
        public Entity.BaseResponse<Entity.SearchResult<List<Entity.AllRuleResponse>>> GetBySearch(string searchText = "", int? pageNo = 1, int? pageSize = 10, string orderBy = "")
        {
            Entity.BaseResponse<Entity.SearchResult<List<Entity.AllRuleResponse>>> response = new Entity.BaseResponse<Entity.SearchResult<List<Entity.AllRuleResponse>>>(true);
            try
            {
                response.Data = _service.List(new Entity.SearchRequest()
                {
                    SearchText = searchText,
                    PageNumber = pageNo.Value,
                    PageSize = pageSize.Value,
                    OrderBy = orderBy
                });
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<Entity.SearchResult<List<Entity.AllRuleResponse>>>(false, ex.Message);
            }
            return response;
        }

        [HttpPut]
        [Route(RuleRoute.Route.Delete, Name = RuleRoute.Name.Delete)]
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

        //[HttpPost]
        //[Route(RuleRoute.Route.UpdateStatus, Name = RuleRoute.Name.UpdateStatus)]
        //public Entity.BaseResponse<bool> UpdateStatus(Guid id, bool status)
        //{
        //    Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(true);
        //    try
        //    {
        //        Entity.ActionStatus result = _service.UpdateStatus(id, status);
        //        response.IsSuccess = result.Success;
        //        response.Message = result.Message;
        //        response.Data = result.Success;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new Entity.BaseResponse<bool>(false, ex.Message);
        //    }
        //    return response;
        //}

    }
}