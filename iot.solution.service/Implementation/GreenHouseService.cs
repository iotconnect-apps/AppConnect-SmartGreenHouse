using component.helper;
using iot.solution.common;
using iot.solution.model.Repository.Interface;
using iot.solution.service.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Entity = iot.solution.entity;
using IOT = IoTConnect.Model;
using LogHandler = component.services.loghandler;
using Model = iot.solution.model.Models;
using Response = iot.solution.entity.Response;

namespace iot.solution.service.Implementation
{
    public class GreenHouseService : IGreenHouseService
    {
        private readonly IGreenHouseRepository _greenHouseRepository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly ICropRepository _cropRepository;
        
        private readonly IotConnectClient _iotConnectClient;
        private readonly LogHandler.Logger _logger;

        public GreenHouseService(IGreenHouseRepository greenHouseRepository, ICropRepository cropRepository, LogHandler.Logger logger,IDeviceRepository deviceRepository)
        {
            _logger = logger;
            _greenHouseRepository = greenHouseRepository;
            _cropRepository = cropRepository;
            _deviceRepository = deviceRepository;
            _iotConnectClient = new IotConnectClient(SolutionConfiguration.BearerToken, SolutionConfiguration.Configuration.EnvironmentCode, SolutionConfiguration.Configuration.SolutionKey);
        }
        public List<Entity.GreenHouse> Get(bool isAdmin,bool? isActive)
        {
            try
            {
                if(isAdmin)
                    return _greenHouseRepository.GetAll().Where(e => !e.IsDeleted && (isActive.HasValue? e.IsActive.Value == isActive:true)).Select(p => Mapper.Configuration.Mapper.Map<Entity.GreenHouse>(p)).ToList();
                else
                    return _greenHouseRepository.GetAll().Where(e => !e.IsDeleted && (isActive.HasValue ? e.IsActive.Value == isActive : true) && e.CompanyGuid == SolutionConfiguration.CompanyId).Select(p => Mapper.Configuration.Mapper.Map<Entity.GreenHouse>(p)).ToList();
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
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
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
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
                        dbGreenHouse.CompanyGuid = SolutionConfiguration.CompanyId;
                        dbGreenHouse.CreatedDate = DateTime.Now;
                        dbGreenHouse.CreatedBy = SolutionConfiguration.CurrentUserId;
                        actionStatus = _greenHouseRepository.Manage(dbGreenHouse);
                        actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.GreenHouse, Entity.GreenHouse>(actionStatus.Data);
                        if (!actionStatus.Success)
                        {
                            _logger.ErrorLog(new Exception($"GreenHouse is not added in solution database, Error: {actionStatus.Message}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                            var deleteEntityResult = _iotConnectClient.GreenHouse.Delete(request.Guid.ToString()).Result;
                            if (deleteEntityResult != null && deleteEntityResult.status)
                            {
                                _logger.ErrorLog(new Exception($"GreenHouse is not deleted from iotconnect, Error: {deleteEntityResult.message}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                                actionStatus.Success = false;
                                actionStatus.Message = "Something Went Wrong!";
                                
                            }
                        }
                    }
                    else
                    {
                        _logger.ErrorLog(new Exception($"GreenHouse is not added in iotconnect, Error: {addEntityResult.message}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                        actionStatus.Success = false;
                        actionStatus.Message = new UtilityHelper().IOTResultMessage(addEntityResult.errorMessages);
                      
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
                        var existingImage = olddbGreenHouse.Image;
                        var dbGreenHouse = Mapper.Configuration.Mapper.Map(request, olddbGreenHouse);
                        if (request.ImageFile != null)
                        {
                            if (File.Exists(SolutionConfiguration.UploadBasePath + dbGreenHouse.Image) && request.ImageFile.Length > 0)
                            {
                                //if already exists image then delete  old image from server
                                File.Delete(SolutionConfiguration.UploadBasePath + dbGreenHouse.Image);
                            }
                            if (request.ImageFile.Length > 0)
                            {
                                // upload new image                                     
                                dbGreenHouse.Image = SaveGreenHouseImage(dbGreenHouse.Guid, request.ImageFile);
                            }
                        }
                        else
                        {
                            dbGreenHouse.Image = existingImage;
                        }
                        dbGreenHouse.UpdatedDate = DateTime.Now;
                        dbGreenHouse.UpdatedBy = SolutionConfiguration.CurrentUserId;
                        dbGreenHouse.CompanyGuid = SolutionConfiguration.CompanyId;
                        actionStatus = _greenHouseRepository.Manage(dbGreenHouse);
                        actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.GreenHouse, Entity.GreenHouse>(dbGreenHouse);
                        if (!actionStatus.Success)
                        {
                            _logger.ErrorLog(new Exception($"GreenHouse is not updated in solution database, Error: {actionStatus.Message}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                            actionStatus.Success = false;
                            actionStatus.Message = "Something Went Wrong!";
                        }
                    }
                    else
                    {
                        _logger.ErrorLog(new Exception($"GreenHouse is not updated in iotconnect, Error: {updateEntityResult.message}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                        actionStatus.Success = false;
                        actionStatus.Message = new UtilityHelper().IOTResultMessage(updateEntityResult.errorMessages);

                    }

                }
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }
        // Saving Image on Server   
        private string SaveGreenHouseImage(Guid guid, IFormFile image)
        {
            var fileBasePath = SolutionConfiguration.UploadBasePath + SolutionConfiguration.GreenHouseImageBasePath;
            bool exists = System.IO.Directory.Exists(fileBasePath);
            if (!exists)
                System.IO.Directory.CreateDirectory(fileBasePath);
            string extension = Path.GetExtension(image.FileName);
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            string fileName = guid.ToString() + "_" + unixTimestamp;
            var filePath = Path.Combine(fileBasePath, fileName + extension);
            
            if (image != null && image.Length > 0 && SolutionConfiguration.AllowedImages.Contains(extension.ToLower()))
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    image.CopyTo(fileStream);
                }
                return Path.Combine(SolutionConfiguration.GreenHouseImageBasePath, fileName + extension);
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
                        dbGreenHouse.UpdatedBy = SolutionConfiguration.CurrentUserId;
                        return _greenHouseRepository.Update(dbGreenHouse);
                    }
                    else
                    {
                        _logger.ErrorLog(new Exception($"GreenHouse is not deleted from iotconnect, Error: {deleteEntityResult.message}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                        actionStatus.Success = false;
                        actionStatus.Message = new UtilityHelper().IOTResultMessage(deleteEntityResult.errorMessages);
                    }
                }
                else
                {
                    _logger.ErrorLog(new Exception($"GreenHouse is not deleted from solution database.Crop/Device exists, Error: {actionStatus.Message}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                    actionStatus.Success = false;
                    actionStatus.Message = "GreenHouse is not deleted from solution database.Crop/Device exists";
                }
               

            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
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
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
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
                else
                {
                    dbGreenHouse.IsActive = status;
                    dbGreenHouse.UpdatedDate = DateTime.Now;
                    dbGreenHouse.UpdatedBy = SolutionConfiguration.CurrentUserId;
                    return _greenHouseRepository.Update(dbGreenHouse);
                }
                //var dbCrop = _cropRepository.FindBy(x => x.GreenHouseGuid.Equals(id)).FirstOrDefault();
                //var dbDevice = _deviceRepository.FindBy(x => x.GreenHouseGuid.Equals(id)).FirstOrDefault();
                //if (dbCrop == null && dbDevice == null)
                //{
                //    dbGreenHouse.IsActive = status;
                //    dbGreenHouse.UpdatedDate = DateTime.Now;
                //    dbGreenHouse.UpdatedBy = SolutionConfiguration.CurrentUserId;
                //    return _greenHouseRepository.Update(dbGreenHouse);
                //}
                //else
                //{
                //    _logger.ErrorLog(new Exception($"GreenHouse is not updated from solution database.Crop/Device exists, Error: {actionStatus.Message}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                //    actionStatus.Success = false;
                //    actionStatus.Message = "GreenHouse is not updated from solution database.Crop/Device exists";
                //}
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }
        public Response.GreenHouseDetailResponse GetGreenHouseDetail(Guid greenhouseId)
        {
            Response.GreenHouseDetailResponse result = new Response.GreenHouseDetailResponse();
            try
            {
                result = new Response.GreenHouseDetailResponse()
                {
                    EnergyUsage = 2700,
                    Temperature = 73,
                    Moisture = 15,
                    Humidity = 62,
                    WaterUsage = 3800,
                    TotalDevices = 15
                };
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }
        public List<Response.GreenHouseCropResponse> GetGreenHouseCorps(Guid greenhouseId)
        {
            try
            {
                return _cropRepository.GetGreenHouseCorps(greenhouseId);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }
    }
}
