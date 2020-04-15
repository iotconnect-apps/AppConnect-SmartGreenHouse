using iot.solution.common;
using iot.solution.entity;
using iot.solution.model.Repository.Interface;
using iot.solution.service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Entity = iot.solution.entity;
using LogHandler = component.services.loghandler;

namespace iot.solution.service.Implementation
{
    public class HardwareKitService : IHardwareKitService
    {
        private readonly IHardwareKitRepository _hardwareKitRepository;
        private readonly IKitDeviceRepository _kitDeviceRepository;
        private readonly IKitTypeRepository _kitTypeRepository;
        private readonly IKitTypeAttributeRepository _kitTypeAttributeRepository;
        private readonly LogHandler.Logger _logger;

        public HardwareKitService(IHardwareKitRepository hardwareKitRepository, IKitDeviceRepository kitDeviceRepository
            , IKitTypeRepository kitTypeRepository, IKitTypeAttributeRepository kitTypeAttributeRepository, LogHandler.Logger logger)
        {
            _hardwareKitRepository = hardwareKitRepository;
            _kitDeviceRepository = kitDeviceRepository;
            _kitTypeRepository = kitTypeRepository;
            _kitTypeAttributeRepository = kitTypeAttributeRepository;
            _logger = logger;
        }
        public Entity.SearchResult<List<Entity.HardwareKitResponse>> List(Entity.SearchRequest request, bool isAssigned)
        {
            try
            {
                var result = _hardwareKitRepository.List(request, isAssigned, null);
                return result;
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return new Entity.SearchResult<List<Entity.HardwareKitResponse>>();
            }
        }
        public Entity.HardwareKitDTO Get(Guid id)
        {
            //Entity.HardwareKit result = new Entity.HardwareKit();
            //try
            //{
            //    result = _hardwareKitRepository.FindBy(kt => kt.Guid.Equals(id)).Select(p => Mapper.Configuration.Mapper.Map<Entity.HardwareKit>(p)).FirstOrDefault();
            //    if (result != null)
            //    {
            //        result.KitDevices = _kitDeviceRepository.FindBy(kt => kt.KitGuid.Equals(id)).Select(p => Mapper.Configuration.Mapper.Map<Entity.KitDevice>(p)).ToList();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            //}
            //return result;

            var hardwareKitDTO = new Entity.HardwareKitDTO();
            try
            {
                hardwareKitDTO = _hardwareKitRepository.GetHardwareKitDetails(new Entity.SearchRequest
                {
                    Guid = id.ToString().ToUpper(),
                    Version = component.helper.SolutionConfiguration.Version,
                    CompanyId = component.helper.SolutionConfiguration.CompanyId.ToString().ToUpper(),
                    InvokingUser = component.helper.SolutionConfiguration.CurrentUserId,
                    PageNumber = 1,
                    PageSize = 10
                });

            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return hardwareKitDTO;
        }
        public Entity.ActionStatus Manage(Entity.KitVerifyRequest hardwareKit, bool isEdit = false)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                foreach (var kit in hardwareKit.HardwareKits)
                {
                    foreach (var device in kit.KitDevices)
                    {
                        if (string.IsNullOrWhiteSpace(device.Tag)) continue;

                        var tagGuid = Guid.Parse(device.Tag);
                        var kitAttribute = _kitTypeAttributeRepository.FindBy(t => t.Guid.Equals(tagGuid)).FirstOrDefault();
                        if (kitAttribute != null)
                        {
                            device.Tag = kitAttribute.Tag;
                            device.AttributeName = kitAttribute.LocalName;
                        }
                    }
                }

                var verifyResult = _hardwareKitRepository.VerifyHardwareKit(hardwareKit, isEdit);

                if (verifyResult.Success)
                {
                    actionStatus = _hardwareKitRepository.SaveHardwareKit(hardwareKit, isEdit);
                }
                else
                {                    
                    List<BulkUploadResponse> errorResult = verifyResult.Data;
                    if (errorResult != null) {
                        foreach (var error in errorResult)
                        {
                            if (string.IsNullOrWhiteSpace(error.tag)) continue;

                            var tag = error.tag;
                            var kitAttribute = _kitTypeAttributeRepository.FindBy(t => t.Tag.Equals(error.tag) && t.LocalName.Equals(error.attributename)).FirstOrDefault();
                            if (kitAttribute != null)
                            {
                                error.tag = kitAttribute.Guid.ToString();
                            }
                        }
                    }

                    actionStatus.Data = errorResult;// verifyResult.Data;
                    actionStatus.Success = verifyResult.Success;
                    actionStatus.Message = "Hardware kit already exist";
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
        public Entity.ActionStatus Delete(Guid id)
        {
            try
            {
                var dbHardwareKit = _hardwareKitRepository.GetByUniqueId(x => x.Guid == id);
                if (dbHardwareKit == null)
                {
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : HardwareKit");
                }
                dbHardwareKit.IsDeleted = true;
                dbHardwareKit.UpdatedDate = DateTime.Now;
                dbHardwareKit.UpdatedBy = component.helper.SolutionConfiguration.CurrentUserId;
                return _hardwareKitRepository.Update(dbHardwareKit);
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
        public ActionStatus VerifyKit(KitVerifyRequest request, bool isEdit = false)
        {
            Entity.ActionStatus result = new Entity.ActionStatus();
            try
            {
                result = _hardwareKitRepository.VerifyHardwareKit(request, isEdit);
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
            return result;
        }
        public ActionStatus UploadKit(KitVerifyRequest request)
        {
            var actionStatus = new Entity.ActionStatus();
            try
            {
                var verifyResult = _hardwareKitRepository.VerifyHardwareKit(request, false);

                if (verifyResult.Success)
                {
                    actionStatus = _hardwareKitRepository.SaveHardwareKit(request, false);
                }
                else
                {
                    actionStatus.Data = verifyResult.Data;
                    actionStatus.Success = verifyResult.Success;
                    actionStatus.Message = "Invalid Kit Details.";
                }
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
            return actionStatus;
        }
    }
}




