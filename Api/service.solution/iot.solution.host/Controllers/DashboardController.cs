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
        public Entity.BaseResponse<List<Entity.LookupItem>> GetFarms(Guid companyId)
        {
            Entity.BaseResponse<List<Entity.LookupItem>> response = new Entity.BaseResponse<List<Entity.LookupItem>>(true);
            try
            {
                response.Data = _service.GetFarmsLookup(companyId);
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<List<Entity.LookupItem>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(DashboardRoute.Route.GetOverview, Name = DashboardRoute.Name.GetOverview)]
        public Entity.BaseResponse<Entity.OverviewResponse> GetOverview(Guid companyId)
        {
            Entity.BaseResponse<Entity.OverviewResponse> response = new Entity.BaseResponse<Entity.OverviewResponse>(true);
            try
            {
                response.Data = _service.GetOverview(companyId);
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<Entity.OverviewResponse>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(DashboardRoute.Route.GetGreenHouseDetail, Name = DashboardRoute.Name.GetGreenHouseDetail)]
        public Entity.BaseResponse<Response.GreenHouseDetailResponse> GetGreenHouseDetail(Guid greenhouseId)
        {
            if (greenhouseId == null || greenhouseId == Guid.Empty)
            {
                return new Entity.BaseResponse<Response.GreenHouseDetailResponse>(false, "Invalid Request");
            }

            Entity.BaseResponse<Response.GreenHouseDetailResponse> response = new Entity.BaseResponse<Response.GreenHouseDetailResponse>(true);
            try
            {
                response.Data = _greenHouseService.GetGreenHouseDetail(greenhouseId);
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<Response.GreenHouseDetailResponse>(false, ex.Message);
            }
            return response;
        }
        [HttpGet]
        [Route(DashboardRoute.Route.GetDeviceDetail, Name = DashboardRoute.Name.GetDeviceDetail)]
        public Entity.BaseResponse<Response.DeviceDetailResponse> GetDeviceDetail(Guid deviceId)
        {
            if (deviceId == null || deviceId == Guid.Empty)
            {
                return new Entity.BaseResponse<Response.DeviceDetailResponse>(false, "Invalid Request");
            }

            Entity.BaseResponse<Response.DeviceDetailResponse> response = new Entity.BaseResponse<Response.DeviceDetailResponse>(true);
            try
            {
                response.Data = _deviceService.GetDeviceDetail(deviceId);
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<Response.DeviceDetailResponse>(false, ex.Message);
            }
            return response;
        }
        [HttpGet]
        [Route(DashboardRoute.Route.GetGreenHouseCorp, Name = DashboardRoute.Name.GetGreenHouseCorp)]
        public Entity.BaseResponse<List<Response.GreenHouseCropResponse>> GetGreenHouseCorps(Guid greenhouseId)
        {
            if (greenhouseId == null || greenhouseId == Guid.Empty)
            {
                return new Entity.BaseResponse<List<Response.GreenHouseCropResponse>>(false, "Invalid Request");
            }

            Entity.BaseResponse<List<Response.GreenHouseCropResponse>> response = new Entity.BaseResponse<List<Response.GreenHouseCropResponse>>(true);
            try
            {
                response.Data = _greenHouseService.GetGreenHouseCorps(greenhouseId);
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<List<Response.GreenHouseCropResponse>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(DashboardRoute.Route.GetGreenHouseDevices, Name = DashboardRoute.Name.GetGreenHouseDevices)]
        public Entity.BaseResponse<List<Response.GreenHouseDevicesResponse>> GetGreenHouseDevices(Guid greenhouseId)
        {
            if (greenhouseId == null || greenhouseId == Guid.Empty)
            {
                return new Entity.BaseResponse<List<Response.GreenHouseDevicesResponse>>(false, "Invalid Request");
            }

            Entity.BaseResponse<List<Response.GreenHouseDevicesResponse>> response = new Entity.BaseResponse<List<Response.GreenHouseDevicesResponse>>(true);
            try
            {
                response.Data = _deviceService.GetGreenHouseDevices(greenhouseId);
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<List<Response.GreenHouseDevicesResponse>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(DashboardRoute.Route.GetGreenHouseChildDevices, Name = DashboardRoute.Name.GetGreenHouseChildDevices)]
        public Entity.BaseResponse<List<Response.GreenHouseDevicesResponse>> GetGreenHouseChildDevices(Guid deviceId)
        {
            if (deviceId == null || deviceId == Guid.Empty)
            {
                return new Entity.BaseResponse<List<Response.GreenHouseDevicesResponse>>(false, "Invalid Request");
            }

            Entity.BaseResponse<List<Response.GreenHouseDevicesResponse>> response = new Entity.BaseResponse<List<Response.GreenHouseDevicesResponse>>(true);
            try
            {
                response.Data = _deviceService.GetGreenHouseChildDevices(deviceId);
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<List<Response.GreenHouseDevicesResponse>>(false, ex.Message);
            }
            return response;
        }
    }
}