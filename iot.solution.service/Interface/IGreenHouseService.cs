using System;
using System.Collections.Generic;
using Entity = iot.solution.entity;
using Response = iot.solution.entity.Response;
using Model = iot.solution.model.Models;
namespace iot.solution.service.Interface
{
    public interface IGreenHouseService
    {
        List<Entity.GreenHouse> Get(bool isAdmin,bool? isActive);
        Entity.GreenHouse Get(Guid id);
       
        Entity.ActionStatus Manage(Entity.GreenHouseModel request);
        Entity.ActionStatus Delete(Guid id);
        Entity.SearchResult<List<Entity.GreenHouseDetail>> List(Entity.SearchRequest request);
        Entity.ActionStatus UpdateStatus(Guid id, bool status);
        
        Response.GreenHouseDetailResponse GetGreenHouseDetail(Guid greenhouseId);
        List<Response.GreenHouseCropResponse> GetGreenHouseCorps(Guid greenhouseId);
    }
}
