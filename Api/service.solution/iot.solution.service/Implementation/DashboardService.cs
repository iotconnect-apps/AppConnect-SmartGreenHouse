using component.logger;
using iot.solution.model.Repository.Interface;
using iot.solution.service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using Entity = iot.solution.entity;

namespace iot.solution.service.Implementation
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardrepository;
        private readonly ILogger _logger;
        public DashboardService(IDashboardRepository dashboardrepository, ILogger logManager)
        {
            _dashboardrepository = dashboardrepository;
            _logger = logManager;
        }

        public List<Entity.LookupItem> GetFarmsLookup(Guid companyId)
        {
            List<Entity.LookupItem> lstResult = new List<Entity.LookupItem>();
            lstResult = (from g in _dashboardrepository.FindBy(r => r.CompanyGuid == companyId)
                         select new Entity.LookupItem()
                         {
                             Text = g.Name,
                             Value = g.Guid.ToString().ToUpper()
                         }).ToList();
            return lstResult;
        }

        public Entity.OverviewResponse GetOverview(Guid companyID)
        {
            return new Entity.OverviewResponse()
            {
                TotalConnectedDevices = 10,
                TotalDisconnectedDevices = 8,
                TotalCorp = 5,
                TotalGreenhouse = 2
            };
        }
    }
}
