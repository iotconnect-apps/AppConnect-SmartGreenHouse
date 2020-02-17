using component.common.model;
using component.common.model.CommonModel;
using component.exception;
using component.logger;
using iot.solution.model.Repository.Interface;
using iot.solution.service.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Entity = iot.solution.entity;
using Model = iot.solution.model.Models;

namespace iot.solution.service.Implementation
{
    public class CropService : ICropService
    {
        private readonly ICropRepository _cropRepository;
        private readonly ILogger _logger;
        public CropService(ICropRepository cropRepository, ILogger logManager)
        {
            _cropRepository = cropRepository;
            _logger = logManager;
        }
        public List<Entity.Crop> Get()
        {
            try
            {
                return _cropRepository.GetAll().Where(e => !e.IsDeleted).Select(p => Mapper.Configuration.Mapper.Map<Entity.Crop>(p)).ToList();
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "CropManager.Get " + ex);
                return null;
            }
        }
        public Entity.Crop Get(Guid id)
        {
            try
            {
                return _cropRepository.FindBy(r => r.Guid == id).Select(p => Mapper.Configuration.Mapper.Map<Entity.Crop>(p)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "CropManager.Get " + ex);
                return null;
            }
        }
        public Entity.ActionStatus Manage(Entity.CropModel crop)
        {
            try
            {

                Entity.Crop cropEntity = Mapper.Configuration.Mapper.Map<Entity.CropModel, Entity.Crop>(crop);
                var dbCrop = Mapper.Configuration.Mapper.Map<Entity.Crop, Model.Crop>(cropEntity);
                Entity.ActionStatus actionStatus = null;
                if (crop.Guid == null || crop.Guid == Guid.Empty)
                {
                    dbCrop.Guid = Guid.NewGuid();
                    if (crop.ImageFile != null)
                    {
                        // upload image                                     
                        dbCrop.Image = SaveCropImage(dbCrop.Guid, crop.ImageFile);
                    }
                    dbCrop.CompanyGuid = AppConfig.CompanyId;
                    dbCrop.CreatedDate = DateTime.Now;
                    dbCrop.CreatedBy = AppConfig.CurrentUserId;
                    actionStatus = _cropRepository.Insert(dbCrop);
                    actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.Crop, Entity.Crop>(actionStatus.Data);
                }
                else
                {
                    var uniqGreenhouse = _cropRepository.GetByUniqueId(x => x.Guid == dbCrop.Guid);
                    if (uniqGreenhouse == null)
                        throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : Crop");
                    if (crop.ImageFile != null)
                    {
                        if (File.Exists(AppConfig.UploadBasePath + uniqGreenhouse.Image) && crop.ImageFile.Length > 0)
                        {
                            //if already exists image then delete  old image from server
                            File.Delete(AppConfig.UploadBasePath + uniqGreenhouse.Image);
                        }
                        if (crop.ImageFile.Length > 0)
                        {
                            // upload new image                                     
                            dbCrop.Image = SaveCropImage(dbCrop.Guid, crop.ImageFile);
                        }
                    }
                    else { 
                        dbCrop.Image = uniqGreenhouse.Image;
                    }
                    dbCrop.CreatedDate = uniqGreenhouse.CreatedDate;
                    dbCrop.CreatedBy = uniqGreenhouse.CreatedBy;
                    dbCrop.UpdatedDate = DateTime.Now;
                    dbCrop.UpdatedBy = AppConfig.CurrentUserId;
                    dbCrop.CompanyGuid = AppConfig.CompanyId;
                    actionStatus = _cropRepository.Update(dbCrop);
                    actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.Crop, Entity.Crop>(actionStatus.Data);
                }
                return actionStatus;
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "Crop.Manage " + ex);
                return new Entity.ActionStatus
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
        // Saving Image on Server   
        private string SaveCropImage(Guid guid, IFormFile image) 
        {
            var fileBasePath = AppConfig.UploadBasePath + AppConfig.CropImageBasePath;
            string extension = Path.GetExtension(image.FileName);
            var filePath = Path.Combine(fileBasePath, guid +  extension);
            if (image !=null && image.Length > 0)
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    image.CopyTo(fileStream);
                }                
                return Path.Combine(AppConfig.CropImageBasePath, guid +  extension);
            }
            return null;
        }
        public Entity.ActionStatus Delete(Guid id)
        {
            try
            {
                //TODO: NEED TO IMPLEMENT RDK CALLS
                var dbUser = _cropRepository.GetByUniqueId(x => x.Guid == id);
                if (dbUser == null)
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : Crop");
                dbUser.IsDeleted = true;
                dbUser.UpdatedDate = DateTime.Now;
                dbUser.UpdatedBy = AppConfig.CurrentUserId;
                return _cropRepository.Update(dbUser);
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "CropManager.Delete " + ex);
                return new Entity.ActionStatus
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
        public Entity.ActionStatus UpdateStatus(Guid id, bool status)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                var dbCorp = _cropRepository.FindBy(x => x.Guid.Equals(id)).FirstOrDefault();
                if (dbCorp == null)
                {
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : Corp");
                }

                //var updatedbStatusResult = _iotConnectClient.Device.UpdateStatus(dbDevice.Guid.ToString(), status).Result;
                //if (updatedbStatusResult != null && updatedbStatusResult.status == (int)HttpStatusCode.OK)
                //{
                dbCorp.IsActive = status;
                dbCorp.UpdatedDate = DateTime.Now;
                dbCorp.UpdatedBy = AppConfig.CurrentUserId;
                return _cropRepository.Update(dbCorp);
                //}
                //else
                //{
                //    _logger.Error($"Device status is not updated in iotconnect, Error: {updatedbStatusResult.message}");
                //    actionStatus.Success = false;
                //    actionStatus.Message = "Something Went Wrong!";
                //}

            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "CorpService.Delete " + ex);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }
        public Entity.SearchResult<List<Entity.Crop>> List(Entity.SearchRequest request)
        {
            try
            {
                var result = _cropRepository.List(request);
                return new Entity.SearchResult<List<Entity.Crop>>()
                {
                    Items = result.Items.Select(p => Mapper.Configuration.Mapper.Map<Entity.Crop>(p)).ToList(),
                    Count = result.Count
                };
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, $"CropService.List, Error: {ex.Message}");
                return new Entity.SearchResult<List<Entity.Crop>>();
            }
        }
        public List<Entity.Crop> GetCrops(Guid greenhouseId)
        {
            try
            {
                return _cropRepository.FindBy(e => e.GreenHouseGuid == greenhouseId && !e.IsDeleted && e.IsActive).Select(c => Mapper.Configuration.Mapper.Map<Entity.Crop>(c)).ToList();
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "CropService.GetGetCrops " + ex);
                return new List<Entity.Crop>();
            }
        }
    }
}
