using component.common.model;
using component.logger;
using iot.solution.data;
using iot.solution.entity;
using iot.solution.model.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Entity = iot.solution.entity;
using Model = iot.solution.model.Models;


namespace iot.solution.model.Repository.Implementation
{
    public class GreenHouseRepository : GenericRepository<Model.GreenHouse>, IGreenHouseRepository
    {
        private readonly ILogger logger;
        public GreenHouseRepository(IUnitOfWork unitOfWork, ILogger logManager) : base(unitOfWork, logManager)
        {
            logger = logManager;
            _uow = unitOfWork;
        }
        public List<Entity.LookupItem> GetLookup(Guid companyId)
        {
            var result = new List<Entity.LookupItem>();
            try
            {
                logger.Information(Constants.ACTION_ENTRY, "UserRepository.GetLookup");
                result = _uow.DbContext.GreenHouse.Where(u => u.CompanyGuid.Equals(companyId) && u.IsActive == true && !u.IsDeleted).Select(g => new Entity.LookupItem() { Text = g.Name, Value = g.Guid.ToString() }).ToList();
                logger.Information(Constants.ACTION_EXIT, "UserRepository.GetLookup");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
        public Entity.SearchResult<List<Entity.GreenHouseDetail>> List(Entity.SearchRequest request)
        {
            Entity.SearchResult<List<Entity.GreenHouseDetail>> result = new Entity.SearchResult<List<Entity.GreenHouseDetail>>();
            try
            {
                logger.Information(Constants.ACTION_ENTRY, "GreenHouseRepository.Get");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(AppConfig.CurrentUserId, request.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("companyguid", AppConfig.CompanyId, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("search", request.SearchText, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagesize", request.PageSize, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagenumber", request.PageNumber, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("orderby", request.OrderBy, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("count", DbType.Int32, ParameterDirection.Output, 16));
                    DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[GreenHouse_List]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result.Items = iot.solution.data.Utilities.DataUtils.DataReaderToList<Entity.GreenHouseDetail>(dbDataReader, null);
                    result.Count = int.Parse(parameters.Where(p => p.ParameterName.Equals("count")).FirstOrDefault().Value.ToString());
                }
                logger.Information(Constants.ACTION_EXIT, "GreenHouseRepository.Get");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
        public ActionStatus Manage(Model.GreenHouse request)
        {
            ActionStatus result = new ActionStatus(true);
            try
            {
                logger.Information(Constants.ACTION_ENTRY, "GreenHouseRepository.Manage");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(AppConfig.CurrentUserId, AppConfig.Version);
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
                    parameters.Add(sqlDataAccess.CreateParameter("culture", AppConfig.Culture, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("enableDebugInfo", AppConfig.EnableDebugInfo, DbType.String, ParameterDirection.Input));
                    int intResult = sqlDataAccess.ExecuteNonQuery(sqlDataAccess.CreateCommand("[GreenHouse_AddUpdate]", CommandType.StoredProcedure, null), parameters.ToArray());
                    string guidResult = parameters.Where(p => p.ParameterName.Equals("newid")).FirstOrDefault().Value.ToString();
                    if (!string.IsNullOrEmpty(guidResult))
                    {
                        result.Data = _uow.DbContext.GreenHouse.Where(u => u.Guid.Equals(Guid.Parse(guidResult))).FirstOrDefault();                    
                    }
                }
                logger.Information(Constants.ACTION_EXIT, "GreenHouseRepository.Manage");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }

    }
}
