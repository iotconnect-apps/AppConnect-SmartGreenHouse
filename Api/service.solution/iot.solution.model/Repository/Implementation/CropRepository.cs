using component.common.model;
using component.logger;
using iot.solution.data;
using iot.solution.model.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Entity = iot.solution.entity;
using Model = iot.solution.model.Models;
using Response = iot.solution.entity.Response;

namespace iot.solution.model.Repository.Implementation
{
    public class CropRepository : GenericRepository<Model.Crop>, ICropRepository
    {
        private readonly ILogger logger;
        public CropRepository(IUnitOfWork unitOfWork, ILogger logManager) : base(unitOfWork, logManager)
        {
            logger = logManager;
            _uow = unitOfWork;
        }

        public Entity.SearchResult<List<Model.Crop>> List(Entity.SearchRequest request)
        {
            Entity.SearchResult<List<Model.Crop>> result = new Entity.SearchResult<List<Model.Crop>>();
            try
            {
                logger.Information(Constants.ACTION_ENTRY, "CorpRepository.Get");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(AppConfig.CurrentUserId, request.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("companyguid", AppConfig.CompanyId, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("greenHouseGuid", request.GreenHouseId, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("search", request.SearchText, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagesize", request.PageSize, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagenumber", request.PageNumber, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("orderby", request.OrderBy, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("count", DbType.Int32, ParameterDirection.Output, 16));
                    DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[Crop_List]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result.Items = iot.solution.data.Utilities.DataUtils.DataReaderToList<Model.Crop>(dbDataReader, null);
                    result.Count = int.Parse(parameters.Where(p => p.ParameterName.Equals("count")).FirstOrDefault().Value.ToString());
                }
                logger.Information(Constants.ACTION_EXIT, "CorpRepository.Get");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }

        public List<Response.GreenHouseCropResponse> GetGreenHouseCorps(Guid greenhouseId)
        {
            List<Response.GreenHouseCropResponse> result = new List<Response.GreenHouseCropResponse>();
            try
            {
                logger.Information(Constants.ACTION_ENTRY, "GreenHouseRepository.GetGreenHouseCorps");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    result = (from c in _uow.DbContext.Crop
                              where c.IsActive && c.IsDeleted == false
                              && c.GreenHouseGuid.Equals(greenhouseId)
                              select new Response.GreenHouseCropResponse()
                              {
                                  CropGuid = c.Guid,
                                  CropName = c.Name,
                                  Image = c.Image,
                              }).ToList();
                }
                logger.Information(Constants.ACTION_EXIT, "GreenHouseRepository.GetGreenHouseCorps");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
    }
}
