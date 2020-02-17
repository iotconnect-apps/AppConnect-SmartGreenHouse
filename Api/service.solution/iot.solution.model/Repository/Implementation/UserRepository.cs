using component.common.model;
using component.logger;
using iot.solution.data;
using iot.solution.model.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Entity = iot.solution.entity;
using Model = iot.solution.model.Models;

namespace iot.solution.model.Repository.Implementation
{
    public class UserRepository : GenericRepository<Model.User>, IUserRepository
    {
        private readonly ILogger logger;
        public UserRepository(IUnitOfWork unitOfWork, ILogger logManager) : base(unitOfWork, logManager)
        {
            _uow = unitOfWork;
            logger = logManager;
        }
        public Model.User Get(string userName)
        {
            var result = new Model.User();
            try
            {
                logger.Information(Constants.ACTION_ENTRY, "UserRepository.Get");
                _uow.DbContext.User.Where(u => u.Email.Equals(userName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                logger.Information(Constants.ACTION_EXIT, "UserRepository.Get");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
        public Entity.SearchResult<List<Entity.UserResponse>> List(Entity.SearchRequest request)
        {
            Entity.SearchResult<List<Entity.UserResponse>> result = new Entity.SearchResult<List<Entity.UserResponse>>();
            try
            {
                logger.Information(Constants.ACTION_ENTRY, "UserRepository.Get");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<System.Data.Common.DbParameter> parameters = sqlDataAccess.CreateParams(AppConfig.CurrentUserId, request.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("companyguid", AppConfig.CompanyId, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("search", request.SearchText, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagesize", request.PageSize, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagenumber", request.PageNumber, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("orderby", request.OrderBy, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("count", DbType.Int32, ParameterDirection.Output, 16));
                    System.Data.Common.DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[User_List]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result.Items = iot.solution.data.Utilities.DataUtils.DataReaderToList<Entity.UserResponse>(dbDataReader, null);
                    result.Count = int.Parse(parameters.Where(p => p.ParameterName.Equals("count")).FirstOrDefault().Value.ToString());
                }
                logger.Information(Constants.ACTION_EXIT, "UserRepository.Get");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
    }
}