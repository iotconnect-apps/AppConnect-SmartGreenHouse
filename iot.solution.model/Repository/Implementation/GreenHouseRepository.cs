using component.helper;
using iot.solution.data;
using iot.solution.entity;
using iot.solution.model.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using Entity = iot.solution.entity;
using LogHandler = component.services.loghandler;
using Model = iot.solution.model.Models;
using Response = iot.solution.entity.Response;

namespace iot.solution.model.Repository.Implementation
{
    public class GreenHouseRepository : GenericRepository<Model.GreenHouse>, IGreenHouseRepository
    {
        private readonly LogHandler.Logger _logger;
        public GreenHouseRepository(IUnitOfWork unitOfWork, LogHandler.Logger logger) : base(unitOfWork, logger)
        {
            _logger = logger;
            _uow = unitOfWork;
        }
        public Entity.BaseResponse<List<Response.GreenHouseDetailResponse>> GetStatistics(Guid greenhouseId)
        {
            Entity.BaseResponse<List<Response.GreenHouseDetailResponse>> result = new Entity.BaseResponse<List<Response.GreenHouseDetailResponse>>();
            try
            {
                _logger.InfoLog(LogHandler.Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(SolutionConfiguration.CurrentUserId, SolutionConfiguration.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("guid", greenhouseId, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("syncDate", DateTime.UtcNow, DbType.DateTime, ParameterDirection.Output));
                    DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[GreenHouse_Statistics_Get]", CommandType.StoredProcedure, null), parameters.ToArray());
                    
                    result.Data = DataUtils.DataReaderToList<Response.GreenHouseDetailResponse>(dbDataReader, null);
                    if (parameters.Where(p => p.ParameterName.Equals("syncDate")).FirstOrDefault() != null)
                    {
                        result.LastSyncDate = Convert.ToString(parameters.Where(p => p.ParameterName.Equals("syncDate")).FirstOrDefault().Value);
                    }
                }
                _logger.InfoLog(LogHandler.Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }
        public List<Entity.LookupItem> GetLookup(Guid companyId)
        {
            var result = new List<Entity.LookupItem>();
            try
            {
                _logger.InfoLog(LogHandler.Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                result = _uow.DbContext.GreenHouse.Where(u => u.CompanyGuid.Equals(companyId) && !u.Guid.Equals(component.helper.SolutionConfiguration.EntityGuid) && u.IsActive == true && !u.IsDeleted).Select(g => new Entity.LookupItem() { Text = g.Name, Value = g.Guid.ToString() }).ToList();
                _logger.InfoLog(LogHandler.Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }
        public Entity.SearchResult<List<Entity.GreenHouseDetail>> List(Entity.SearchRequest request)
        {
            Entity.SearchResult<List<Entity.GreenHouseDetail>> result = new Entity.SearchResult<List<Entity.GreenHouseDetail>>();
            try
            {
                _logger.InfoLog(LogHandler.Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, request.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("companyguid", component.helper.SolutionConfiguration.CompanyId, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("search", request.SearchText, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagesize", request.PageSize, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagenumber", request.PageNumber, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("orderby", request.OrderBy, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("count", DbType.Int32, ParameterDirection.Output, 16));
                    DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[GreenHouse_List]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result.Items = DataUtils.DataReaderToList<Entity.GreenHouseDetail>(dbDataReader, null);
                    result.Count = int.Parse(parameters.Where(p => p.ParameterName.Equals("count")).FirstOrDefault().Value.ToString());
                }
                _logger.InfoLog(LogHandler.Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }
        public ActionStatus Manage(Model.GreenHouse request)
        {
            ActionStatus result = new ActionStatus(true);
            try
            {
                _logger.InfoLog(LogHandler.Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, component.helper.SolutionConfiguration.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("companyGuid", request.CompanyGuid, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("guid", request.Guid, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("name", request.Name, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("description", request.Description, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("address", request.Address, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("address2", request.Address2, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("city", request.City, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("stateGuid", request.StateGuid, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("countryGuid", request.CountryGuid, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("zipCode", request.Zipcode, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("latitude", request.Latitude, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("longitude", request.Longitude, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("image", request.Image, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("newid", request.Guid, DbType.Guid, ParameterDirection.Output));
                    parameters.Add(sqlDataAccess.CreateParameter("culture", component.helper.SolutionConfiguration.Culture, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("enableDebugInfo", component.helper.SolutionConfiguration.EnableDebugInfo, DbType.String, ParameterDirection.Input));
                    int intResult = sqlDataAccess.ExecuteNonQuery(sqlDataAccess.CreateCommand("[GreenHouse_AddUpdate]", CommandType.StoredProcedure, null), parameters.ToArray());
                    string guidResult = parameters.Where(p => p.ParameterName.Equals("newid")).FirstOrDefault().Value.ToString();
                    if (!string.IsNullOrEmpty(guidResult))
                    {
                        result.Data = _uow.DbContext.GreenHouse.Where(u => u.Guid.Equals(Guid.Parse(guidResult))).FirstOrDefault();                    
                    }
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
