using host.iot.solution.Filter;
using iot.solution.entity.Structs.Routes;
using iot.solution.service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using Entity = iot.solution.entity;
using Response = iot.solution.entity.Response;

namespace host.iot.solution.Controllers
{
    [Route(DashboardRoute.Route.Global)]
    [ApiController]
    public class DashboardController : BaseController
    {
        private readonly IDashboardService _service;
        private readonly IGreenHouseService _greenHouseService;
        private readonly IDeviceService _deviceService;

        public DashboardController(IDashboardService dashboardService, IGreenHouseService greenHouseService, IDeviceService deviceService)
        {
            _service = dashboardService;
            _greenHouseService = greenHouseService;
            _deviceService = deviceService;
        }

        [HttpGet]
        [Route(DashboardRoute.Route.GetFarms, Name = DashboardRoute.Name.GetFarms)]
        [EnsureGuidParameterAttribute("companyId", "Company")]
        public Entity.BaseResponse<List<Entity.LookupItem>> GetFarms(string companyId)
        {
            Entity.BaseResponse<List<Entity.LookupItem>> response = new Entity.BaseResponse<List<Entity.LookupItem>>(true);
            try
            {
                response.Data = _service.GetFarmsLookup(Guid.Parse(companyId));
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<List<Entity.LookupItem>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(DashboardRoute.Route.GetOverview, Name = DashboardRoute.Name.GetOverview)]
        public Entity.BaseResponse<Entity.DashboardOverviewResponse> GetOverview()
        {
           Entity.BaseResponse<Entity.DashboardOverviewResponse> response = new Entity.BaseResponse<Entity.DashboardOverviewResponse>(true);
            try
            {
                response.Data = _service.GetOverview();
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<Entity.DashboardOverviewResponse>();
            }
            return response;
        }

        [HttpGet]
        [Route(DashboardRoute.Route.GetGreenHouseDetail, Name = DashboardRoute.Name.GetGreenHouseDetail)]
        [EnsureGuidParameterAttribute("greenhouseId","Greenhouse")]
        public Entity.BaseResponse<Response.GreenHouseDetailResponse> GetGreenHouseDetail(string greenhouseId)
        {
            if (greenhouseId == null || greenhouseId == string.Empty)
            {
                return new Entity.BaseResponse<Response.GreenHouseDetailResponse>(false, "Invalid Request");
            }

            Entity.BaseResponse<Response.GreenHouseDetailResponse> response = new Entity.BaseResponse<Response.GreenHouseDetailResponse>(true);
            try
            {
                response = _greenHouseService.GetGreenHouseDetail(Guid.Parse(greenhouseId));
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<Response.GreenHouseDetailResponse>(false, ex.Message);
            }
            return response;
        }
        [HttpGet]
        [Route(DashboardRoute.Route.GetDeviceDetail, Name = DashboardRoute.Name.GetDeviceDetail)]
        [EnsureGuidParameterAttribute("deviceId", "Device")]
        public Entity.BaseResponse<Response.DeviceDetailResponse> GetDeviceDetail(string deviceId)
        {
            if (deviceId == null || deviceId == string.Empty)
            {
                return new Entity.BaseResponse<Response.DeviceDetailResponse>(false, "Invalid Request");
            }

            Entity.BaseResponse<Response.DeviceDetailResponse> response = new Entity.BaseResponse<Response.DeviceDetailResponse>(true);
            try
            {
                response = _deviceService.GetDeviceDetail(Guid.Parse(deviceId));
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<Response.DeviceDetailResponse>(false, ex.Message);
            }
            return response;
        }
        [HttpGet]
        [Route(DashboardRoute.Route.GetGreenHouseCorp, Name = DashboardRoute.Name.GetGreenHouseCorp)]
        [EnsureGuidParameterAttribute("greenhouseId", "Greenhouse")]
        public Entity.BaseResponse<List<Response.GreenHouseCropResponse>> GetGreenHouseCorps(string greenhouseId)
        {
            if (greenhouseId == null || greenhouseId == string.Empty)
            {
                return new Entity.BaseResponse<List<Response.GreenHouseCropResponse>>(false, "Invalid Request");
            }

            Entity.BaseResponse<List<Response.GreenHouseCropResponse>> response = new Entity.BaseResponse<List<Response.GreenHouseCropResponse>>(true);
            try
            {
                response.Data = _greenHouseService.GetGreenHouseCorps(Guid.Parse(greenhouseId));
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<List<Response.GreenHouseCropResponse>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(DashboardRoute.Route.GetGreenHouseDevices, Name = DashboardRoute.Name.GetGreenHouseDevices)]
        [EnsureGuidParameterAttribute("greenhouseId", "Greenhouse")]
        public Entity.BaseResponse<List<Response.GreenHouseDevicesResponse>> GetGreenHouseDevices(string greenhouseId)
        {
            if (greenhouseId == null || greenhouseId == string.Empty)
            {
                return new Entity.BaseResponse<List<Response.GreenHouseDevicesResponse>>(false, "Invalid Request");
            }

            Entity.BaseResponse<List<Response.GreenHouseDevicesResponse>> response = new Entity.BaseResponse<List<Response.GreenHouseDevicesResponse>>(true);
            try
            {
                response.Data = _deviceService.GetGreenHouseDevices(Guid.Parse(greenhouseId));
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<List<Response.GreenHouseDevicesResponse>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(DashboardRoute.Route.GetGreenHouseChildDevices, Name = DashboardRoute.Name.GetGreenHouseChildDevices)]
        [EnsureGuidParameterAttribute("deviceId", "Device")]
        public Entity.BaseResponse<List<Response.GreenHouseDevicesResponse>> GetGreenHouseChildDevices(string deviceId)
        {
            if (deviceId == null || deviceId == string.Empty)
            {
                return new Entity.BaseResponse<List<Response.GreenHouseDevicesResponse>>(false, "Invalid Request");
            }

            Entity.BaseResponse<List<Response.GreenHouseDevicesResponse>> response = new Entity.BaseResponse<List<Response.GreenHouseDevicesResponse>>(true);
            try
            {
                response.Data = _deviceService.GetGreenHouseChildDevices(Guid.Parse(deviceId));
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<List<Response.GreenHouseDevicesResponse>>(false, ex.Message);
            }
            return response;
        }
    }
}