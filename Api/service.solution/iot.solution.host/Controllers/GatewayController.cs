using iot.solution.entity.Structs.Routes;
using iot.solution.service.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using Entity = iot.solution.entity;

namespace host.iot.solution.Controllers
{
    [Route(GatewayRoute.Route.Global)]
    public class GatewayController : BaseController
    {
        private readonly IDeviceService _service;
         
        public GatewayController(IDeviceService deviceEngine)
        {
            _service = deviceEngine;
        }
        
        [HttpGet]
        [Route(GatewayRoute.Route.GetList, Name = GatewayRoute.Name.GetList)]
        public Entity.BaseResponse<List<Entity.Device>> Get()
        {
            Entity.BaseResponse<List<Entity.Device>> response = new Entity.BaseResponse<List<Entity.Device>>(true);
            try
            {
                response.Data = _service.Get();
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<List<Entity.Device>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(GatewayRoute.Route.GetById, Name = GatewayRoute.Name.GetById)]
        public Entity.BaseResponse<Entity.Device> Get(Guid id)
        {
            Entity.BaseResponse<Entity.Device> response = new Entity.BaseResponse<Entity.Device>(true);
            try
            {
                response.Data = _service.Get(id);
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<Entity.Device>(false, ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route(GatewayRoute.Route.Manage, Name = GatewayRoute.Name.Add)]
        public Entity.BaseResponse<Entity.Device> Manage([FromBody]Entity.Device request)
        {
            Entity.BaseResponse<Entity.Device> response = new Entity.BaseResponse<Entity.Device>(true);
            try
            {
                var status = _service.Manage(request);
                response.IsSuccess = status.Success;
                response.Message = status.Message;
                response.Data = status.Data;
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<Entity.Device>(false, ex.Message);
            }
            return response;
        }

        [HttpPut]
        [Route(GatewayRoute.Route.Delete, Name = GatewayRoute.Name.Delete)]
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
        [Route(GatewayRoute.Route.BySearch, Name = GatewayRoute.Name.BySearch)]
        public Entity.BaseResponse<Entity.SearchResult<List<Entity.DeviceSearchResponse>>> GetBySearch(string searchText = "", int? pageNo = 1, int? pageSize = 10, string orderBy = "")
        {
            Entity.BaseResponse<Entity.SearchResult<List<Entity.DeviceSearchResponse>>> response = new Entity.BaseResponse<Entity.SearchResult<List<Entity.DeviceSearchResponse>>>(true);
            try
            {
                response.Data = _service.GatewayList(new Entity.SearchRequest()
                {
                    SearchText = searchText,
                    PageNumber = pageNo.Value,
                    PageSize = pageSize.Value,
                    OrderBy = orderBy
                });
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<Entity.SearchResult<List<Entity.DeviceSearchResponse>>>(false, ex.Message);
            }
            return response;
        }

        //[HttpGet]
        //[Route(GatewayRoute.Route.AllChildDevice, Name = GatewayRoute.Name.AllChildDevice)]
        //public Entity.BaseResponse<Entity.SearchResult<List<Entity.Device>>> AllChildDevice(string parentDeviceGuid, string searchText = "", int? pageNo = 1, int? pageSize = 10, string orderBy = "")
        //{
        //    Entity.BaseResponse<Entity.SearchResult<List<Entity.Device>>> response = new Entity.BaseResponse<Entity.SearchResult<List<Entity.Device>>>(true);
        //    try
        //    {
        //        response.Data = _service.AllChildDevice(parentDeviceGuid, new Entity.SearchRequest()
        //        {
        //            SearchText = searchText,
        //            PageNumber = pageNo.Value,
        //            PageSize = pageSize.Value,
        //            OrderBy = orderBy
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return new Entity.BaseResponse<Entity.SearchResult<List<Entity.Device>>>(false, ex.Message);
        //    }
        //    return response;
        //}

        [HttpPost]
        [Route(GatewayRoute.Route.UpdateStatus, Name = GatewayRoute.Name.UpdateStatus)]
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
    }
}