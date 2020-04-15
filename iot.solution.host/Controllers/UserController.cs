using component.helper;
using iot.solution.entity.Structs.Routes;
using iot.solution.service.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Entity = iot.solution.entity;

namespace host.iot.solution.Controllers
{
    [Route(UserRoute.Route.Global)]
    public class UserController : BaseController
    {
        private readonly IUserService _service;

        public UserController(IUserService userService)
        {
            _service = userService;
        }

        [HttpGet]
        [Route(UserRoute.Route.GetList, Name = UserRoute.Name.GetList)]
        public Entity.BaseResponse<List<Entity.User>> Get()
        {
            Entity.BaseResponse<List<Entity.User>> response = new Entity.BaseResponse<List<Entity.User>>(true);
            try
            {
                response.Data = _service.Get();
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<List<Entity.User>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(UserRoute.Route.GetById, Name = UserRoute.Name.GetById)]
        public Entity.BaseResponse<Entity.User> Get(Guid id)
        {
            Entity.BaseResponse<Entity.User> response = new Entity.BaseResponse<Entity.User>(true);
            try
            {
                response.Data = _service.Get(id);
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.User>(false, ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route(UserRoute.Route.Manage, Name = UserRoute.Name.Manage)]
        public Entity.BaseResponse<Entity.UserResponse> Manage([FromBody]Entity.AddUserRequest request)
        {
            Entity.BaseResponse<Entity.UserResponse> response = new Entity.BaseResponse<Entity.UserResponse>(true);
            try
            {
                var status = _service.Manage(request);
                response.IsSuccess = status.Success;
                response.Message = status.Message;
                response.Data = status.Data;
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.UserResponse>(false, ex.Message);
            }
            return response;
        }

        [HttpPut]
        [Route(UserRoute.Route.Delete, Name = UserRoute.Name.Delete)]
        public Entity.BaseResponse<bool> Delete(Guid id)
        {
            Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(true);
            try
            {
                if (id.Equals(SolutionConfiguration.CurrentUserId)) {
                    return new Entity.BaseResponse<bool>(false, "You can't delete your own account!");
                }
                var status = _service.Delete(id);
                response.IsSuccess = status.Success;
                response.Message = status.Message;
                response.Data = status.Success;
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<bool>(false, ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route(UserRoute.Route.UpdateStatus, Name = UserRoute.Name.Status)]
        public Entity.BaseResponse<bool> UpdateStatus(Guid id, bool status)
        {
            Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(true);
            try
            {
                 if (id.Equals(SolutionConfiguration.CurrentUserId))
                    {
                        return new Entity.BaseResponse<bool>(false, "You can't change your own status!");
                    }
                Entity.ActionStatus result = _service.UpdateStatus(id, status);
                response.IsSuccess = result.Success;
                response.Message = result.Message;
                response.Data = result.Success;
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<bool>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(UserRoute.Route.BySearch, Name = UserRoute.Name.BySearch)]
        public Entity.BaseResponse<Entity.SearchResult<List<Entity.UserResponse>>> GetBySearch(string searchText = "", int? pageNo = 1, int? pageSize = 10, string orderBy = "")
        {
            //return Ok(_service.List(
            //    new Entity.SearchRequest()
            //    {
            //        SearchText = searchText,
            //        PageNumber = pageNo.Value,
            //        PageSize = pageSize.Value,
            //        OrderBy = orderBy
            //    }));
            Entity.BaseResponse<Entity.SearchResult<List<Entity.UserResponse>>> response = new Entity.BaseResponse<Entity.SearchResult<List<Entity.UserResponse>>>(true);
            try
            {
                response.Data = _service.List(new Entity.SearchRequest()
                {
                    SearchText = searchText,
                    PageNumber = pageNo.Value,
                    PageSize = pageSize.Value,
                    OrderBy = orderBy
                });
                if (response.Data.Count < 0)
                {
                    response.Data.Count = 0;
                    response.IsSuccess = false;
                    response.Message = "User not authorized!";
                }
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<Entity.SearchResult<List<Entity.UserResponse>>>(false, ex.Message);
            }
            return response;
        }
    }
}