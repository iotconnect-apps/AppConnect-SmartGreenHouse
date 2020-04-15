﻿using iot.solution.data;
using iot.solution.model.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Entity = iot.solution.entity;
using LogHandler = component.services.loghandler;
using Model = iot.solution.model.Models;

namespace iot.solution.model.Repository.Implementation
{
    public class AdminUserRepository : GenericRepository<Model.AdminUser>, IAdminUserRepository
    {
        private readonly LogHandler.Logger _logger;
        public AdminUserRepository(IUnitOfWork unitOfWork, LogHandler.Logger logger) : base(unitOfWork, logger)
        {
            _uow = unitOfWork;
            _logger = logger;
        }
        public Model.AdminUser Get(string userName)
        {
            Model.AdminUser result = null;
            try
            {
                _logger.InfoLog(LogHandler.Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                //result = _uow.DbContext.AdminUser.Where(u => u.Email.Equals(userName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                result = _uow.DbContext.AdminUser.Where(u => u.Email.Equals(userName)).FirstOrDefault();
                _logger.InfoLog(LogHandler.Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }
      
        public Model.AdminUser AdminLogin(Entity.LoginRequest request)
        {
            Model.AdminUser result = null;
            try
            {
                _logger.InfoLog(LogHandler.Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                result = _uow.DbContext.AdminUser.Where(x => x.Email.Equals(request.Username)).FirstOrDefault();
                _logger.InfoLog(LogHandler.Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }
        public Entity.SearchResult<List<Entity.UserResponse>> List(Entity.SearchRequest request)
        {
            Entity.SearchResult<List<Entity.UserResponse>> result = new Entity.SearchResult<List<Entity.UserResponse>>();
            try
            {
                _logger.InfoLog(LogHandler.Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<System.Data.Common.DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, request.Version);

                    parameters.Add(sqlDataAccess.CreateParameter("companyguid", request.CompanyId, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("search", request.SearchText, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagesize", request.PageSize, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagenumber", request.PageNumber, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("orderby", request.OrderBy, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("count", DbType.Int32, ParameterDirection.Output, 16));


                    var commandDef = sqlDataAccess.CreateCommand("[AdminUser_List]", CommandType.StoredProcedure, null);


                    System.Data.Common.DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(commandDef, parameters.ToArray());
                    result.Items = DataUtils.DataReaderToList<Entity.UserResponse>(dbDataReader, null);
                    result.Count = int.Parse(parameters.Where(p => p.ParameterName.Equals("count"))?.FirstOrDefault().Value.ToString());
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