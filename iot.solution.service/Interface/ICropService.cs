using System;
using System.Collections.Generic;
using Entity = iot.solution.entity;
using Request = iot.solution.entity.Request;
using Response = iot.solution.entity.Response;

namespace iot.solution.service.Interface
{
    public interface ICropService
    {
        List<Entity.Crop> Get();
        Entity.Crop Get(Guid id);
        Entity.ActionStatus Manage(Entity.CropModel corp);
        Entity.SearchResult<List<Entity.Crop>> List(Entity.SearchRequest request);
        Entity.ActionStatus Delete(Guid id);
        Entity.ActionStatus DeleteImage(Guid id);
        Entity.ActionStatus UpdateStatus(Guid id, bool status);
        List<Entity.Crop> GetCrops(Guid greenhouseId);
    }
}
