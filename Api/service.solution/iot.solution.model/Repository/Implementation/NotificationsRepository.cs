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
    public class NotificationsRepository : GenericRepository<GreenHouse>,INotificationsRepository
    {
        private readonly ILogger logger;
        public NotificationsRepository(IUnitOfWork unitOfWork, ILogger logManager) : base(unitOfWork, logManager)
        {
            logger = logManager;
            _uow = unitOfWork;
        }

        //public Entity.SearchResult<List<Entity.Notifications>> List(Entity.SearchRequest request)
        //{
        //    Entity.SearchResult<List<Entity.Notifications>> result = new Entity.SearchResult<List<Entity.Notifications>>();
        //    try
        //    {
        //        logger.Information(Constants.ACTION_ENTRY, "NotificationsRepository.Get");
        //        using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
        //        {
        //            List<DbParameter> parameters = sqlDataAccess.CreateParams(AppConfig.CurrentUserId, request.Version);                    
        //            parameters.Add(sqlDataAccess.CreateParameter("count", DbType.Int32, ParameterDirection.Output, 16));
        //            DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[AdminRule_List]", CommandType.StoredProcedure, null), parameters.ToArray());
        //            result.Items = iot.solution.data.Utilities.DataUtils.DataReaderToList<Entity.Notifications>(dbDataReader, null);
        //            result.Count = int.Parse(parameters.Where(p => p.ParameterName.Equals("count")).FirstOrDefault().Value.ToString());
        //        }
        //        logger.Information(Constants.ACTION_EXIT, "NotificationsRepository.Get");
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(Constants.ACTION_EXCEPTION, ex);
        //    }
        //    return result;
        //}
    }
}
