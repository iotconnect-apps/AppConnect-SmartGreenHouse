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
    public class RoleRepository : GenericRepository<Model.Role>, IRoleRepository
    {
        private readonly ILogger logger;

        public RoleRepository(IUnitOfWork unitOfWork, ILogger logManager) : base(unitOfWork, logManager)
        {
            logger = logManager;
            _uow = unitOfWork;
        }

        public Entity.SearchResult<List<Model.Role>> List(Entity.SearchRequest request)
        {
            Entity.SearchResult<List<Model.Role>> result = new Entity.SearchResult<List<Model.Role>>();
            try
            {
                logger.Information(Constants.ACTION_ENTRY, "RoleRepository.Get");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<System.Data.Common.DbParameter> parameters = sqlDataAccess.CreateParams(AppConfig.CurrentUserId, request.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("companyguid", AppConfig.CompanyId, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("search", request.SearchText, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagesize", request.PageSize, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagenumber", request.PageNumber, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("orderby", request.OrderBy, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("count", DbType.Int32, ParameterDirection.Output, 16));
                    parameters.Add(sqlDataAccess.CreateParameter("culture", AppConfig.Culture, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("enableDebugInfo", AppConfig.EnableDebugInfo, DbType.String, ParameterDirection.Input));
                    System.Data.Common.DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[Role_List]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result.Items = iot.solution.data.Utilities.DataUtils.DataReaderToList<Model.Role>(dbDataReader, null);
                    result.Count = int.Parse(parameters.Where(p => p.ParameterName.Equals("count")).FirstOrDefault().Value.ToString());
                }
                logger.Information(Constants.ACTION_EXIT, "RoleRepository.Get");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }

        public ActionStatus Manage(Model.Role request)
        {
            ActionStatus result = new ActionStatus(true);
            try
            {
                logger.Information(Constants.ACTION_ENTRY, "RoleRepository.Manage");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(AppConfig.CurrentUserId, AppConfig.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("companyGuid", request.CompanyGuid, DbType.Guid, ParameterDirection.Input));                   
                    parameters.Add(sqlDataAccess.CreateParameter("name", request.Name, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("description", request.Description, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("isAdminRole", request.IsAdminRole, DbType.Boolean, ParameterDirection.Input));                    
                    parameters.Add(sqlDataAccess.CreateParameter("newid",request.Guid, DbType.Guid, ParameterDirection.Output));
                    parameters.Add(sqlDataAccess.CreateParameter("culture", AppConfig.Culture, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("enableDebugInfo", AppConfig.EnableDebugInfo, DbType.String, ParameterDirection.Input));
                    int intResult = sqlDataAccess.ExecuteNonQuery(sqlDataAccess.CreateCommand("[Role_AddUpdate]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result.Data = parameters.Where(p => p.ParameterName.Equals("newid")).FirstOrDefault().Value.ToString();
                }
                logger.Information(Constants.ACTION_EXIT, "RoleRepository.Manage");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
    }
}