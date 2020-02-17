using iot.solution.entity;
using System;
using System.Collections.Generic;
using System.Text;
using Entity = iot.solution.entity;

namespace iot.solution.service.Interface
{
    public interface IRuleService
    {
        Entity.SingleRuleResponse Get(Guid id);
        Entity.ActionStatus Manage(Entity.Rule request);
        Entity.ActionStatus Delete(Guid id);
        Entity.SearchResult<List<Entity.AllRuleResponse>> List(Entity.SearchRequest request);
        ActionStatus UpdateStatus(Guid id, bool status);
    }
}
