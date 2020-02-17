using component.common.model;
using component.common.model.CommonModel;
using component.exception;
using component.helper;
using component.logger;
using iot.solution.model.Repository.Interface;
using iot.solution.service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

using Entity = iot.solution.entity;
using Model = iot.solution.model.Models;
using IOT = IoTConnect.Model;

namespace iot.solution.service.Data
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IotConnectClient _iotConnectClient;
        private readonly ILogger _logger;
        private readonly IUserRepository _userRepository;

        public RoleService(IRoleRepository userRoleRepository, ILogger logger, IUserRepository userRepository)
        {
            _logger = logger;
            _roleRepository = userRoleRepository;
            _userRepository = userRepository;
            _iotConnectClient = new IotConnectClient(AppConfig.BearerToken, AppConfig.EnvironmentCode, AppConfig.SolutionKey);
        }

        public List<Entity.Role> Get()
        {
            try
            {
                return _roleRepository.GetAll().Where(e => !e.IsDeleted).Select(p => Mapper.Configuration.Mapper.Map<Entity.Role>(p)).ToList();
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "Role.Get " + ex);
                return new List<Entity.Role>();
            }
        }
        public Entity.Role Get(Guid id)
        {
            try
            {
                return _roleRepository.FindBy(r => r.Guid == id).Select(p => Mapper.Configuration.Mapper.Map<Entity.Role>(p)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "Role.Get " + ex);
                return null;
            }

        }
        public Entity.ActionStatus Manage(Entity.Role request)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                if (request.Guid == null || request.Guid == Guid.Empty)
                {
                    List<string> solutionkeys = new List<string>();
                    solutionkeys.Add(AppConfig.SolutionId.ToString());
                    var addModel = new IOT.AddRoleModel() { name = request.Name, description = request.Description, solutions = solutionkeys };
                    var addRoleResult = AsyncHelpers.RunSync<IOT.DataResponse<IOT.AddRoleResult>>(() => _iotConnectClient.User.AddRole(addModel));

                    if (addRoleResult != null && addRoleResult.status && addRoleResult.data != null)
                    {
                        request.Guid = Guid.Parse(addRoleResult.data.newid.ToUpper());
                        var dbRole = Mapper.Configuration.Mapper.Map<Entity.Role, Model.Role>(request);
                        dbRole.Guid = request.Guid;
                        dbRole.CompanyGuid = AppConfig.CompanyId;
                        dbRole.CreatedDate = DateTime.Now;
                        dbRole.CreatedBy = AppConfig.CurrentUserId;
                        actionStatus = _roleRepository.Manage(dbRole);
                        actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.Role, Entity.Role>(dbRole);
                        if (!actionStatus.Success)
                        {
                            _logger.Error($"Role is not added in solution database, Error: {actionStatus.Message}");
                            //var deleteEntityResult = _iotConnectClient.Role.Delete(request.Guid.ToString()).Result;
                            //if (deleteEntityResult != null && deleteEntityResult.status != (int)HttpStatusCode.OK)
                            //{
                            //_logger.Error($"Role is not deleted from iotconnect, Error: {deleteEntityResult.message}");
                            _logger.Error($"Role is not deleted from iotconnect");
                            actionStatus.Success = false;
                            actionStatus.Message = "Something Went Wrong!";
                            //}
                        }
                    }
                    else
                    {
                        _logger.Error($"Role is not added in iotconnect, Error: {addRoleResult.message}");
                        actionStatus.Success = false;
                        actionStatus.Message = "Something Went Wrong!";
                    }
                }
                else
                {
                    var olddbRole = _roleRepository.FindBy(x => x.Guid.Equals(request.Guid)).FirstOrDefault();
                    if (olddbRole == null)
                    {
                        throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : Role");
                    }

                    //var updateEntityResult = _iotConnectClient.User.Update(request.Guid.ToString(), Mapper.Configuration.Mapper.Map<IOT.UpdateEntityModel>(request)).Result;
                    //if (updateEntityResult != null && updateEntityResult.status == (int)HttpStatusCode.OK && updateEntityResult.data != null)
                    //{
                    var dbRole = Mapper.Configuration.Mapper.Map(request, olddbRole);
                    dbRole.CreatedBy = olddbRole.CreatedBy;
                    dbRole.CreatedDate = olddbRole.CreatedDate;
                    dbRole.UpdatedDate = DateTime.Now;
                    dbRole.CompanyGuid = AppConfig.CompanyId;
                    dbRole.UpdatedBy = AppConfig.CurrentUserId;
                    actionStatus = _roleRepository.Manage(dbRole);
                    actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.Role, Entity.Role>(dbRole);
                    if (!actionStatus.Success)
                    {
                        _logger.Error($"Role is not updated in solution database, Error: {actionStatus.Message}");
                        actionStatus.Success = false;
                        actionStatus.Message = "Something Went Wrong!";
                    }
                    //}
                    //else
                    //{
                    //    _logger.Error($"Role is not added in iotconnect, Error: {updateEntityResult.message}");
                    //    actionStatus.Success = false;
                    //    actionStatus.Message = "Something Went Wrong!";
                    //}

                }
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "RoleManager.Delete " + ex);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }
        public Entity.ActionStatus Delete(Guid id)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                var dbRole = _roleRepository.FindBy(x => x.Guid.Equals(id)).FirstOrDefault();
                if (dbRole == null)
                {
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : Role");
                }
                var userRole = _userRepository.FindBy(x => x.RoleGuid.Equals(id) && x.IsDeleted.Equals(false)).FirstOrDefault();
                if (userRole == null)
                {
                    dbRole.IsDeleted = true;
                    dbRole.UpdatedDate = DateTime.Now;
                    dbRole.UpdatedBy = AppConfig.CurrentUserId;
                    actionStatus = _roleRepository.Update(dbRole);
                }
                else
                {
                    _logger.Error($"Role is not deleted in solution database.User exists, Error: {actionStatus.Message}");
                    actionStatus.Success = false;
                    actionStatus.Message = "Role is not deleted in solution database.User exists";
                }

                //if (!actionStatus.Success)
                //{
                //    _logger.Error($"Role is not deleted in solution database, Error: {actionStatus.Message}");
                //    actionStatus.Success = false;
                //    actionStatus.Message = "Something Went Wrong!";
                //}
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "RoleManager.Delete " + ex);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }
        public Entity.SearchResult<List<Entity.Role>> List(Entity.SearchRequest request)
        {
            try
            {
                var result = _roleRepository.List(request);
                return new Entity.SearchResult<List<Entity.Role>>()
                {
                    Items = result.Items.Select(p => Mapper.Configuration.Mapper.Map<Entity.Role>(p)).ToList(),
                    Count = result.Count
                };
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, $"RoleService.List, Error: {ex.Message}");
                return new Entity.SearchResult<List<Entity.Role>>();
            }
        }
        public Entity.ActionStatus UpdateStatus(Guid id, bool status)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                var dbRole = _roleRepository.FindBy(x => x.Guid.Equals(id)).FirstOrDefault();
                if (dbRole == null)
                {
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : Role");
                }
                var userRole = _userRepository.FindBy(x => x.RoleGuid.Equals(id)).FirstOrDefault();
                if (userRole == null)
                {                   
                    dbRole.IsActive = status;
                    dbRole.UpdatedDate = DateTime.Now;
                    dbRole.UpdatedBy = AppConfig.CurrentUserId;
                    return _roleRepository.Update(dbRole);
                }
                else
                {
                    _logger.Error($"Role is not updated in solution database.User exists, Error: {actionStatus.Message}");
                    actionStatus.Success = false;
                    actionStatus.Message = "Role is not updated in solution database.User exists";
                }

            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "RoleService.Delete " + ex);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }
    }
}