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
using LogHandler = component.services.loghandler;
using Model = iot.solution.model.Models;

namespace iot.solution.service.Implementation
{
    public class CropService : ICropService
    {
        private readonly ICropRepository _cropRepository;
        private readonly LogHandler.Logger _logger;
        public CropService(ICropRepository cropRepository, LogHandler.Logger logger)
        {
            _cropRepository = cropRepository;
            _logger = logger;
        }
        public List<Entity.Crop> Get()
        {
            try
            {
                return _cropRepository.GetAll().Where(e => !e.IsDeleted).Select(p => Mapper.Configuration.Mapper.Map<Entity.Crop>(p)).ToList();
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
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
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
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
                    var greehouseCrop = _cropRepository.FindBy(t => t.Name.Trim().Equals(crop.Name.Trim()) && t.GreenHouseGuid == crop.GreenhouseGuid && !t.IsDeleted).FirstOrDefault();
                    if (greehouseCrop == null)
                    {
                        dbCrop.Guid = Guid.NewGuid();
                        if (crop.ImageFile != null)
                        {
                            // upload image                                     
                            dbCrop.Image = SaveCropImage(dbCrop.Guid, crop.ImageFile);
                        }
                        dbCrop.CompanyGuid = component.helper.SolutionConfiguration.CompanyId;
                        dbCrop.CreatedDate = DateTime.Now;
                        dbCrop.CreatedBy = component.helper.SolutionConfiguration.CurrentUserId;
                        actionStatus = _cropRepository.Insert(dbCrop);
                        actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.Crop, Entity.Crop>(actionStatus.Data);
                    }
                    else {
                        return new Entity.ActionStatus
                        {
                            Success = false,
                            Message = "Crop name already exists!",
                            Data=null
                        };
                    }
                }
                else
                {
                    var uniqGreenhouse = _cropRepository.GetByUniqueId(x => x.Guid == dbCrop.Guid);
                    if (uniqGreenhouse == null)
                        throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : Crop");
                    var greehouseCrop = _cropRepository.FindBy(t => t.Name.Trim().Equals(crop.Name.Trim()) && t.Guid!=dbCrop.Guid && t.GreenHouseGuid == crop.GreenhouseGuid && !t.IsDeleted).FirstOrDefault();
                    if (greehouseCrop == null)
                    {
                        if (crop.ImageFile != null)
                        {
                            if (File.Exists(component.helper.SolutionConfiguration.UploadBasePath + uniqGreenhouse.Image) && crop.ImageFile.Length > 0)
                            {
                                //if already exists image then delete  old image from server
                                File.Delete(component.helper.SolutionConfiguration.UploadBasePath + uniqGreenhouse.Image);
                            }
                            if (crop.ImageFile.Length > 0)
                            {
                                // upload new image                                     
                                dbCrop.Image = SaveCropImage(dbCrop.Guid, crop.ImageFile);
                            }
                        }
                        else
                        {
                            dbCrop.Image = uniqGreenhouse.Image;
                        }
                        dbCrop.CreatedDate = uniqGreenhouse.CreatedDate;
                        dbCrop.CreatedBy = uniqGreenhouse.CreatedBy;
                        dbCrop.UpdatedDate = DateTime.Now;
                        dbCrop.UpdatedBy = component.helper.SolutionConfiguration.CurrentUserId;
                        dbCrop.CompanyGuid = component.helper.SolutionConfiguration.CompanyId;
                        actionStatus = _cropRepository.Update(dbCrop);
                        actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.Crop, Entity.Crop>(actionStatus.Data);
                    }
                    else
                    {
                        return new Entity.ActionStatus
                        {
                            Success = false,
                            Message = "Crop name already exists!",
                            Data = null
                        };
                    }
                }
                return actionStatus;
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
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
            var fileBasePath = component.helper.SolutionConfiguration.UploadBasePath + component.helper.SolutionConfiguration.CropImageBasePath;
            bool exists = System.IO.Directory.Exists(fileBasePath);
            if (!exists)
                System.IO.Directory.CreateDirectory(fileBasePath);
            string extension = Path.GetExtension(image.FileName);
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            string fileName = guid.ToString() + "_" + unixTimestamp;
            var filePath = Path.Combine(fileBasePath, fileName +  extension);
            if (image !=null && image.Length > 0 && SolutionConfiguration.AllowedImages.Contains(extension.ToLower()))
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    image.CopyTo(fileStream);
                }                
                return Path.Combine(component.helper.SolutionConfiguration.CropImageBasePath, fileName +  extension);
            }
            return null;
        }
        // Delete Image on Server   
        private bool DeleteEntityImage(Guid guid, string imageName)
        {
            var fileBasePath = SolutionConfiguration.UploadBasePath + SolutionConfiguration.CropImageBasePath;
            var filePath = Path.Combine(fileBasePath, imageName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            return true;
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
                dbUser.UpdatedBy = component.helper.SolutionConfiguration.CurrentUserId;
                return _cropRepository.Update(dbUser);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return new Entity.ActionStatus
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
        public Entity.ActionStatus DeleteImage(Guid id)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(false);
            try
            {
                var dbEntity = _cropRepository.FindBy(x => x.Guid.Equals(id)).FirstOrDefault();
                if (dbEntity == null)
                {
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : Entity");
                }

                bool deleteStatus = DeleteEntityImage(id, dbEntity.Image);
                if (deleteStatus)
                {
                    dbEntity.Image = "";
                    dbEntity.UpdatedDate = DateTime.Now;
                    dbEntity.UpdatedBy = SolutionConfiguration.CurrentUserId;
                    dbEntity.CompanyGuid = SolutionConfiguration.CompanyId;

                    actionStatus = _cropRepository.Update(dbEntity);
                    actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.Crop, Entity.Crop>(actionStatus.Data);
                    actionStatus.Success = true;
                    actionStatus.Message = "Image deleted successfully!";
                    if (!actionStatus.Success)
                    {
                        _logger.ErrorLog(new Exception($"Entity is not updated in database, Error: {actionStatus.Message}"));
                        actionStatus.Success = false;
                        actionStatus.Message = actionStatus.Message;
                    }
                }
                else
                {
                    actionStatus.Success = false;
                    actionStatus.Message = "Image not deleted!";
                }
                return actionStatus;
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
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

              
                dbCorp.IsActive = status;
                dbCorp.UpdatedDate = DateTime.Now;
                dbCorp.UpdatedBy = component.helper.SolutionConfiguration.CurrentUserId;
                return _cropRepository.Update(dbCorp);
               

            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
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
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
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
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return new List<Entity.Crop>();
            }
        }
    }
}
