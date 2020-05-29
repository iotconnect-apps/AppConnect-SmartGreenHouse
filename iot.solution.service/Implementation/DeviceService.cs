using component.helper;
using iot.solution.common;
using iot.solution.entity;
using iot.solution.model.Repository.Interface;
using iot.solution.service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Entity = iot.solution.entity;
using IOT = IoTConnect.Model;
using LogHandler = component.services.loghandler;
using Model = iot.solution.model.Models;
using Response = iot.solution.entity.Response;

namespace iot.solution.service.Implementation
{
    public class DeviceService : IDeviceService
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly IHardwareKitRepository _hardwareKitRepository;
        private readonly IKitTypeAttributeRepository _kitTypeAttributeRepository;
        private readonly IKitTypeRepository _kitTypeRepository;
        private readonly IotConnectClient _iotConnectClient;
        private readonly ILookupService _lookupService;
        private readonly LogHandler.Logger _logger;

        public DeviceService(ILookupService lookupService, IDeviceRepository deviceRepository, IKitTypeAttributeRepository kitTypeAttributeRepository
            , IKitTypeRepository kitTypeRepository, IHardwareKitRepository hardwareKitRepository, LogHandler.Logger logger)
        {
            _logger = logger;
            _deviceRepository = deviceRepository;
            _hardwareKitRepository = hardwareKitRepository;
            _kitTypeAttributeRepository = kitTypeAttributeRepository;
            _kitTypeRepository = kitTypeRepository;
            _lookupService = lookupService;
            _iotConnectClient = new IotConnectClient(SolutionConfiguration.BearerToken, SolutionConfiguration.Configuration.EnvironmentCode, SolutionConfiguration.Configuration.SolutionKey);
        }

        public List<Entity.Device> Get()
        {
            try
            {
                return _deviceRepository.GetAll().Where(e => !e.IsDeleted).Select(p => Mapper.Configuration.Mapper.Map<Entity.Device>(p)).ToList();
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }
        public Entity.Device Get(Guid id)
        {
            try
            {
                return _deviceRepository.FindBy(r => r.Guid == id).Select(p => Mapper.Configuration.Mapper.Map<Entity.Device>(p)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }
        public Entity.ActionStatus Manage(Entity.Device request)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                var dbDevice = Mapper.Configuration.Mapper.Map<Entity.Device, Model.Device>(request);
                if (request.Guid == null || request.Guid == Guid.Empty)
                {
                    var addDeviceResult = _iotConnectClient.Device.Add(Mapper.Configuration.Mapper.Map<IOT.AddDeviceModel>(request)).Result;
                    if (addDeviceResult != null && addDeviceResult.status && addDeviceResult.data != null)
                    {
                        request.Guid = Guid.Parse(addDeviceResult.data.newid.ToUpper());
                        IOT.DataResponse<IOT.AcquireDeviceResult> acquireResult = _iotConnectClient.Device.AcquireDevice(request.UniqueId, new IOT.AcquireDeviceModel()).Result;
                        dbDevice.IsProvisioned = acquireResult.status;
                        dbDevice.IsActive = true;
                        dbDevice.Guid = request.Guid;
                        dbDevice.CompanyGuid = SolutionConfiguration.CompanyId;
                        dbDevice.CreatedDate = DateTime.Now;
                        dbDevice.CreatedBy = SolutionConfiguration.CurrentUserId;
                        actionStatus = _deviceRepository.Manage(dbDevice);
                        actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.Device, Entity.Device>(actionStatus.Data);
                        if (!actionStatus.Success)
                        {
                            _logger.ErrorLog(new Exception($"Device is not added in solution database, Error: {actionStatus.Message}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                            var deleteEntityResult = _iotConnectClient.Device.Delete(request.Guid.ToString()).Result;
                            if (deleteEntityResult != null && deleteEntityResult.status)
                            {
                                _logger.ErrorLog(new Exception($"Device is not deleted from iotconnect"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                                actionStatus.Success = false;
                                actionStatus.Message = "Something Went Wrong!";
                            }
                        }
                    }
                    else
                    {
                        _logger.ErrorLog(new Exception($"Device is not added in iotconnect, Error: {addDeviceResult.message}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                        actionStatus.Success = false;
                        actionStatus.Message = new UtilityHelper().IOTResultMessage(addDeviceResult.errorMessages);

                    }

                }
                else
                {
                    var olddbDevice = _deviceRepository.FindBy(x => x.Guid.Equals(request.Guid)).FirstOrDefault();
                    if (olddbDevice == null)
                    {
                        throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : Device");
                    }
                    var updateEntityResult = _iotConnectClient.Device.Update(request.Guid.ToString(), Mapper.Configuration.Mapper.Map<IOT.UpdateDeviceModel>(request)).Result;
                    if (updateEntityResult != null && updateEntityResult.status && updateEntityResult.data != null)
                    {
                        dbDevice.CreatedDate = olddbDevice.CreatedDate;
                        dbDevice.CreatedBy = olddbDevice.CreatedBy;
                        dbDevice.UpdatedDate = DateTime.Now;
                        dbDevice.UpdatedBy = SolutionConfiguration.CurrentUserId;
                        dbDevice.CompanyGuid = SolutionConfiguration.CompanyId;
                        actionStatus = _deviceRepository.Manage(dbDevice);
                        actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.Device, Entity.Device>(actionStatus.Data);
                        if (!actionStatus.Success)
                        {
                            _logger.ErrorLog(new Exception($"Device is not updated in solution database, Error: {actionStatus.Message}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                            actionStatus.Success = false;
                            actionStatus.Message = "Something Went Wrong!";
                        }
                    }
                    else
                    {
                        _logger.ErrorLog(new Exception($"Device is not added in iotconnect, Error: {updateEntityResult.message}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                        actionStatus.Success = false;
                        actionStatus.Message = new UtilityHelper().IOTResultMessage(updateEntityResult.errorMessages);

                    }
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
        public Entity.ActionStatus Delete(Guid id)
        {

            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                var dbDevice = _deviceRepository.FindBy(x => x.Guid.Equals(id)).FirstOrDefault();
                if (dbDevice == null)
                {
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : Device");
                }

                var deleteEntityResult = _iotConnectClient.Device.Delete(id.ToString()).Result;
                if (deleteEntityResult != null && deleteEntityResult.status)
                {
                    dbDevice.IsDeleted = true;
                    dbDevice.UpdatedDate = DateTime.Now;
                    dbDevice.UpdatedBy = SolutionConfiguration.CurrentUserId;
                    return _deviceRepository.Update(dbDevice);
                }
                else
                {
                    _logger.ErrorLog(new Exception($"Device is not deleted from iotconnect, Error: {deleteEntityResult.message}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                    actionStatus.Success = false;
                    actionStatus.Message = new UtilityHelper().IOTResultMessage(deleteEntityResult.errorMessages);

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
        public Entity.ActionStatus UpdateStatus(Guid id, bool status)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                var dbDevice = _deviceRepository.FindBy(x => x.Guid.Equals(id)).FirstOrDefault();
                if (dbDevice == null)
                {
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : Device");
                }

                var updatedbStatusResult = _iotConnectClient.Device.UpdateDeviceStatus(dbDevice.Guid.ToString(), new IOT.UpdateDeviceStatusModel() { IsActive = status }).Result;
                if (updatedbStatusResult != null && updatedbStatusResult.status)
                {
                    dbDevice.IsActive = status;
                    dbDevice.UpdatedDate = DateTime.Now;
                    dbDevice.UpdatedBy = SolutionConfiguration.CurrentUserId;
                    return _deviceRepository.Update(dbDevice);
                }
                else
                {
                    _logger.ErrorLog(new Exception($"Device status is not updated in iotconnect, Error: {updatedbStatusResult.message}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                    actionStatus.Success = false;
                    actionStatus.Message = new UtilityHelper().IOTResultMessage(updatedbStatusResult.errorMessages);

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
        public Entity.ActionStatus AcquireDevice(string deviceUniqueId)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                IOT.DataResponse<IOT.AcquireDeviceResult> acquireResult = _iotConnectClient.Device.AcquireDevice(deviceUniqueId, new IOT.AcquireDeviceModel()).Result;
                if (acquireResult != null && acquireResult.status)
                {
                    var dbDevice = _deviceRepository.FindBy(x => x.UniqueId.Equals(deviceUniqueId)).FirstOrDefault();
                    dbDevice.IsProvisioned = true;
                    dbDevice.UpdatedDate = DateTime.Now;
                    dbDevice.UpdatedBy = SolutionConfiguration.CurrentUserId;
                    return _deviceRepository.Update(dbDevice);
                }
                else
                {
                    return new ActionStatus(false, acquireResult.message);
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }
        public Entity.SearchResult<List<Entity.Device>> List(Entity.SearchRequest request)
        {
            try
            {
                Entity.SearchResult<List<Model.Device>> result = _deviceRepository.List(request);
                return new Entity.SearchResult<List<Entity.Device>>()
                {
                    Items = result.Items.Select(p => Mapper.Configuration.Mapper.Map<Entity.Device>(p)).ToList(),
                    Count = result.Count
                };
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return new Entity.SearchResult<List<Entity.Device>>();
            }
        }
        public List<Entity.Device> GetGreenHouseDeviceList(Guid greenhouseId)
        {
            try
            {
                return _deviceRepository.FindBy(e => e.GreenHouseGuid == greenhouseId && !e.IsDeleted && e.IsActive).Select(c => Mapper.Configuration.Mapper.Map<Entity.Device>(c)).ToList();
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return new List<Entity.Device>();
            }
        }
        public Entity.SearchResult<List<Entity.DeviceDetailResponse>> GetGreenHouseDeviceDetailList(Entity.SearchRequest request)
        {
            try
            {
                Entity.SearchResult<List<Entity.DeviceDetailResponse>> result = _deviceRepository.DetailList(request);
                return new Entity.SearchResult<List<Entity.DeviceDetailResponse>>()
                {
                    Items = result.Items.Select(p => Mapper.Configuration.Mapper.Map<Entity.DeviceDetailResponse>(p)).ToList(),
                    Count = result.Count
                };
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return new Entity.SearchResult<List<Entity.DeviceDetailResponse>>();
            }
        }
        public Entity.SearchResult<List<Entity.DeviceSearchResponse>> GatewayList(Entity.SearchRequest request)
        {
            try
            {
                var result = _deviceRepository.GatewayList(request);
                return new Entity.SearchResult<List<Entity.DeviceSearchResponse>>()
                {
                    Items = result.Items.Select(p => Mapper.Configuration.Mapper.Map<Entity.DeviceSearchResponse>(p)).ToList(),
                    Count = result.Count
                };
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return new Entity.SearchResult<List<Entity.DeviceSearchResponse>>();
            }
        }
        public SearchResult<List<Device>> ChildDeviceList(SearchRequest request)
        {
            try
            {
                var result = _deviceRepository.GetChildDevice(request);
                return new Entity.SearchResult<List<Entity.Device>>()
                {
                    Items = result.Items.Select(p => Mapper.Configuration.Mapper.Map<Entity.Device>(p)).ToList(),
                    Count = result.Count
                };
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return new Entity.SearchResult<List<Entity.Device>>();
            }
        }
        public Entity.BaseResponse<Response.DeviceDetailResponse> GetDeviceDetail(Guid deviceId)
        {
            Entity.BaseResponse<List<Response.DeviceDetailResponse>> listResult = new Entity.BaseResponse<List<Response.DeviceDetailResponse>>();
            Entity.BaseResponse<Response.DeviceDetailResponse> result = new Entity.BaseResponse<Response.DeviceDetailResponse>(true);
            try
            {
                listResult = _deviceRepository.GetStatistics(deviceId);
                if (listResult.Data.Count > 0)
                {
                    result.IsSuccess = true;
                    result.Data = listResult.Data[0];
                    result.LastSyncDate = listResult.LastSyncDate;
                }

            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;

        }
        public List<Response.GreenHouseDevicesResponse> GetGreenHouseDevices(Guid greenhouseId)
        {
            try
            {
                return _deviceRepository.GetGreenHouseDevices(greenhouseId, null);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }
        public List<Response.GreenHouseDevicesResponse> GetGreenHouseChildDevices(Guid deviceId)
        {
            try
            {
                return _deviceRepository.GetGreenHouseDevices(null, deviceId);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }
        public Entity.BaseResponse<int> ValidateKit(string kitCode)
        {
            Entity.BaseResponse<int> result = new Entity.BaseResponse<int>(true);
            try
            {
                return _deviceRepository.ValidateKit(kitCode);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return null;
            }

        }
        public Entity.BaseResponse<bool> ProvisionKit(Entity.ProvisionKitRequest request)
        {
            Entity.BaseResponse<bool> result = new Entity.BaseResponse<bool>(true);
            try
            {
                Entity.BaseResponse<List<Entity.KitDevice>> kitDeviceList = _deviceRepository.ProvisionKit(request);

                if (kitDeviceList != null && kitDeviceList.Data != null && kitDeviceList.Data.Any())
                {
                    List<Entity.KitDevice> deviceList = kitDeviceList.Data.OrderBy(d => d.ParentDeviceGuid.HasValue).ToList();
                    string templateGuid = _lookupService.GetIotTemplateGuidByName(deviceList.FirstOrDefault().TemplateName);

                    if (!string.IsNullOrEmpty(templateGuid))
                    {
                        List<Model.Device> lstAddedDevice = new List<Model.Device>();
                        bool IsDeviceAdded = true;
                        foreach (var device in deviceList)
                        {
                            string tagName = string.Empty;
                            if (device.ParentDeviceGuid.HasValue)
                            {
                                var kitType = _kitTypeAttributeRepository.FindBy(t => t.Guid == device.TagGuid).FirstOrDefault();
                                if (kitType != null)
                                {
                                    tagName = kitType.Tag.ToString();
                                }
                                else
                                {
                                    throw new Exception("Device tag is not exists in solution.");
                                }
                            }

                            IOT.AddDeviceModel iotDeviceDetail = new IOT.AddDeviceModel()
                            {
                                DisplayName = device.Name,
                                entityGuid = request.GreenHouseGuid.ToString(),
                                uniqueId = device.UniqueId,
                                deviceTemplateGuid = templateGuid,// device.TemplateGuid.ToString(),
                                parentDeviceGuid = device.ParentDeviceGuid.ToString(),
                                note = device.Note,
                                tag = tagName,//device.Tag,
                                properties = new List<IOT.AddProperties>()

                            };
                            var addDeviceResult = _iotConnectClient.Device.Add(iotDeviceDetail).Result;
                            if (addDeviceResult != null && addDeviceResult.status && addDeviceResult.data != null)
                            {
                                Guid newDeviceId = Guid.Parse(addDeviceResult.data.newid.ToUpper());
                                IOT.DataResponse<IOT.AcquireDeviceResult> acquireResult = _iotConnectClient.Device.AcquireDevice(device.UniqueId, new IOT.AcquireDeviceModel()).Result;
                                var intUpdated = deviceList.Where(d => d.ParentDeviceGuid == device.Guid).Select(d => d.ParentDeviceGuid = newDeviceId).Count();

                                lstAddedDevice.Add(new Model.Device()
                                {
                                    Guid = newDeviceId,
                                    CompanyGuid = SolutionConfiguration.CompanyId,
                                    GreenHouseGuid = request.GreenHouseGuid,
                                    TemplateGuid = device.TemplateGuid,
                                    ParentDeviceGuid = device.ParentDeviceGuid,
                                    Type = null,
                                    UniqueId = device.UniqueId,
                                    Name = device.Name,
                                    Note = device.Note,
                                    Tag = tagName,//device.Tag,
                                    IsProvisioned = acquireResult.status,
                                    IsActive = true,
                                    IsDeleted = false,
                                    CreatedDate = DateTime.UtcNow,
                                    CreatedBy = SolutionConfiguration.CurrentUserId
                                });
                            }
                            else
                            {
                                IsDeviceAdded = false;
                                _logger.ErrorLog(new Exception($"Kit is not added in iotconnect, Error: {addDeviceResult.message}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                                result.IsSuccess = false;
                                result.Message = new UtilityHelper().IOTResultMessage(addDeviceResult.errorMessages);
                                break;
                            }
                        }

                        if (IsDeviceAdded && lstAddedDevice != null && lstAddedDevice.Any())
                        {
                            foreach (var device in lstAddedDevice)
                            {
                                Entity.ActionStatus actionStatus = _deviceRepository.Manage(device);
                                if (!actionStatus.Success)
                                {
                                    _logger.ErrorLog(new Exception($"Device is not added in solution database, Error: {actionStatus.Message}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                                    var deleteEntityResult = _iotConnectClient.Device.Delete(device.Guid.ToString()).Result;
                                    if (deleteEntityResult != null && deleteEntityResult.status)
                                    {
                                        _logger.ErrorLog(new Exception($"Device is not deleted from iotconnect"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                                        actionStatus.Success = false;
                                        actionStatus.Message = "Something Went Wrong!";
                                    }
                                }
                            }
                        }

                        if (result.IsSuccess)
                        {
                            result.Message = "Kit Added Successfully!";
                            //Update companyid in hardware kit
                            var hardwareKit = _hardwareKitRepository.GetByUniqueId(t => t.KitCode == request.KitCode);
                            if (hardwareKit != null)
                            {
                                hardwareKit.CompanyGuid = SolutionConfiguration.CompanyId;
                                _hardwareKitRepository.Update(hardwareKit);
                            }
                        }
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Data = false;
                        result.Message = "Unable To Locate Kit Type.";
                    }

                }
                else
                {
                    result.IsSuccess = false;
                    result.Data = false;
                    result.Message = "Invalid Kit Details.Please Correct It!";
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return null;
            }
            return result;
        }
        public Entity.BaseResponse<Entity.DeviceCounterResult> GetDeviceCounters()
        {
            Entity.BaseResponse<Entity.DeviceCounterResult> result = new Entity.BaseResponse<Entity.DeviceCounterResult>(true);
            try
            {
                IOT.DataResponse<List<IOT.DeviceCounterResult>> deviceCounterResult = _iotConnectClient.Device.GetDeviceCounters("").Result;
                if (deviceCounterResult != null && deviceCounterResult.status)
                {
                    result.Data = Mapper.Configuration.Mapper.Map<Entity.DeviceCounterResult>(deviceCounterResult.data.FirstOrDefault());
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return new Entity.BaseResponse<Entity.DeviceCounterResult>(false, ex.Message);
            }
            return result;
        }
        public Entity.BaseResponse<List<Entity.DeviceTelemetryDataResult>> GetTelemetryData(Guid deviceId)
        {
            Entity.BaseResponse<List<Entity.DeviceTelemetryDataResult>> result = new Entity.BaseResponse<List<Entity.DeviceTelemetryDataResult>>(true);
            try
            {
                var childDevices = _deviceRepository.FindBy(x => x.ParentDeviceGuid.Value.Equals(deviceId) && !x.IsDeleted).ToList();

                if (childDevices != null && childDevices.Count > 0)
                {
                    result.Data = new List<DeviceTelemetryDataResult>();
                    foreach (var device in childDevices)
                    {
                        IOT.DataResponse<List<IOT.DeviceTelemetryData>> deviceCounterResult = _iotConnectClient.Device.GetTelemetryData(device.Guid.ToString()).Result;
                        if (deviceCounterResult != null && deviceCounterResult.status)
                        {
                            result.Data.AddRange(deviceCounterResult.data.Select(d => Mapper.Configuration.Mapper.Map<Entity.DeviceTelemetryDataResult>(d)).ToList());
                        }
                    }
                }
                else
                {
                   result.Data = null;
                   result.Message = "No Child Device Found!";
                   result.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return new Entity.BaseResponse<List<Entity.DeviceTelemetryDataResult>>(false, ex.Message);
            }
            return result;
        }
        public Entity.BaseResponse<Entity.DeviceConnectionStatusResult> GetConnectionStatus(string uniqueId)
        {
            Entity.BaseResponse<Entity.DeviceConnectionStatusResult> result = new Entity.BaseResponse<Entity.DeviceConnectionStatusResult>(true);
            try
            {
                IOT.DataResponse<List<IOT.DeviceConnectionStatus>> deviceConnectionStatus = _iotConnectClient.Device.GetConnectionStatus(uniqueId).Result;
                if (deviceConnectionStatus != null && deviceConnectionStatus.status && deviceConnectionStatus.data != null)
                {
                    result.Data = Mapper.Configuration.Mapper.Map<Entity.DeviceConnectionStatusResult>(deviceConnectionStatus.data.FirstOrDefault());
                }
                else
                {
                    result.Data = null;
                    result.IsSuccess = false;
                    result.Message = new UtilityHelper().IOTResultMessage(deviceConnectionStatus.errorMessages);
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return new Entity.BaseResponse<Entity.DeviceConnectionStatusResult>(false, ex.Message);
            }
            return result;
        }
    }
}
