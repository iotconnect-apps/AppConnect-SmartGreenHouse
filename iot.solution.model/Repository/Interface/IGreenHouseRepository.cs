using iot.solution.entity;
using System;
using System.Collections.Generic;
using Entity = iot.solution.entity;
using Model = iot.solution.model.Models;
using Response = iot.solution.entity.Response;
namespace iot.solution.model.Repository.Interface
{
    public interface IGreenHouseRepository : IGenericRepository<Model.GreenHouse>
    {
        Entity.BaseResponse<List<Response.GreenHouseDetailResponse>> GetStatistics(Guid greenhouseId);
        Entity.SearchResult<List<Entity.GreenHouseDetail>> List(Entity.SearchRequest request);
        List<Entity.LookupItem> GetLookup(Guid companyId);
        ActionStatus Manage(Model.GreenHouse request);
        
    }
}
