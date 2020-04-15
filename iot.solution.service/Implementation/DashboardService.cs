using component.logger;
using iot.solution.model.Repository.Interface;
using iot.solution.service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Entity = iot.solution.entity;
using LogHandler = component.services.loghandler;

namespace iot.solution.service.Implementation
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardrepository;
        private readonly LogHandler.Logger _logger;
        private readonly IDeviceService _deviceService;
        public DashboardService(IDashboardRepository dashboardrepository, LogHandler.Logger logger, IDeviceService deviceService)
        {
            _dashboardrepository = dashboardrepository;
            _logger = logger;
            _deviceService = deviceService;
        }

        public List<Entity.LookupItem> GetFarmsLookup(Guid companyId)
        {
            List<Entity.LookupItem> lstResult = new List<Entity.LookupItem>();
            try
            {
                lstResult = (from g in _dashboardrepository.FindBy(r => r.CompanyGuid == companyId)
                             select new Entity.LookupItem()
                             {
                                 Text = g.Name,
                                 Value = g.Guid.ToString().ToUpper()
                             }).ToList();
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return lstResult;
        }

        public Entity.DashboardOverviewResponse GetOverview()
        {
            List<Entity.DashboardOverviewResponse> listResult = new List<Entity.DashboardOverviewResponse>();
            Entity.DashboardOverviewResponse result = new Entity.DashboardOverviewResponse();
            try
            {
                listResult = _dashboardrepository.GetStatistics();
                if (listResult.Count > 0)
                {
                    result = listResult[0];
                }

                var deviceResult = _deviceService.GetDeviceCounters();

                if (deviceResult.IsSuccess && deviceResult.Data != null)
                {
                    result.ConnectedDeviceCount = deviceResult.Data.connected;
                    result.DisconnectedDeviceCount = deviceResult.Data.disConnected;
                }
                else
                {
                    result.ConnectedDeviceCount = 0;
                    result.DisconnectedDeviceCount = 0;
                }


            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }
    }
}
