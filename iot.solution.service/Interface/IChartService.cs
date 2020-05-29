using System;
using System.Collections.Generic;
using Entity = iot.solution.entity;
using Request = iot.solution.entity.Request;
using Response = iot.solution.entity.Response;

namespace iot.solution.service.Interface
{
    public interface IChartService
    {
        Entity.ActionStatus TelemetrySummary_DayWise();
        Entity.ActionStatus TelemetrySummary_HourWise();
        Entity.BaseResponse<List<Response.WaterUsageResponse>> GetWaterUsage(Request.ChartRequest request);
        Entity.BaseResponse<List<Response.EnergyUsageResponse>> GetEnergyUsage(Request.ChartRequest request);
        Entity.BaseResponse<List<Response.SoilNutritionResponse>> GetSoilNutrition(Request.ChartRequest request);
    }
}
