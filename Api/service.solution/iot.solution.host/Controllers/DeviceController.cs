using iot.solution.entity.Structs.Routes;
using iot.solution.service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using Entity = iot.solution.entity;

namespace host.iot.solution.Controllers
{
    [Route(DeviceRoute.Route.Global)]
    [ApiController]
    public class DeviceController : BaseController
    {
        private readonly IDeviceService _service;

        public DeviceController(IDeviceService deviceEngine)
        {
            _service = deviceEngine;
        }

        [HttpGet]
        [Route(DeviceRoute.Route.GetList, Name = DeviceRoute.Name.GetList)]
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
        [Route(DeviceRoute.Route.GetById, Name = DeviceRoute.Name.GetById)]
        public Entity.BaseResponse<Entity.Device> Get(Guid id)
        {
            if (id == null || id == Guid.Empty)
            {
                return new Entity.BaseResponse<Entity.Device>(false, "Invalid Request");
            }

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
        [Route(DeviceRoute.Route.Manage, Name = DeviceRoute.Name.Add)]
        public Entity.BaseResponse<Entity.Device> Manage([FromBody]Entity.Device request)
        {
            if (request == null || !request.ParentDeviceGuid.HasValue)
            {
                return new Entity.BaseResponse<Entity.Device>(false, "Invalid Request");
            }

            Entity.BaseResponse<Entity.Device> response = new Entity.BaseResponse<Entity.Device>(true);
            try
            {
                if ((request.ParentDeviceGuid.HasValue && request.ParentDeviceGuid != Guid.Empty && (request.TemplateGuid == null || request.TemplateGuid == Guid.Empty)))
                {
                    var parentDevice = _service.Get(request.ParentDeviceGuid.Value);
                    request.TemplateGuid = parentDevice.TemplateGuid;
                    request.GreenHouseGuid = parentDevice.GreenHouseGuid;
                    request.CompanyGuid = parentDevice.CompanyGuid;
                }

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
        [Route(DeviceRoute.Route.Delete, Name = DeviceRoute.Name.Delete)]
        public Entity.BaseResponse<bool> Delete(Guid id)
        {
            if (id == null || id == Guid.Empty)
            {
                return new Entity.BaseResponse<bool>(false, "Invalid Request");
            }

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
        [Route(DeviceRoute.Route.BySearch, Name = DeviceRoute.Name.BySearch)]
        public Entity.BaseResponse<Entity.SearchResult<List<Entity.Device>>> GetBySearch(string searchText = "", int? pageNo = 1, int? pageSize = 10, string orderBy = "")
        {
            Entity.BaseResponse<Entity.SearchResult<List<Entity.Device>>> response = new Entity.BaseResponse<Entity.SearchResult<List<Entity.Device>>>(true);
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
                return new Entity.BaseResponse<Entity.SearchResult<List<Entity.Device>>>(false, ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route(DeviceRoute.Route.UpdateStatus, Name = DeviceRoute.Name.UpdateStatus)]
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
        [Route(DeviceRoute.Route.ChildDevice, Name = DeviceRoute.Name.ChildDevice)]
        public Entity.BaseResponse<Entity.SearchResult<List<Entity.Device>>> GetChildDevice(string parentDeviceGuid = "", int? pageNo = 1, int? pageSize = 10, string orderBy = "")
        {
            Entity.BaseResponse<Entity.SearchResult<List<Entity.Device>>> response = new Entity.BaseResponse<Entity.SearchResult<List<Entity.Device>>>(true);
            try
            {
                response.Data = _service.ChildDeviceList(new Entity.SearchRequest()
                {
                    Guid = parentDeviceGuid,
                    PageNumber = pageNo.Value,
                    PageSize = pageSize.Value,
                    OrderBy = orderBy
                });
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<Entity.SearchResult<List<Entity.Device>>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(DeviceRoute.Route.GetGreenHouseDevices, Name = DeviceRoute.Name.GetGreenHouseDevices)]
        public Entity.BaseResponse<List<Entity.Device>> GetGreenHouseDevices(Guid greenhouseId)
        {
            if (greenhouseId == null || greenhouseId == Guid.Empty)
            {
                return new Entity.BaseResponse<List<Entity.Device>>(false, "Invalid Request");
            }

            Entity.BaseResponse<List<Entity.Device>> response = new Entity.BaseResponse<List<Entity.Device>>(true);
            try
            {
                response.Data = _service.GetGreenHouseDeviceList(greenhouseId);
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<List<Entity.Device>>(false, ex.Message);
            }
            return response;
        }
        [HttpGet]
        [Route(DeviceRoute.Route.GetGreenHouseDevicesDetails, Name = DeviceRoute.Name.GetGreenHouseDevicesDetails)]
        public Entity.BaseResponse<Entity.SearchResult<List<Entity.DeviceDetailResponse>>> GetGreenHouseDevicesDetails(Guid greenhouseId,string searchText = "", int? pageNo = 1, int? pageSize = 10, string orderBy = "")
        {
            Entity.BaseResponse<Entity.SearchResult<List<Entity.DeviceDetailResponse>>> response = new Entity.BaseResponse<Entity.SearchResult<List<Entity.DeviceDetailResponse>>>(true);
            try
            {
                response.Data = _service.GetGreenHouseDeviceDetailList(new Entity.SearchRequest()
                {
                    GreenHouseId = greenhouseId,
                    SearchText = searchText,
                    PageNumber = -1,//pageNo.Value,
                    PageSize =-1,// pageSize.Value,
                    OrderBy = orderBy
                });
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<Entity.SearchResult<List<Entity.DeviceDetailResponse>>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(DeviceRoute.Route.ValidateKit, Name = DeviceRoute.Name.ValidateKit)]
        public Entity.BaseResponse<int> ValidateKit(string kitCode)
        {
            Entity.BaseResponse<int> response = new Entity.BaseResponse<int>(true);
            try
            {
                response = _service.ValidateKit(kitCode);
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<int>(false, ex.Message);
            }
            return response;
        }
        [HttpPost]
        [Route(DeviceRoute.Route.ProvisionKit, Name = DeviceRoute.Name.ProvisionKit)]
        public Entity.BaseResponse<bool> ProvisionKit(Entity.ProvisionKitRequest request)
        {
            Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(true);
            try
            {
                response = _service.ProvisionKit(request);
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<bool>(false, ex.Message);
            }
            return response;
        }
    }
}