using System;
using System.Collections.Generic;
using Entity = iot.solution.entity;
using Response = iot.solution.entity.Response;

namespace iot.solution.service.Interface
{
    public interface IDeviceService
    {
        List<Entity.Device> Get();
        Entity.Device Get(Guid id);
        Entity.ActionStatus Manage(Entity.Device device);
        Entity.ActionStatus Delete(Guid id);
        Entity.SearchResult<List<Entity.Device>> List(Entity.SearchRequest request);
        Entity.SearchResult<List<Entity.DeviceDetailResponse>> GetGreenHouseDeviceDetailList(Entity.SearchRequest request);
        Response.DeviceDetailResponse GetDeviceDetail(Guid deviceId);
        
            List<Entity.Device> GetGreenHouseDeviceList(Guid greenhouseId);
      
        Entity.SearchResult<List<Entity.DeviceSearchResponse>> GatewayList(Entity.SearchRequest request);
        Entity.ActionStatus UpdateStatus(Guid id, bool status);
        Entity.SearchResult<List<Entity.Device>> ChildDeviceList(Entity.SearchRequest request);
        
        List<Response.GreenHouseDevicesResponse> GetGreenHouseDevices(Guid greenhouseId);
        List<Response.GreenHouseDevicesResponse> GetGreenHouseChildDevices(Guid deviceId);
        Entity.BaseResponse<int> ValidateKit(string kitCode);
        Entity.BaseResponse<bool> ProvisionKit(Entity.ProvisionKitRequest request);
    }
}
