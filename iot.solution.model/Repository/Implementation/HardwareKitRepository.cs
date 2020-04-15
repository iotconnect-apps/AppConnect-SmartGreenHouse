using iot.solution.data;
using iot.solution.entity;
using iot.solution.model.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.Serialization;
using Entity = iot.solution.entity;
using LogHandler = component.services.loghandler;
using Model = iot.solution.model.Models;

namespace iot.solution.model.Repository.Implementation
{
    public class HardwareKitRepository : GenericRepository<Model.HardwareKit>, IHardwareKitRepository
    {
        private readonly LogHandler.Logger _logger;
        public HardwareKitRepository(IUnitOfWork unitOfWork, LogHandler.Logger logger) : base(unitOfWork, logger)
        {
            _logger = logger;
            _uow = unitOfWork;
        }

        public Entity.SearchResult<List<Entity.HardwareKitResponse>> List(Entity.SearchRequest request, bool isAssigned, string companyId)
        {
            Entity.SearchResult<List<Entity.HardwareKitResponse>> result = new Entity.SearchResult<List<Entity.HardwareKitResponse>>();
            try
            {
                _logger.InfoLog(LogHandler.Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<System.Data.Common.DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, request.Version);
                    if (!string.IsNullOrEmpty(companyId))
                        parameters.Add(sqlDataAccess.CreateParameter("companyguid", Guid.Parse(companyId), DbType.Guid, ParameterDirection.Input));

                    parameters.Add(sqlDataAccess.CreateParameter("search", request.SearchText, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("isAssigned", isAssigned ? 1 : 0, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagesize", request.PageSize, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagenumber", request.PageNumber, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("orderby", request.OrderBy, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("count", DbType.Int32, ParameterDirection.Output, 16));
                    System.Data.Common.DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[HardwareKit_List]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result.Items = DataUtils.DataReaderToList<Entity.HardwareKitResponse>(dbDataReader, null);
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
        public Entity.HardwareKitDTO GetHardwareKitDetails(Entity.SearchRequest request)
        {
            Entity.SearchResult<List<Entity.HardwareKit>> result = new Entity.SearchResult<List<Entity.HardwareKit>>();
            var hardwareKitDto = new Entity.HardwareKitDTO();
            try
            {
                _logger.InfoLog(LogHandler.Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<System.Data.Common.DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, request.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("guid", request.Guid, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagesize", request.PageSize, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagenumber", request.PageNumber, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("orderby", request.OrderBy, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("count", DbType.Int32, ParameterDirection.Output, 16));
                    System.Data.Common.DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[KitDevice_List]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result.Items = DataUtils.DataReaderToList<Entity.HardwareKit>(dbDataReader, null);
                    result.Count = int.Parse(parameters.Where(p => p.ParameterName.Equals("count")).FirstOrDefault().Value.ToString());
                    HardwareKitDTO hardwareKitDTO = new HardwareKitDTO();

                    if (result.Count >= 0)
                    {

                        var hardwareKit = result.Items.FirstOrDefault();
                        hardwareKitDto = (from c in result.Items
                                          group c by new
                                          {
                                              c.KitGuid,
                                              c.KitCode,
                                              c.KitType,
                                          } into gcs
                                          select new HardwareKitDTO()
                                          {
                                              Guid = hardwareKit.Guid,
                                              CompanyGuid = hardwareKit.CompanyGuid,
                                              KitTypeGuid = result.Items.Count > 0 ? result.Items[0].TemplateGuid : hardwareKit.KitTypeGuid,
                                              KitCode = gcs.Key.KitCode,
                                              KitDevices = gcs.Select(x => new KitDeviceDTO()
                                              {
                                                  Name = x.Name,
                                                  IsProvisioned = x.IsProvisioned,
                                                  Note = x.Note,
                                                  Tag = x.TagGuid,
                                                  KitGuid = x.KitGuid,
                                                  UniqueId = x.UniqueId,
                                                  ParentUniqueId = x.ParentUniqueId,
                                              }).ToList()
                                          }).FirstOrDefault();


                    }
                }
                _logger.InfoLog(LogHandler.Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }

            return hardwareKitDto;
        }
        public ActionStatus SaveHardwareKit(Entity.KitVerifyRequest requestData, bool isEdit)
        {
            var response = new ActionStatus();
            var result = new List<BulkUploadResponse>();
            var createdHardwareKits = new List<BulkUploadResponse>();
            try
            {
                _logger.InfoLog(LogHandler.Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                var xmlData = string.Empty;
                using (var stringwriter = new System.IO.StringWriter())
                {
                    var serializer = new XmlSerializer(requestData.GetType());
                    serializer.Serialize(stringwriter, requestData);
                    xmlData = stringwriter.ToString();
                }

                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<System.Data.Common.DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, component.helper.SolutionConfiguration.Version); parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, component.helper.SolutionConfiguration.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("data", xmlData.ToString(), DbType.Xml, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("isEdit", (isEdit == false ? 0 : 1), DbType.Boolean, ParameterDirection.Input));
                    System.Data.Common.DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[KitDevice_AddUpdate]", CommandType.StoredProcedure, null), parameters.ToArray());
                    // result = DataUtils.DataReaderToList<Entity.BulkUploadResponse>(dbDataReader, null);


                    while (dbDataReader.Read())
                    {
                        var item = new Entity.BulkUploadResponse();
                        if (dbDataReader["kitGuid"] != null)
                        {
                            //createdHardwareKits.Add(new Guid(dbDataReader["kitGuid"].ToString().ToUpper()));
                            item.kitGuid = new Guid(dbDataReader["kitGuid"].ToString().ToUpper());
                            item.kitTypeGuid = new Guid(dbDataReader["kitTypeGuid"].ToString().ToUpper());
                            item.kitCode = (dbDataReader["kitCode"].ToString());

                            createdHardwareKits.Add(item);
                        }
                        else
                        {
                            response.Message = dbDataReader["fieldname"].ToString().ToUpper();
                            response.Data = null;
                            response.Success = false;
                        }
                    }

                    if ((createdHardwareKits.Count != 0 || createdHardwareKits.Count == 1) || isEdit)
                    {
                        //requestData.HardwareKits.FirstOrDefault(). = createdHardwareKits.FirstOrDefault();
                        response.Success = true;
                        response.Data = createdHardwareKits;
                    }
                    else
                    {
                        response.Success = false;
                        response.Data = createdHardwareKits;
                        response.Message = "Unable to save Hardware-Kit";
                    }

                }

                _logger.InfoLog(LogHandler.Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message.ToString();
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }

            return response;
        }
        public ActionStatus UploadHardwareKit(List<Entity.HardwareKitDTO> requestData)
        {
            var response = new ActionStatus();
            try
            {
                _logger.InfoLog(LogHandler.Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                List<Guid> createdHardwareKits = new List<Guid>();
                var stringResult = string.Empty;
                using (var stringwriter = new System.IO.StringWriter())
                {
                    var serializer = new XmlSerializer(requestData.GetType());
                    serializer.Serialize(stringwriter, requestData);
                    stringResult = stringwriter.ToString();
                }

                stringResult = stringResult.Replace("HardwareKitDTO", "HardwareKit").Replace("KitDeviceDTO", "KitDevice");

                XDocument requestXML = XDocument.Parse(stringResult);
                requestXML = new XDocument(new XElement("HardwareKits", requestXML.Root));

                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<System.Data.Common.DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, component.helper.SolutionConfiguration.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("data", requestXML.ToString(), DbType.Xml, ParameterDirection.Input));

                    System.Data.Common.DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[KitDevice_AddUpdate]", CommandType.StoredProcedure, null), parameters.ToArray());

                    while (dbDataReader.Read())
                    {
                        if (dbDataReader["guid"] != null)
                        {
                            createdHardwareKits.Add(new Guid(dbDataReader["guid"].ToString().ToUpper()));
                        }
                        else
                        {
                            response.Message = dbDataReader["fieldname"].ToString().ToUpper();
                            response.Data = null;
                            response.Success = false;
                        }
                    }

                    if (createdHardwareKits.Count != 0 && createdHardwareKits.Count == 1)
                    {
                        //  requestData.Guid = createdHardwareKits.FirstOrDefault();
                        response.Success = true;
                        response.Data = requestData;
                    }
                    else
                    {
                        response.Success = false;
                        response.Data = requestData;
                        response.Message = "Unable to save Hardware-Kit";
                    }
                    //var result = DataUtils.DataReaderToList<Entity.HardwareKitResponse>(dbDataReader, null);
                    //result.Count = int.Parse(parameters.Where(p => p.ParameterName.Equals("count")).FirstOrDefault().Value.ToString());
                }
                _logger.InfoLog(LogHandler.Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message.ToString();
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }

            return response;
        }
        public ActionStatus VerifyHardwareKit(KitVerifyRequest request, bool isEdit = false)
        {
            var response = new ActionStatus(true);
            var result = new List<BulkUploadResponse>();
            try
            {
                _logger.InfoLog(LogHandler.Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);

                var xmlData = string.Empty;
                using (var stringwriter = new System.IO.StringWriter())
                {
                    var serializer = new XmlSerializer(request.GetType());
                    serializer.Serialize(stringwriter, request);
                    xmlData = stringwriter.ToString();
                }

                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<System.Data.Common.DbParameter> parameters = new List<System.Data.Common.DbParameter>();
                    parameters.Add(sqlDataAccess.CreateParameter("data", xmlData, DbType.Xml, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("isEdit", (isEdit == false ? 0 : 1), DbType.Boolean, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("invokinguser", component.helper.SolutionConfiguration.CurrentUserId, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("version", component.helper.SolutionConfiguration.Version, DbType.String, ParameterDirection.Input));

                    System.Data.Common.DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[KitDevice_Validate]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result = DataUtils.DataReaderToList<Entity.BulkUploadResponse>(dbDataReader, null);

                    response.Data = result;

                    var isValid = result.Where(x => !string.IsNullOrEmpty(x.kitError) || !string.IsNullOrEmpty(x.deviceError)).Count();

                    if (isValid > 0)
                        response.Success = false;
                    else
                        response.Success = true;
                }
                _logger.InfoLog(LogHandler.Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message.ToString();
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }

            return response;
        }
    }
}
