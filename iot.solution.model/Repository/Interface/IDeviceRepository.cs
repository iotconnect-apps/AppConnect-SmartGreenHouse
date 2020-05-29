using System;
using System.Collections.Generic;
using Entity = iot.solution.entity;
using Model = iot.solution.model.Models;
using Response = iot.solution.entity.Response;

namespace iot.solution.model.Repository.Interface
{
    public interface IDeviceRepository : IGenericRepository<Model.Device>
    {
        Model.Device Get(string device);
        Entity.ActionStatus Manage(Model.Device request);
        Entity.ActionStatus Delete(Guid id);
        Entity.SearchResult<List<Model.Device>> List(Entity.SearchRequest request);
        Entity.SearchResult<List<Entity.DeviceDetailResponse>> DetailList(Entity.SearchRequest request);
        Entity.SearchResult<List<Entity.DeviceSearchResponse>> GatewayList(Entity.SearchRequest request);
        Entity.SearchResult<List<Model.Device>> GetChildDevice(Entity.SearchRequest request);
        List<Entity.LookupItem> GetGetwayLookup();
        List<Entity.LookupItem> GetDeviceLookup();
        List<Response.GreenHouseDevicesResponse> GetGreenHouseDevices(Guid? greenhouseId, Guid? deviceId);
        Entity.BaseResponse<int> ValidateKit(string kitCode);
        Entity.BaseResponse<List<Entity.KitDevice>> ProvisionKit(Entity.ProvisionKitRequest request);
        Entity.BaseResponse<List<Response.DeviceDetailResponse>> GetStatistics(Guid deviceId);
    }
}
