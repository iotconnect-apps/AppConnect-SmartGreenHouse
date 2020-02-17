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
using IoTConnect.Model;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using iot.solution.model.Models;
using iot.solution.entity;
using Response = iot.solution.entity.Response;


namespace iot.solution.service.Implementation
{
    public class AdminUserService : IAdminUserService
    {
        private readonly IAdminUserRepository _adminUserRepository;
        private readonly ILogger _logger;

        public AdminUserService(IAdminUserRepository adminUserRepository, ILogger logManager)
        {
            _logger = logManager;
            _adminUserRepository = adminUserRepository;
        }

        public Entity.ActionStatus AdminLogin(Entity.LoginRequest request)
        {
            Entity.ActionStatus result = new ActionStatus(true);
            try
            {
                Model.AdminUser adminUser = _adminUserRepository.AdminLogin(request);
                if (adminUser == null)
                {
                    result.Success = false;
                    result.Message = "No User Found!";
                }
                else
                {
                    result.Success = true;
                    result.Data = adminUser;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                _logger.Error(Constants.ACTION_EXCEPTION, $"UserManager.Login {ex.Message}");
            }
            return result;
        }


        public Entity.AdminUserResponse Get(Guid id)
        {
            try
            {
                return _adminUserRepository.FindBy(x => x.Guid.Equals(id)).Select(p => Mapper.Configuration.Mapper.Map<Entity.AdminUserResponse>(p)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, $"AdminUserService.Get, Error: { ex.Message} ");
                return null;
            }
        }


        public SearchResult<List<UserResponse>> GetAdminUserList(SearchRequest searchRequest)
        {
            SearchResult<List<UserResponse>> userList;
            try
            {
                userList = _adminUserRepository.List(searchRequest);
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, $"UserManager.Login {ex.Message}");
                return null;
            }

            return userList;
        }

        public Entity.ActionStatus Manage(Entity.AddAdminUserRequest request)
        {
            Entity.ActionStatus actionStatus = new ActionStatus(true);
            try
            {
                if (request.Id == null || request.Id == Guid.Empty)
                {
                    var checkExisting = _adminUserRepository.FindBy(x => x.Email.Equals(request.Email) && x.IsActive && !x.IsDeleted).FirstOrDefault();

                    if (checkExisting == null)
                    {

                        var adminUser = Mapper.Configuration.Mapper.Map<Entity.AddAdminUserRequest, Model.AdminUser>(request);



                        adminUser.CreatedBy = AppConfig.CurrentUserId;
                        adminUser.CreatedDate = DateTime.Now;
                        adminUser.IsActive = true;
                        adminUser.IsDeleted = false;
                        adminUser.Guid = Guid.NewGuid();
                        adminUser.CompanyGuid = AppConfig.CompanyId;

                        actionStatus = _adminUserRepository.Insert(adminUser);

                        actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.AdminUser, Entity.UserResponse>(actionStatus.Data);

                        if (!actionStatus.Success)
                        {
                            _logger.Error($"Admin User is not added in solution database, Error: {actionStatus.Message}");
                            actionStatus.Success = false;
                            actionStatus.Message = "Something Went Wrong!";
                        }
                    }
                    else
                    {
                        // _logger.Error($"AdminUser is not updated in solution database, Error: {actionStatus.Message}");
                        actionStatus.Success = false;
                        actionStatus.Message = "Email Id Already Registered.";
                    }


                }
                else
                {
                    var oldAdminUser = _adminUserRepository.FindBy(x => x.Guid.Equals(request.Id)).FirstOrDefault();
                    if (oldAdminUser == null)
                    {
                        throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : AdminUser");
                    }
                    else
                    {
                        var dbAdminUser = Mapper.Configuration.Mapper.Map(request, oldAdminUser);

                        dbAdminUser.CreatedBy = oldAdminUser.CreatedBy;
                        dbAdminUser.CreatedDate = oldAdminUser.CreatedDate;
                        dbAdminUser.UpdatedDate = DateTime.Now;
                        dbAdminUser.UpdatedBy = AppConfig.CurrentUserId;
                        dbAdminUser.CompanyGuid = AppConfig.CompanyId;

                        actionStatus = _adminUserRepository.Update(dbAdminUser);

                        actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.AdminUser, Entity.UserResponse>(actionStatus.Data);

                        if (!actionStatus.Success)
                        {
                            _logger.Error($"AdminUser is not updated in solution database, Error: {actionStatus.Message}");
                            actionStatus.Success = false;
                            actionStatus.Message = "Something Went Wrong!";
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "AdminUserManager.Manage " + ex);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }

            return actionStatus;

        }


        public Entity.ActionStatus Delete(Guid id)
        {
            Entity.ActionStatus actionStatus = new ActionStatus(true);

            try
            {

                var dbAdminUser = _adminUserRepository.FindBy(x => x.Guid.Equals(id)).FirstOrDefault();

                if (dbAdminUser == null)
                {
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound } : AdminUser");
                }
                else
                {
                    dbAdminUser.IsDeleted = true;
                    dbAdminUser.UpdatedBy = AppConfig.CurrentUserId;
                    dbAdminUser.UpdatedDate = DateTime.Now;
                    actionStatus = _adminUserRepository.Update(dbAdminUser);
                    actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.AdminUser, Entity.UserResponse>(actionStatus.Data);

                    if (!actionStatus.Success)
                    {
                        _logger.Error($"AdminUser is not deleted in solution database, Error: {actionStatus.Message}");
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "AdminUserManager.Delete " + ex);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }

            return actionStatus;
        }
        /// <summary>
        /// Put fasle for InActive and Tru
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public ActionStatus UpdateStatus(Guid id, bool status)
        {
            Entity.ActionStatus actionStatus = new ActionStatus(true);
            try
            {
                var existingUser = _adminUserRepository.FindBy(x => x.Guid.Equals(id) && !x.IsDeleted).FirstOrDefault();

                if (existingUser == null)
                {
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : AdminUser");
                }
                else
                {
                    existingUser.IsActive = status;
                    existingUser.UpdatedBy = AppConfig.CurrentUserId;
                    existingUser.UpdatedDate = DateTime.Now;
                    actionStatus = _adminUserRepository.Update(existingUser);
                    actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.AdminUser, Entity.UserResponse>(actionStatus.Data);
                    if (!actionStatus.Success)
                    {
                        _logger.Error($"AdminUser is not updated in solution database, Error: {actionStatus.Message}");
                        actionStatus.Success = false;
                        actionStatus.Message = "Something Went Wrong!";
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "AdminUserManager.UpdateStatus " + ex);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }

            return actionStatus;
        }
    }
}
