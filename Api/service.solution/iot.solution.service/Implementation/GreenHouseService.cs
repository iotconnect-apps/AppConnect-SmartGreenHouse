using System;
using System.Collections.Generic;
using System.Linq;
using component.common.model.CommonModel;
using component.exception;
using component.logger;
using component.helper;
using component.common.model;
using iot.solution.model.Repository.Interface;
using iot.solution.service.Interface;

using Entity = iot.solution.entity;
using Response = iot.solution.entity.Response;
using Model = iot.solution.model.Models;
using IOT = IoTConnect.Model;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace iot.solution.service.Implementation
{
    public class GreenHouseService : IGreenHouseService
    {
        private readonly IGreenHouseRepository _greenHouseRepository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly ICropRepository _cropRepository;
        
        private readonly IotConnectClient _iotConnectClient;
        private readonly ILogger _logger;

        public GreenHouseService(IGreenHouseRepository greenHouseRepository, ICropRepository cropRepository, ILogger logger,IDeviceRepository deviceRepository)
        {
            _logger = logger;
            _greenHouseRepository = greenHouseRepository;
            _cropRepository = cropRepository;
            _deviceRepository = deviceRepository;
            _iotConnectClient = new IotConnectClient(AppConfig.BearerToken, AppConfig.EnvironmentCode, AppConfig.SolutionKey);
        }
        public List<Entity.GreenHouse> Get()
        {
            try
            {
                return _greenHouseRepository.GetAll().Where(e => !e.IsDeleted).Select(p => Mapper.Configuration.Mapper.Map<Entity.GreenHouse>(p)).ToList();
            }
            catch (Exception ex)
            {

                _logger.Error(Constants.ACTION_EXCEPTION, "GreenHouse.GetAll " + ex);
                return new List<Entity.GreenHouse>();
            }
        }
        public Entity.GreenHouse Get(Guid id)
        {
            try
            {
                return _greenHouseRepository.FindBy(r => r.Guid == id).Select(p => Mapper.Configuration.Mapper.Map<Entity.GreenHouse>(p)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "GreenHouse.Get " + ex);
                return null;
            }
        }
       
        public Entity.ActionStatus Manage(Entity.GreenHouseModel request)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {


                if (request.Guid == null || request.Guid == Guid.Empty)
                {
                    Entity.GreenHouse ghEntity = Mapper.Configuration.Mapper.Map<Entity.GreenHouseModel, Entity.GreenHouse>(request);
                    var addEntityResult = AsyncHelpers.RunSync<IOT.DataResponse<IOT.AddEntityResult>>(() =>
                       _iotConnectClient.GreenHouse.Add(Mapper.Configuration.Mapper.Map<IOT.AddEntityModel>(ghEntity)));

                    if (addEntityResult != null && addEntityResult.status && addEntityResult.data != null)
                    {
                        request.Guid = Guid.Parse(addEntityResult.data.EntityGuid.ToUpper());
                        var dbGreenHouse = Mapper.Configuration.Mapper.Map<Entity.GreenHouse, Model.GreenHouse>(ghEntity);
                        if (request.ImageFile != null)
                        {
                            // upload image                                     
                            dbGreenHouse.Image = SaveGreenHouseImage(request.Guid, request.ImageFile);
                        }
                        dbGreenHouse.Guid = request.Guid;
                        dbGreenHouse.CompanyGuid = AppConfig.CompanyId;
                        dbGreenHouse.CreatedDate = DateTime.Now;
                        dbGreenHouse.CreatedBy = AppConfig.CurrentUserId;
                        actionStatus = _greenHouseRepository.Manage(dbGreenHouse);
                        actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.GreenHouse, Entity.GreenHouse>(actionStatus.Data);
                        if (!actionStatus.Success)
                        {
                            _logger.Error($"GreenHouse is not added in solution database, Error: {actionStatus.Message}");
                            var deleteEntityResult = _iotConnectClient.GreenHouse.Delete(request.Guid.ToString()).Result;
                            if (deleteEntityResult != null && deleteEntityResult.status)
                            {
                                _logger.Error($"GreenHouse is not deleted from iotconnect, Error: {deleteEntityResult.message}");
                                actionStatus.Success = false;
                                actionStatus.Message = "Something Went Wrong!";
                            }
                        }
                    }
                    else
                    {
                        _logger.Error($"GreenHouse is not added in iotconnect, Error: {addEntityResult.message}");
                        actionStatus.Success = false;
                        actionStatus.Message = "Something Went Wrong!";
                    }
                }
                else
                {
                    Entity.GreenHouse ghEntity = Mapper.Configuration.Mapper.Map<Entity.GreenHouseModel, Entity.GreenHouse>(request);
                    var olddbGreenHouse = _greenHouseRepository.FindBy(x => x.Guid.Equals(request.Guid)).FirstOrDefault();
                    if (olddbGreenHouse == null)
                    {
                        throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : GreenHouse");
                    }

                    var updateEntityResult = _iotConnectClient.GreenHouse.Update(request.Guid.ToString(), Mapper.Configuration.Mapper.Map<IOT.UpdateEntityModel>(ghEntity)).Result;
                    if (updateEntityResult != null && updateEntityResult.status && updateEntityResult.data != null)
                    {
                        var dbGreenHouse = Mapper.Configuration.Mapper.Map(request, olddbGreenHouse);
                        if (request.ImageFile != null)
                        {
                            if (File.Exists(AppConfig.UploadBasePath + dbGreenHouse.Image) && request.ImageFile.Length > 0)
                            {
                                //if already exists image then delete  old image from server
                                File.Delete(AppConfig.UploadBasePath + dbGreenHouse.Image);
                            }
                            if (request.ImageFile.Length > 0)
                            {
                                // upload new image                                     
                                dbGreenHouse.Image = SaveGreenHouseImage(dbGreenHouse.Guid.Value, request.ImageFile);
                            }
                        }
                        else
                        {
                            //dbGreenHouse.Image = uniqGreenhouse.Image;
                        }
                        dbGreenHouse.UpdatedDate = DateTime.Now;
                        dbGreenHouse.UpdatedBy = AppConfig.CurrentUserId;
                        dbGreenHouse.CompanyGuid = AppConfig.CompanyId;
                        actionStatus = _greenHouseRepository.Manage(dbGreenHouse);
                        actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.GreenHouse, Entity.GreenHouse>(dbGreenHouse);
                        if (!actionStatus.Success)
                        {
                            _logger.Error($"GreenHouse is not updated in solution database, Error: {actionStatus.Message}");
                            actionStatus.Success = false;
                            actionStatus.Message = "Something Went Wrong!";
                        }
                    }
                    else
                    {
                        _logger.Error($"GreenHouse is not added in iotconnect, Error: {updateEntityResult.message}");
                        actionStatus.Success = false;
                        actionStatus.Message = "Something Went Wrong!";
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "GreenHouseManager.Delete " + ex);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }
        // Saving Image on Server   
        private string SaveGreenHouseImage(Guid guid, IFormFile image)
        {
            var fileBasePath = AppConfig.UploadBasePath + AppConfig.GreenHouseImageBasePath;
            string extension = Path.GetExtension(image.FileName);
            var filePath = Path.Combine(fileBasePath, guid + extension);
            if (image != null && image.Length > 0)
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    image.CopyTo(fileStream);
                }
                return Path.Combine(AppConfig.GreenHouseImageBasePath, guid + extension);
            }
            return null;
        }
        public Entity.ActionStatus Delete(Guid id)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                var dbGreenHouse = _greenHouseRepository.FindBy(x => x.Guid.Equals(id)).FirstOrDefault();
                if (dbGreenHouse == null)
                {
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : GreenHouse");
                }
                var dbCrop = _cropRepository.FindBy(x => x.GreenHouseGuid.Equals(id) && x.IsDeleted.Equals(false)).FirstOrDefault();
                var dbDevice = _deviceRepository.FindBy(x => x.GreenHouseGuid.Equals(id) && x.IsDeleted.Equals(false)).FirstOrDefault();
                if (dbCrop == null && dbDevice==null)
                {
                    var deleteEntityResult = _iotConnectClient.GreenHouse.Delete(id.ToString()).Result;
                    if (deleteEntityResult != null && deleteEntityResult.status)
                    {
                        dbGreenHouse.IsDeleted = true;
                        dbGreenHouse.UpdatedDate = DateTime.Now;
                        dbGreenHouse.UpdatedBy = AppConfig.CurrentUserId;
                        return _greenHouseRepository.Update(dbGreenHouse);
                    }
                    else
                    {
                        _logger.Error($"GreenHouse is not deleted from iotconnect, Error: {deleteEntityResult.message}");
                        actionStatus.Success = false;
                        actionStatus.Message = "Something Went Wrong!";
                    }
                }
                else
                {
                    _logger.Error($"GreenHouse is not deleted from solution database.Crop/Device exists, Error: {actionStatus.Message}");
                    actionStatus.Success = false;
                    actionStatus.Message = "GreenHouse is not deleted from solution database.Crop/Device exists";
                }
               

            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "GreenHouseManager.Delete " + ex);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }
       
        public Entity.SearchResult<List<Entity.GreenHouseDetail>> List(Entity.SearchRequest request)
        {
            try
            {
                var result = _greenHouseRepository.List(request);
                return new Entity.SearchResult<List<Entity.GreenHouseDetail>>()
                {
                    Items = result.Items.Select(p => Mapper.Configuration.Mapper.Map<Entity.GreenHouseDetail>(p)).ToList(),
                    Count = result.Count
                };
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, $"GreenHouseService.List, Error: {ex.Message}");
                return new Entity.SearchResult<List<Entity.GreenHouseDetail>>();
            }
        }
        public Entity.ActionStatus UpdateStatus(Guid id, bool status)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                var dbGreenHouse = _greenHouseRepository.FindBy(x => x.Guid.Equals(id)).FirstOrDefault();
                if (dbGreenHouse == null)
                {
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : GreenHouse");
                }
                var dbCrop = _cropRepository.FindBy(x => x.GreenHouseGuid.Equals(id)).FirstOrDefault();
                var dbDevice = _deviceRepository.FindBy(x => x.GreenHouseGuid.Equals(id)).FirstOrDefault();
                if (dbCrop == null && dbDevice == null)
                {

                    dbGreenHouse.IsActive = status;
                    dbGreenHouse.UpdatedDate = DateTime.Now;
                    dbGreenHouse.UpdatedBy = AppConfig.CurrentUserId;
                    return _greenHouseRepository.Update(dbGreenHouse);
                }
                else
                {
                    _logger.Error($"GreenHouse is not updated from solution database.Crop/Device exists, Error: {actionStatus.Message}");
                    actionStatus.Success = false;
                    actionStatus.Message = "GreenHouse is not updated from solution database.Crop/Device exists";
                }
                

            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "UserManager.Delete " + ex);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }
        public Response.GreenHouseDetailResponse GetGreenHouseDetail(Guid greenhouseId)
        {
            return new Response.GreenHouseDetailResponse()
            {
                EnergyUsage = 2700,
                Temperature = 73,
                Moisture = 15,
                Humidity = 62,
                WaterUsage = 3800,
                TotalDevices = 15
            };
        }
        public List<Response.GreenHouseCropResponse> GetGreenHouseCorps(Guid greenhouseId)
        {
            try
            {
                return _cropRepository.GetGreenHouseCorps(greenhouseId);
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, $"GetGreenHouseCorps.List, Error: {ex.Message}");
                return null;
            }
        }
    }
}
