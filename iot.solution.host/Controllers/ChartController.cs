using iot.solution.entity.Structs.Routes;
using iot.solution.service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using Entity = iot.solution.entity;
using Response = iot.solution.entity.Response;
using Request = iot.solution.entity.Request;

namespace host.iot.solution.Controllers
{
    [Route(ChartRoute.Route.Global)]
    [ApiController]
    public class ChartController : BaseController
    {
        private readonly IChartService _chartService;
        
        public ChartController(IChartService chartService)
        {
            _chartService = chartService;
        }

        #region IOS Application
        [HttpPost]
        [Route(ChartRoute.Route.EnergyUsage, Name = ChartRoute.Name.EnergyUsage)]
        public Entity.BaseResponse<List<Response.EnergyUsageResponse>> EnergyUsage(Request.ChartRequest request)
        {
            Entity.BaseResponse<List<Response.EnergyUsageResponse>> response = new Entity.BaseResponse<List<Response.EnergyUsageResponse>>(true);
            try
            {
                response.Data = _chartService.GetEnergyUsage(request);
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<List<Response.EnergyUsageResponse>>(false, ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route(ChartRoute.Route.WaterUsage, Name = ChartRoute.Name.WaterUsage)]
        public Entity.BaseResponse<List<Response.WaterUsageResponse>> WaterUsage(Request.ChartRequest request)
        {
            Entity.BaseResponse<List<Response.WaterUsageResponse>> response = new Entity.BaseResponse<List<Response.WaterUsageResponse>>(true);
            try
            {
                response.Data = _chartService.GetWaterUsage(request);
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<List<Response.WaterUsageResponse>>(false, ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route(ChartRoute.Route.SoilNutrition, Name = ChartRoute.Name.SoilNutrition)]
        public Entity.BaseResponse<List<Response.SoilNutritionResponse>> SoilNutrition(Request.ChartRequest request)
        {
            Entity.BaseResponse<List<Response.SoilNutritionResponse>> response = new Entity.BaseResponse<List<Response.SoilNutritionResponse>>(true);
            try
            {
                response.Data = _chartService.GetSoilNutrition(request);
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<List<Response.SoilNutritionResponse>>(false, ex.Message);
            }
            return response;
        }
        #endregion
    }
}