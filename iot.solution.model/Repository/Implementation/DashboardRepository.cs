using iot.solution.model.Models;
using component.logger;
using iot.solution.model.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using LogHandler = component.services.loghandler;
using Entity = iot.solution.entity;
using System.Reflection;
using iot.solution.data;
using System.Data.Common;
using System.Data;
using component.helper;

namespace iot.solution.model.Repository.Implementation
{
    public class DashboardRepository : GenericRepository<GreenHouse>, IDashboardRepository
    {
        private readonly LogHandler.Logger _logger;
        public DashboardRepository(IUnitOfWork unitOfWork, LogHandler.Logger logManager) : base(unitOfWork, logManager)
        {
            _logger = logManager;
            _uow = unitOfWork;
        }
        public List<Entity.DashboardOverviewResponse> GetStatistics()
        {
            List<Entity.DashboardOverviewResponse> result = new List<Entity.DashboardOverviewResponse>();
            try
            {
                _logger.InfoLog(LogHandler.Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(SolutionConfiguration.CurrentUserId, SolutionConfiguration.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("guid", SolutionConfiguration.CompanyId, DbType.Guid, ParameterDirection.Input));
                    DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[CompanyStatistics_Get]", CommandType.StoredProcedure, null), parameters.ToArray());
                    // DataUtils.DataReaderToObject(dbDataReader, result);
                    result = DataUtils.DataReaderToList<Entity.DashboardOverviewResponse>(dbDataReader, null);
                }
                _logger.InfoLog(LogHandler.Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }
    }
}
