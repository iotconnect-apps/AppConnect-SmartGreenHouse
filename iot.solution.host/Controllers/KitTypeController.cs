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
    [Route(KitTypeRoute.Route.Global)]
    public class KitTypeController : BaseController
    {
        private readonly IKitTypeService _service;

        public KitTypeController(IKitTypeService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route(KitTypeRoute.Route.Get, Name = KitTypeRoute.Name.Get)]
        public Entity.BaseResponse<List<Entity.KitType>> GetAll()
        {
            Entity.BaseResponse<List<Entity.KitType>> response = new Entity.BaseResponse<List<Entity.KitType>>(true);
            try
            {
                response.Data = _service.GetAllKitTypes();
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<List<Entity.KitType>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(KitTypeRoute.Route.GetTypeDetail, Name = KitTypeRoute.Name.GetTypeDetail)]
        public Entity.BaseResponse<Entity.KitType> GetTypeDetail(Guid templateId)
        {
            Entity.BaseResponse<Entity.KitType> response = new Entity.BaseResponse<Entity.KitType>(true);
            try
            {
                response.Data = _service.GetAllKitTypeDetail(templateId);
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<Entity.KitType>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(KitTypeRoute.Route.GetAttributes, Name = KitTypeRoute.Name.GetAttributes)]
        public Entity.BaseResponse<List<Entity.KitTypeAttribute>> GetAttributes(Guid templateId)
        {
            Entity.BaseResponse<List<Entity.KitTypeAttribute>> response = new Entity.BaseResponse<List<Entity.KitTypeAttribute>>(true);
            try
            {
                response.Data = _service.GetKitTypeAttributes(templateId);
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<List<Entity.KitTypeAttribute>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(KitTypeRoute.Route.GetCommands, Name = KitTypeRoute.Name.GetCommands)]
        public Entity.BaseResponse<List<Entity.KitTypeCommand>> GetCommands(Guid templateId)
        {
            Entity.BaseResponse<List<Entity.KitTypeCommand>> response = new Entity.BaseResponse<List<Entity.KitTypeCommand>>(true);
            try
            {
                response.Data = _service.GetKitTypeCommands(templateId);
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<List<Entity.KitTypeCommand>>(false, ex.Message);
            }
            return response;
        }
    }
}