using component.helper;
using iot.solution.common;
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


namespace iot.solution.service.Data
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IotConnectClient _iotConnectClient;
        private readonly LogHandler.Logger _logger;
        private readonly IUserRepository _userRepository;

        public RoleService(IRoleRepository userRoleRepository, LogHandler.Logger logger, IUserRepository userRepository)
        {
            _logger = logger;
            _roleRepository = userRoleRepository;
            _userRepository = userRepository;
            _iotConnectClient = new IotConnectClient(SolutionConfiguration.BearerToken, SolutionConfiguration.Configuration.EnvironmentCode, SolutionConfiguration.Configuration.SolutionKey);
        }

        public List<Entity.Role> Get()
        {
            try
            {
                return _roleRepository.GetAll().Where(e => !e.IsDeleted).Select(p => Mapper.Configuration.Mapper.Map<Entity.Role>(p)).ToList();
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
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
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
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
                    solutionkeys.Add(SolutionConfiguration.SolutionId.ToString());
                    var addModel = new IOT.AddRoleModel() { name = request.Name, description = request.Description, solutions = solutionkeys };
                    var addRoleResult = AsyncHelpers.RunSync<IOT.DataResponse<IOT.AddRoleResult>>(() => _iotConnectClient.Role.AddRole(addModel));

                    if (addRoleResult != null && addRoleResult.status && addRoleResult.data != null)
                    {
                        request.Guid = Guid.Parse(addRoleResult.data.newid.ToUpper());
                        var dbRole = Mapper.Configuration.Mapper.Map<Entity.Role, Model.Role>(request);
                        dbRole.Guid = request.Guid;
                        dbRole.CompanyGuid = SolutionConfiguration.CompanyId;
                        dbRole.CreatedDate = DateTime.Now;
                        dbRole.CreatedBy = SolutionConfiguration.CurrentUserId;
                        actionStatus = _roleRepository.Manage(dbRole);
                        actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.Role, Entity.Role>(dbRole);
                        if (!actionStatus.Success)
                        {
                            _logger.ErrorLog(new Exception($"Role is not added in solution database, Error: {actionStatus.Message}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                            //var deleteEntityResult = _iotConnectClient.Role.Delete(request.Guid.ToString()).Result;
                            //if (deleteEntityResult != null && deleteEntityResult.status != (int)HttpStatusCode.OK)
                            //{
                            //_logger.Error($"Role is not deleted from iotconnect, Error: {deleteEntityResult.message}");
                            _logger.ErrorLog(new Exception($"Role is not deleted from iotconnect"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                            actionStatus.Success = false;
                            actionStatus.Message = "Something Went Wrong!";
                            //}
                        }
                    }
                    else
                    {
                        _logger.ErrorLog(new Exception($"Role is not added in iotconnect, Error: {addRoleResult.message}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                        actionStatus.Success = false;
                        actionStatus.Message = new UtilityHelper().IOTResultMessage(addRoleResult.errorMessages);
                        
                    }
                }
                else
                {
                    var olddbRole = _roleRepository.FindBy(x => x.Guid.Equals(request.Guid)).FirstOrDefault();
                    
                    if (olddbRole == null)
                    {
                        throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : Role");
                    }

                    List<string> solutionkeys = new List<string>();
                    solutionkeys.Add(SolutionConfiguration.SolutionId.ToString());
                    var updateModel = new IOT.UpdateRoleModel() { name = request.Name, description = request.Description, solutions = solutionkeys };
                    var updateRoleResult = AsyncHelpers.RunSync<IOT.DataResponse<IOT.UpdateRoleResult>>(() => _iotConnectClient.Role.UpdateRole(olddbRole.Guid.ToString(), updateModel));
                                       
                    //var updateEntityResult = _iotConnectClient.User.Update(request.Guid.ToString(), Mapper.Configuration.Mapper.Map<IOT.UpdateEntityModel>(request)).Result;
                    //if (updateEntityResult != null && updateEntityResult.status == (int)HttpStatusCode.OK && updateEntityResult.data != null)
                    //{
                    if(updateRoleResult.status)
                    {
                        var dbRole = Mapper.Configuration.Mapper.Map(request, olddbRole);
                        dbRole.CreatedBy = olddbRole.CreatedBy;
                        dbRole.CreatedDate = olddbRole.CreatedDate;
                        dbRole.UpdatedDate = DateTime.Now;
                        dbRole.CompanyGuid = SolutionConfiguration.CompanyId;
                        dbRole.UpdatedBy = SolutionConfiguration.CurrentUserId;
                        actionStatus = _roleRepository.Manage(dbRole);
                        actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.Role, Entity.Role>(dbRole);
                        if (!actionStatus.Success)
                        {
                            _logger.ErrorLog(new Exception($"Role is not updated in solution database, Error: {actionStatus.Message}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                            actionStatus.Success = false;
                            actionStatus.Message = "Something Went Wrong!";
                        }
                    }
                    else
                    {
                        _logger.ErrorLog(new Exception($"Role is not Updated in iotconnect, Error: {updateRoleResult.message}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                        actionStatus.Success = false;
                        actionStatus.Message = new UtilityHelper().IOTResultMessage(updateRoleResult.errorMessages);
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
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
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
                    var deleteRoleResult = _iotConnectClient.Role.DeleteRole(id.ToString()).Result;
                    if (deleteRoleResult != null && deleteRoleResult.status)
                    {
                        dbRole.IsDeleted = true;
                        dbRole.UpdatedDate = DateTime.Now;
                        dbRole.UpdatedBy = SolutionConfiguration.CurrentUserId;
                        actionStatus = _roleRepository.Update(dbRole);
                    }
                    else
                    {
                        _logger.ErrorLog(new Exception($"Role is not deleted in iotconnect, Error: {deleteRoleResult.message}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                        actionStatus.Success = false;
                        actionStatus.Message = new UtilityHelper().IOTResultMessage(deleteRoleResult.errorMessages);
                    }
                }
                else
                {
                    _logger.ErrorLog(new Exception($"Role is not deleted in solution database.User exists, Error: {actionStatus.Message}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
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
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
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
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
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
                    var updateRoleStatusResult = _iotConnectClient.Role.UpdateRoleStatus(id.ToString(), status).Result;
                    if (updateRoleStatusResult != null && updateRoleStatusResult.status)
                    {
                        dbRole.IsActive = status;
                        dbRole.UpdatedDate = DateTime.Now;
                        dbRole.UpdatedBy = SolutionConfiguration.CurrentUserId;
                        return _roleRepository.Update(dbRole);
                    }
                    else
                    {
                        _logger.ErrorLog(new Exception($"Role is not updated in iotconnect, Error: {updateRoleStatusResult.message}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                        actionStatus.Success = false;
                        actionStatus.Message = new UtilityHelper().IOTResultMessage(updateRoleStatusResult.errorMessages);
                    }
                }
                else
                {
                    _logger.ErrorLog(new Exception($"Role is not updated in solution database.User exists, Error: {actionStatus.Message}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                    actionStatus.Success = false;
                    actionStatus.Message = "Role is not updated in solution database.User exists";
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
    }
}