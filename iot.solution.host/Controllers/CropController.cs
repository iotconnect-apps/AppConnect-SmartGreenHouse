﻿using host.iot.solution.Filter;
using iot.solution.entity.Structs.Routes;
using iot.solution.model.Models;
using iot.solution.service.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using Entity = iot.solution.entity;

namespace host.iot.solution.Controllers
{
    /// <summary>
    /// Crop
    /// </summary>
    [Route(CropRoute.Route.Global)]
    public class CropController : BaseController
    {
        private readonly ICropService _service;
        
        public CropController(ICropService cropEngine)
        {
            _service = cropEngine;
        }

        [HttpGet]
        [Route(CropRoute.Route.GetCrops, Name = CropRoute.Name.GetCrops)]
        [EnsureGuidParameterAttribute("greenHouseId", "Greenhouse")]
        public Entity.BaseResponse<List<Entity.Crop>> GetCrops(string greenHouseId)
        {
            Entity.BaseResponse<List<Entity.Crop>> response = new Entity.BaseResponse<List<Entity.Crop>>(true);
            try
            {
                response.Data = _service.GetCrops(Guid.Parse(greenHouseId));
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<List<Entity.Crop>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(CropRoute.Route.GetList, Name = CropRoute.Name.GetList)]
        public Entity.BaseResponse<List<Entity.Crop>> Get()
        {
            Entity.BaseResponse<List<Entity.Crop>> response = new Entity.BaseResponse<List<Entity.Crop>>(true);
            try
            {
                response.Data = _service.Get();
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<List<Entity.Crop>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(CropRoute.Route.GetById, Name = CropRoute.Name.GetById)]
        [EnsureGuidParameterAttribute("id", "Crop")]
        public Entity.BaseResponse<Entity.Crop> Get(string id)
        {
            Entity.BaseResponse<Entity.Crop> response = new Entity.BaseResponse<Entity.Crop>(true);
            try
            {
                response.Data = _service.Get(Guid.Parse(id));
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<Entity.Crop>(false, ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route(CropRoute.Route.Manage, Name = CropRoute.Name.Add)]
        public Entity.BaseResponse<Entity.Crop> Manage([FromForm]Entity.CropModel request)
        {
            Entity.BaseResponse<Entity.Crop> response = new Entity.BaseResponse<Entity.Crop>(true);
            try
            {
               
                var status = _service.Manage(request);
                response.IsSuccess = status.Success;
                response.Message = status.Message;
                response.Data = status.Data;
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<Entity.Crop>(false, ex.Message);
            }
            return response;
        }

        [HttpPut]
        [Route(CropRoute.Route.Delete, Name = CropRoute.Name.Delete)]
        [EnsureGuidParameterAttribute("id", "Crop")]
        public Entity.BaseResponse<bool> Delete(string id)
        {
            Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(true);
            try
            {
                var status = _service.Delete(Guid.Parse(id));
                response.IsSuccess = status.Success;
                response.Message = status.Message;
                response.Data = status.Success;
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<bool>(false, ex.Message);
            }
            return response;
        }
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [HttpPut]
        [Route(CropRoute.Route.DeleteImage, Name = CropRoute.Name.DeleteImage)]
        [EnsureGuidParameterAttribute("id", "CropImage")]
        public Entity.BaseResponse<bool> DeleteImage(string id)
        {
            Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(true);
            try
            {
                var status = _service.DeleteImage(Guid.Parse(id));
                response.IsSuccess = status.Success;
                response.Message = status.Message;
                response.Data = status.Success;
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<bool>(false, ex.Message);
            }
            return response;
        }
        [HttpPost]
        [Route(CropRoute.Route.UpdateStatus, Name = CropRoute.Name.UpdateStatus)]
        [EnsureGuidParameterAttribute("id", "Crop")]
        public Entity.BaseResponse<bool> UpdateStatus(string id, bool status)
        {
            Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(true);
            try
            {
                Entity.ActionStatus result = _service.UpdateStatus(Guid.Parse(id), status);
                response.IsSuccess = result.Success;
                response.Message = result.Message;
                response.Data = result.Success;
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<bool>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(CropRoute.Route.BySearch, Name = CropRoute.Name.BySearch)]
        public Entity.BaseResponse<Entity.SearchResult<List<Entity.Crop>>> GetBySearch(string greenHouseId = "", string searchText = "", int? pageNo = 1, int? pageSize = 10, string orderBy = "")
        {
            Entity.BaseResponse<Entity.SearchResult<List<Entity.Crop>>> response = new Entity.BaseResponse<Entity.SearchResult<List<Entity.Crop>>>(true);
            try
            {
                response.Data = _service.List(new Entity.SearchRequest()
                {
                    SearchText = searchText,
                    PageNumber = pageNo.Value,
                    PageSize = pageSize.Value,
                    OrderBy = orderBy
                });
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<Entity.SearchResult<List<Entity.Crop>>>(false, ex.Message);
            }
            return response;
        }
    }
}