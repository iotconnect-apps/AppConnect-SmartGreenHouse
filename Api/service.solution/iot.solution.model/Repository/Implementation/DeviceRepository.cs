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
using System.Xml.Linq;
using Entity = iot.solution.entity;
using Model = iot.solution.model.Models;
using Response = iot.solution.entity.Response;

namespace iot.solution.model.Repository.Implementation
{
    public class DeviceRepository : GenericRepository<Model.Device>, IDeviceRepository
    {
        private readonly ILogger logger;

        public DeviceRepository(IUnitOfWork unitOfWork, ILogger logManager) : base(unitOfWork, logManager)
        {
            logger = logManager;
            _uow = unitOfWork;
        }

        public Model.Device Get(string device)
        {
            var result = new Model.Device();
            try
            {
                logger.Information(Constants.ACTION_ENTRY, "DeviceRepository.Get");
                _uow.DbContext.Device.Where(u => u.Name.Equals(device, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                logger.Information(Constants.ACTION_EXIT, "DeviceRepository.Get");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
        public Entity.ActionStatus Manage(Model.Device request)
        {
            ActionStatus result = new ActionStatus(true);
            try
            {
                logger.Information(Constants.ACTION_ENTRY, "DeviceRepository.Manage");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(AppConfig.CurrentUserId, AppConfig.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("guid", request.Guid, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("companyGuid", request.CompanyGuid, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("greenHouseGuid", request.GreenHouseGuid, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("templateGuid", request.TemplateGuid, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("parentDeviceGuid", request.ParentDeviceGuid, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("type", request.Type, DbType.Byte, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("uniqueId", request.UniqueId, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("name", request.Name, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("note", request.Note, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("tag", request.Tag, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("image", request.Image, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("isProvisioned", request.IsProvisioned, DbType.Boolean, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("newid", request.Guid, DbType.Guid, ParameterDirection.Output));
                    parameters.Add(sqlDataAccess.CreateParameter("culture", AppConfig.Culture, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("enableDebugInfo", AppConfig.EnableDebugInfo, DbType.String, ParameterDirection.Input));
                    int intResult = sqlDataAccess.ExecuteNonQuery(sqlDataAccess.CreateCommand("[Device_AddUpdate]", CommandType.StoredProcedure, null), parameters.ToArray());
                    string guidResult = parameters.Where(p => p.ParameterName.Equals("newid")).FirstOrDefault().Value.ToString();
                    if (!string.IsNullOrEmpty(guidResult))
                    {
                        result.Data = _uow.DbContext.Device.Where(u => u.Guid.Equals(Guid.Parse(guidResult))).FirstOrDefault();
                    }
                    //   result.Data = int.Parse(parameters.Where(p => p.ParameterName.Equals("newid")).FirstOrDefault().Value.ToString());

                }
                logger.Information(Constants.ACTION_EXIT, "DeviceRepository.Manage");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
        public Entity.ActionStatus Delete(Guid id)
        {
            throw new NotImplementedException();
        }
        public Entity.SearchResult<List<Model.Device>> List(Entity.SearchRequest request)
        {
            Entity.SearchResult<List<Model.Device>> result = new Entity.SearchResult<List<Model.Device>>();
            try
            {
                logger.Information(Constants.ACTION_ENTRY, "DeviceRepository.Get");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<System.Data.Common.DbParameter> parameters = sqlDataAccess.CreateParams(AppConfig.CurrentUserId, request.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("companyguid", AppConfig.CompanyId, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("isParent", false, DbType.Boolean, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("search", request.SearchText, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagesize", request.PageSize, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagenumber", request.PageNumber, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("orderby", request.OrderBy, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("count", DbType.Int32, ParameterDirection.Output, 16));
                    System.Data.Common.DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[Device_List]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result.Items = iot.solution.data.Utilities.DataUtils.DataReaderToList<Model.Device>(dbDataReader, null);
                    result.Count = int.Parse(parameters.Where(p => p.ParameterName.Equals("count")).FirstOrDefault().Value.ToString());
                }
                logger.Information(Constants.ACTION_EXIT, "DeviceRepository.Get");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
        public Entity.SearchResult<List<Entity.DeviceDetailResponse>> DetailList(Entity.SearchRequest request)
        {
            Entity.SearchResult<List<Entity.DeviceDetailResponse>> result = new Entity.SearchResult<List<Entity.DeviceDetailResponse>>();
            try
            {
                logger.Information(Constants.ACTION_ENTRY, "DeviceRepository.ListByGreenHouse");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<System.Data.Common.DbParameter> parameters = sqlDataAccess.CreateParams(AppConfig.CurrentUserId, request.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("greenHouseGuid", request.GreenHouseId, DbType.Guid, ParameterDirection.Input));                  
                    parameters.Add(sqlDataAccess.CreateParameter("search", request.SearchText, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagesize", request.PageSize, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagenumber", request.PageNumber, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("orderby", request.OrderBy, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("count", DbType.Int32, ParameterDirection.Output, 16));                 
                    System.Data.Common.DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[Device_ListByGreenHouse]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result.Items = iot.solution.data.Utilities.DataUtils.DataReaderToList<Entity.DeviceDetailResponse>(dbDataReader, null);
                    result.Count = int.Parse(parameters.Where(p => p.ParameterName.Equals("count")).FirstOrDefault().Value.ToString());
                }
                logger.Information(Constants.ACTION_EXIT, "DeviceRepository.ListByGreenHouse");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
        public Entity.SearchResult<List<Entity.DeviceSearchResponse>> GatewayList(Entity.SearchRequest request)
        {
            Entity.SearchResult<List<Entity.DeviceSearchResponse>> result = new Entity.SearchResult<List<Entity.DeviceSearchResponse>>();
            try
            {
                logger.Information(Constants.ACTION_ENTRY, "DeviceRepository.GatewayList");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<System.Data.Common.DbParameter> parameters = sqlDataAccess.CreateParams(AppConfig.CurrentUserId, request.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("companyguid", AppConfig.CompanyId, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("isParent", true, DbType.Boolean, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("search", request.SearchText, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagesize", request.PageSize, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagenumber", request.PageNumber, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("orderby", request.OrderBy, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("count", DbType.Int32, ParameterDirection.Output, 16));
                    System.Data.Common.DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[Device_List]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result.Items = iot.solution.data.Utilities.DataUtils.DataReaderToList<Entity.DeviceSearchResponse>(dbDataReader, null);
                    result.Count = int.Parse(parameters.Where(p => p.ParameterName.Equals("count")).FirstOrDefault().Value.ToString());
                }
                logger.Information(Constants.ACTION_EXIT, "DeviceRepository.GatewayList");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
        public Entity.SearchResult<List<Model.Device>> GetChildDevice(Entity.SearchRequest request)
        {
            Entity.SearchResult<List<Model.Device>> result = new Entity.SearchResult<List<Model.Device>>();
            try
            {
                logger.Information(Constants.ACTION_ENTRY, "ChildDeviceRepository.Get");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<System.Data.Common.DbParameter> parameters = sqlDataAccess.CreateParams(AppConfig.CompanyId, request.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("guid", request.Guid, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagesize", request.PageSize, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagenumber", request.PageNumber, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("orderby", request.OrderBy, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("count", DbType.Int32, ParameterDirection.Output, 16));
                    System.Data.Common.DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[ChildDevice_ListByGuid]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result.Items = iot.solution.data.Utilities.DataUtils.DataReaderToList<Model.Device>(dbDataReader, null);
                    result.Count = int.Parse(parameters.Where(p => p.ParameterName.Equals("count")).FirstOrDefault().Value.ToString());
                }
                logger.Information(Constants.ACTION_EXIT, "ChildDeviceRepository.Get");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
        public List<Response.GreenHouseDevicesResponse> GetGreenHouseDevices(Guid? greenhouseId, Guid? deviceId)
        {
            List<Response.GreenHouseDevicesResponse> result = new List<Response.GreenHouseDevicesResponse>();
            try
            {
                logger.Information(Constants.ACTION_ENTRY, "GetGreenHouseDevices.Get");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<System.Data.Common.DbParameter> parameters = sqlDataAccess.CreateParams(AppConfig.CompanyId, AppConfig.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("companyGuid", AppConfig.CompanyId, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("greenHouseGuid", greenhouseId, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("parentDeviceGuid", deviceId, DbType.Guid, ParameterDirection.Input));
                    System.Data.Common.DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[Device_Lookup]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result = iot.solution.data.Utilities.DataUtils.DataReaderToList<Response.GreenHouseDevicesResponse>(dbDataReader, null);
                }
                logger.Information(Constants.ACTION_EXIT, "GetGreenHouseDevices.Get");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
        public Entity.BaseResponse<int> ValidateKit(string kitCode)
        {
            Entity.BaseResponse<int> result = new Entity.BaseResponse<int>();
            try
            {
                logger.Information(Constants.ACTION_ENTRY, "ValidateKit.Get");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<System.Data.Common.DbParameter> parameters = sqlDataAccess.CreateParams(AppConfig.CompanyId, AppConfig.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("kitCode", kitCode, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("culture", AppConfig.Culture, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("enableDebugInfo", AppConfig.EnableDebugInfo, DbType.String, ParameterDirection.Input));                    
                    result.Data = (Int32)(sqlDataAccess.ExecuteScalar(sqlDataAccess.CreateCommand("[Validate_KitCode]", CommandType.StoredProcedure,null),parameters.ToArray()));
                    if (result.Data > 0 && result !=null)                   
                        result.IsSuccess = true;                        
                   
                        
                }
                logger.Information(Constants.ACTION_EXIT, "ValidateKit.Get");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
        public Entity.BaseResponse<List<Entity.KitDevice>> ProvisionKit(Entity.ProvisionKitRequest request)
        {
            Entity.BaseResponse<List<Entity.KitDevice>> result = new Entity.BaseResponse<List<Entity.KitDevice>>();
            try
            {
                logger.Information(Constants.ACTION_ENTRY, "GetDeviceStatus.Post");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    var kitDevices = string.Join(",", request.KitDevices);
                    
                    List<System.Data.Common.DbParameter> parameters = sqlDataAccess.CreateParams(AppConfig.CompanyId, AppConfig.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("kitCode", request.KitCode, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("uniqueId", kitDevices, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("culture", AppConfig.Culture, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("enableDebugInfo", AppConfig.EnableDebugInfo, DbType.String, ParameterDirection.Input));
                    System.Data.Common.DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[KitDevice_GetStatus]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result.Data =iot.solution.data.Utilities.DataUtils.DataReaderToList<Entity.KitDevice>(dbDataReader, null);
                    if (parameters.Where(p => p.ParameterName.Equals("output")).FirstOrDefault().Value.ToString() == "1") {
                        result.Message = parameters.Where(p => p.ParameterName.Equals("fieldname")).FirstOrDefault().Value.ToString();
                    }
                }
                logger.Information(Constants.ACTION_EXIT, "GetDeviceStatus.Get");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }

        #region Lookup
        public List<Entity.LookupItem> GetGetwayLookup()
        {
            using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
            {
                return sqlDataAccess.QueryList<Entity.LookupItem>("SELECT CONVERT(NVARCHAR(50),[Guid]) AS [Value], [name] AS [Text] FROM [Device] WHERE [parentDeviceGuid] IS NULL AND [isActive] = 1 AND [isDeleted] = 0");
            }
        }
        public List<Entity.LookupItem> GetDeviceLookup()
        {
            using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
            {
                return sqlDataAccess.QueryList<Entity.LookupItem>("SELECT CONVERT(NVARCHAR(50),[Guid]) AS [Value], [name] AS [Text] FROM [Device] WHERE [parentDeviceGuid] IS NOT NULL AND [isActive] = 1 AND [isDeleted] = 0");
            }
        }
        #endregion
    }
}
