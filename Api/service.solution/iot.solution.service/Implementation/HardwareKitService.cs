using System;
using System.Collections.Generic;
using System.Text;
using Entity = iot.solution.entity;
using Model = iot.solution.model.Models;
using component.common.model;
using component.common.model.CommonModel;
using component.exception;
using component.logger;
using iot.solution.service.Interface;
using iot.solution.model.Repository.Interface;
using System.Linq;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Linq;
using iot.solution.entity;

namespace iot.solution.service.Implementation
{
    public class HardwareKitService : IHardwareKitService
    {
        private readonly IHardwareKitRepository _hardwareKitRepository;
        private readonly IKitDeviceRepository _kitDeviceRepository;
        private readonly IKitTypeRepository _kitTypeRepository;
        private readonly ILogger _logger;

        public HardwareKitService(IHardwareKitRepository hardwareKitRepository, IKitDeviceRepository kitDeviceRepository, IKitTypeRepository kitTypeRepository, ILogger logManager)
        {
            _hardwareKitRepository = hardwareKitRepository;
            _kitDeviceRepository = kitDeviceRepository;
            _kitTypeRepository = kitTypeRepository;
            _logger = logManager;
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
                _logger.Error(Constants.ACTION_EXCEPTION, $"HardwareKitService.List, Error: {ex.Message}");
                return new Entity.SearchResult<List<Entity.HardwareKitResponse>>();
            }
        }
        public Entity.HardwareKitDTO Get(Guid id)
        {
            var hardwareKitDTO = new Entity.HardwareKitDTO();
            try
            {
                hardwareKitDTO = _hardwareKitRepository.GetHardwareKitDetails(new Entity.SearchRequest
                {
                    Guid = id.ToString().ToUpper(),
                    Version = AppConfig.Version,
                    CompanyId = AppConfig.CompanyId.ToString().ToUpper(),
                    InvokingUser = AppConfig.CurrentUserId,
                    PageNumber = 1,
                    PageSize = 10
                });

            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, $"HardwareKitService.List, Error: {ex.Message}");
            }

            return hardwareKitDTO;


        }
        public Entity.ActionStatus Manage(Entity.KitVerifyRequest hardwareKit, bool isEdit = false)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                var verifyResult = _hardwareKitRepository.VerifyHardwareKit(hardwareKit, isEdit);

                if (verifyResult.Success)
                {
                    actionStatus = _hardwareKitRepository.SaveHardwareKit(hardwareKit, isEdit);
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
                _logger.Error(Constants.ACTION_EXCEPTION, "HardwareKit.Manage " + ex);
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
                dbHardwareKit.UpdatedBy = AppConfig.CurrentUserId;
                return _hardwareKitRepository.Update(dbHardwareKit);
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "HardwareKit.Delete " + ex);
                return new Entity.ActionStatus
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        //public Entity.ActionStatus UploadKit(Entity.KitVerifyRequest request)
        //{

        //    var result = new Entity.ActionStatus();
        //    var hwkitList = new List<Entity.HardwareKitDTO>();
        //    var tetetete = request.GroupBy(c => c.KitCode).Select(x => new Entity.HardwareKitDTO() { KitCode = x.Key }).ToList();

        //    try
        //    {
        //        foreach (var item in tetetete)
        //        {
        //            var kitDevices = request.Where(x => x.KitCode.Equals(item.KitCode)).Select(x => new Entity.KitDeviceDTO
        //            {
        //                Name = x.Name,
        //                ParentUniqueId = x.ParentUniqueId,
        //                KitCode = x.KitCode,
        //                Note = x.Note,
        //                Tag = x.Tag,
        //                UniqueId = x.UniqueId
        //            }).ToList();
        //            hwkitList.Add(new Entity.HardwareKitDTO()
        //            {
        //                KitCode = item.KitCode,
        //                KitTypeGuid = new Guid(),
        //                KitDevices = kitDevices
        //            });
        //        }

        //        var uploadResult = _hardwareKitRepository.UploadHardwareKit(hwkitList);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error(Constants.ACTION_EXCEPTION, "HardwareKit.Upload " + ex);
        //        return new Entity.ActionStatus
        //        {
        //            Success = false,
        //            Message = ex.Message
        //        };
        //    }
        //    return result;
        //}

        public ActionStatus VerifyKit(KitVerifyRequest request, bool isEdit = false)
        {
            var result = new Entity.ActionStatus();
            try
            {
                result = _hardwareKitRepository.VerifyHardwareKit(request, isEdit);
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "HardwareKit.Upload " + ex);
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
                _logger.Error(Constants.ACTION_EXCEPTION, "HardwareKit.Upload " + ex);
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




