using iot.solution.entity;
using System;
using System.Collections.Generic;
using Entity = iot.solution.entity;
using Model = iot.solution.model.Models;

namespace iot.solution.model.Repository.Interface
{
    public interface IGreenHouseRepository : IGenericRepository<Model.GreenHouse>
    {
        Entity.SearchResult<List<Entity.GreenHouseDetail>> List(Entity.SearchRequest request);
        List<Entity.LookupItem> GetLookup(Guid companyId);
        ActionStatus Manage(Model.GreenHouse request);
        
    }
}
