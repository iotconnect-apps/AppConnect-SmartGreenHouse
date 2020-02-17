using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using component.common.model.CommonModel;
using component.exception;
using component.logger;
using component.helper;
using component.common.model;
using iot.solution.model.Repository.Interface;
using iot.solution.service.Interface;
using Entity = iot.solution.entity;
using Model = iot.solution.model.Models;
using IOT = IoTConnect.Model;
using System.IdentityModel.Tokens.Jwt;
using iot.solution.entity;
using Response = iot.solution.entity.Response;

namespace iot.solution.service.Implementation
{
    public class DeviceService : IDeviceService
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly IotConnectClient _iotConnectClient;
        private readonly ILogger _logger;

        public DeviceService(IDeviceRepository deviceRepository, ILogger logger)
        {
            _logger = logger;
            _deviceRepository = deviceRepository;
            _iotConnectClient = new IotConnectClient(AppConfig.BearerToken, AppConfig.EnvironmentCode, AppConfig.SolutionKey);
        }

        public List<Entity.Device> Get()
        {
            try
            {
                return _deviceRepository.GetAll().Where(e => !e.IsDeleted).Select(p => Mapper.Configuration.Mapper.Map<Entity.Device>(p)).ToList();
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "DeviceManager.Get " + ex);
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
                _logger.Error(Constants.ACTION_EXCEPTION, "DeviceManager.Get " + ex);
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
                        dbDevice.Guid = request.Guid;
                        dbDevice.CompanyGuid = AppConfig.CompanyId;
                        dbDevice.CreatedDate = DateTime.Now;
                        dbDevice.CreatedBy = AppConfig.CurrentUserId;
                        actionStatus = _deviceRepository.Manage(dbDevice);
                        actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.Device, Entity.Device>(actionStatus.Data);
                        if (!actionStatus.Success)
                        {
                            _logger.Error($"Device is not added in solution database, Error: {actionStatus.Message}");
                            var deleteEntityResult = _iotConnectClient.Device.Delete(request.Guid.ToString()).Result;
                            if (deleteEntityResult != null && deleteEntityResult.status)
                            {
                                _logger.Error($"Device is not deleted from iotconnect");
                                actionStatus.Success = false;
                                actionStatus.Message = "Something Went Wrong!";
                            }
                        }
                    }
                    else
                    {
                        _logger.Error($"Device is not added in iotconnect, Error: {addDeviceResult.message}");
                        actionStatus.Success = false;
                        actionStatus.Message = "Something Went Wrong!";
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
                        dbDevice.UpdatedBy = AppConfig.CurrentUserId;
                        dbDevice.CompanyGuid = AppConfig.CompanyId;
                        actionStatus = _deviceRepository.Manage(dbDevice);
                        actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.Device, Entity.Device>(actionStatus.Data);
                        if (!actionStatus.Success)
                        {
                            _logger.Error($"Device is not updated in solution database, Error: {actionStatus.Message}");
                            actionStatus.Success = false;
                            actionStatus.Message = "Something Went Wrong!";
                        }
                    }
                    else
                    {
                        _logger.Error($"Device is not added in iotconnect, Error: {updateEntityResult.message}");
                        actionStatus.Success = false;
                        actionStatus.Message = "Something Went Wrong!";
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "Device.Manage " + ex);
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
                    dbDevice.UpdatedBy = AppConfig.CurrentUserId;
                    return _deviceRepository.Update(dbDevice);
                }
                else
                {
                    _logger.Error($"Device is not deleted from iotconnect, Error: {deleteEntityResult.message}");
                    actionStatus.Success = false;
                    actionStatus.Message = "Something Went Wrong!";
                }

            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "DeviceManager.Delete " + ex);
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
                    dbDevice.UpdatedBy = AppConfig.CurrentUserId;
                    return _deviceRepository.Update(dbDevice);
                }
                else
                {
                    _logger.Error($"Device status is not updated in iotconnect, Error: {updatedbStatusResult.message}");
                    actionStatus.Success = false;
                    actionStatus.Message = "Something Went Wrong!";
                }
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "DeviceService.Delete " + ex);
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
                _logger.Error(Constants.ACTION_EXCEPTION, $"DeviceService.List, Error: {ex.Message}");
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
                _logger.Error(Constants.ACTION_EXCEPTION, $"DeviceService.GetGreenHouseDeviceList, Error: {ex.Message}");
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
                _logger.Error(Constants.ACTION_EXCEPTION, $"DeviceService.List, Error: {ex.Message}");
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
                _logger.Error(Constants.ACTION_EXCEPTION, $"DeviceService.List, Error: {ex.Message}");
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
                _logger.Error(Constants.ACTION_EXCEPTION, $"DeviceService.List, Error: {ex.Message}");
                return new Entity.SearchResult<List<Entity.Device>>();
            }
        }
        public Response.DeviceDetailResponse GetDeviceDetail(Guid deviceId)
        {
            return new Response.DeviceDetailResponse()
            {
                EnergyUsage = 2700,
                Temperature = 73,
                Moisture = 15,
                Humidity = 62,
                WaterUsage = 3800,
                TotalDevices = ChildDeviceList(new Entity.SearchRequest()
                {
                    Guid = deviceId.ToString(),
                    PageNumber = -1,
                    PageSize = -1,
                    OrderBy = ""
                }).Items.Count()
        };
           
        }
        public List<Response.GreenHouseDevicesResponse> GetGreenHouseDevices(Guid greenhouseId)
        {
            try
            {
                return _deviceRepository.GetGreenHouseDevices(greenhouseId, null);
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, $"GetGreenHouseDevices.List, Error: {ex.Message}");
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
                _logger.Error(Constants.ACTION_EXCEPTION, $"GetGreenHouseChildDevices.List, Error: {ex.Message}");
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
                _logger.Error(Constants.ACTION_EXCEPTION, $"Device.ValidateKit, Error: {ex.Message}");
                return null;
            }

        }
        public Entity.BaseResponse<bool> ProvisionKit(Entity.ProvisionKitRequest request)
        {
            Entity.BaseResponse<bool> result = new Entity.BaseResponse<bool>(true);
            try
            {
                var repoResult = _deviceRepository.ProvisionKit(request);

                if (repoResult != null && repoResult.Data != null && repoResult.Data.Any())
                {
                    List<Entity.KitDevice> deviceList = repoResult.Data.OrderBy(d => d.ParentDeviceGuid.HasValue).ToList();

                    foreach (var device in deviceList)
                    {
                        IOT.AddDeviceModel iotDeviceDetail = new IOT.AddDeviceModel()
                        {
                            DisplayName = device.Name,
                            entityGuid = request.GreenHouseGuid.ToString(),
                            uniqueId = device.UniqueId,
                            deviceTemplateGuid = device.TemplateGuid.ToString(),
                            parentDeviceGuid = device.ParentDeviceGuid.ToString(),
                            note = device.Note,
                            tag = device.Tag,
                            properties = new List<IOT.AddProperties>()

                        };
                        var addDeviceResult = _iotConnectClient.Device.Add(iotDeviceDetail).Result;
                        if (addDeviceResult != null && addDeviceResult.status && addDeviceResult.data != null)
                        {
                            Guid newDeviceId = Guid.Parse(addDeviceResult.data.newid.ToUpper());
                            var intUpdated = deviceList.Where(d => d.ParentDeviceGuid == device.Guid).Select(d => d.ParentDeviceGuid = newDeviceId).Count();

                            Entity.ActionStatus actionStatus = _deviceRepository.Manage(new Model.Device()
                            {
                                Guid = newDeviceId,
                                CompanyGuid = AppConfig.CompanyId,
                                GreenHouseGuid = request.GreenHouseGuid,
                                TemplateGuid = device.TemplateGuid,
                                ParentDeviceGuid = device.ParentDeviceGuid,
                                Type = null,
                                UniqueId = device.UniqueId,
                                Name = device.Name,
                                Note = device.Note,
                                Tag = device.Tag,
                                IsProvisioned = false,
                                IsActive = true,
                                IsDeleted = false,
                                CreatedDate = DateTime.UtcNow,
                                CreatedBy = AppConfig.CurrentUserId
                            });

                            if (!actionStatus.Success)
                            {
                                _logger.Error($"Device is not added in solution database, Error: {actionStatus.Message}");
                                var deleteEntityResult = _iotConnectClient.Device.Delete(device.Guid.ToString()).Result;
                                if (deleteEntityResult != null && deleteEntityResult.status)
                                {
                                    _logger.Error($"Device is not deleted from iotconnect");
                                    actionStatus.Success = false;
                                    actionStatus.Message = "Something Went Wrong!";
                                }
                            }
                            else
                            {
                              //  result.Message = "Kit Added Successfully!";
                            }
                        }
                        else
                        {
                            _logger.Error($"Kit is not added in iotconnect, Error: {addDeviceResult.message}");
                            result.IsSuccess = false;
                            result.Message = "Something Went Wrong!";
                        }
                    }
                }
                else
                {
                    return new Entity.BaseResponse<bool>(false, repoResult.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, $"Device.GetDeviceStatus, Error: {ex.Message}");
                return null;
            }
            return result;
        }
    }
}
