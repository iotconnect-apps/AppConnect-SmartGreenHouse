using host.iot.solution.Filter;
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
    [Route(LookupRoute.Route.Global)]
    public class LookupController : BaseController
    {
        private readonly ILookupService _service;

        public LookupController(ILookupService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route(LookupRoute.Route.Get, Name = LookupRoute.Name.Get)]
        public Entity.BaseResponse<List<Entity.LookupItem>> Get(string type, string param = "")
        {
            Entity.BaseResponse<List<Entity.LookupItem>> response = new Entity.BaseResponse<List<Entity.LookupItem>>(true);
            try
            {
                response.Data = _service.Get(type, param);
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<List<Entity.LookupItem>>(false, ex.Message);
            }
            return response;
        }
        [HttpGet]
        [Route(LookupRoute.Route.GetAllTemplate, Name = LookupRoute.Name.GetAllTemplate)]
        public Entity.BaseResponse<List<Entity.LookupItem>> GetAllTemplate()
        {
            Entity.BaseResponse<List<Entity.LookupItem>> response = new Entity.BaseResponse<List<Entity.LookupItem>>(true);
            try
            {
                response.Data = _service.GetAllTemplate();
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<List<Entity.LookupItem>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(LookupRoute.Route.GetAllTemplateIoT, Name = LookupRoute.Name.GetAllTemplateIoT)]
        public Entity.BaseResponse<List<Entity.LookupItem>> GetAllTemplateFromIoT()
        {
            Entity.BaseResponse<List<Entity.LookupItem>> response = new Entity.BaseResponse<List<Entity.LookupItem>>(true);
            try
            {
                response.Data = _service.GetAllTemplateFromIoT();
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<List<Entity.LookupItem>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(LookupRoute.Route.GetAttributesIoT, Name = LookupRoute.Name.GetAttributesIoT)]
        public Entity.BaseResponse<List<Entity.KitTypeAttribute>> GetAttributesFromIoT(string templateGuid)
        {
            Entity.BaseResponse<List<Entity.KitTypeAttribute>> response = new Entity.BaseResponse<List<Entity.KitTypeAttribute>>(true);
            try
            {
                response.Data = _service.GetAllAttributesFromIoT(templateGuid);
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<List<Entity.KitTypeAttribute>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(LookupRoute.Route.GetCommandsIoT, Name = LookupRoute.Name.GetCommandsIoT)]
        public Entity.BaseResponse<List<Entity.LookupItem>> GetAllCommandsFromIoT(string templateGuid)
        {
            Entity.BaseResponse<List<Entity.LookupItem>> response = new Entity.BaseResponse<List<Entity.LookupItem>>(true);
            try
            {
                response.Data = _service.GetAllCommandsFromIoT(templateGuid);
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<List<Entity.LookupItem>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(LookupRoute.Route.GetTemplate, Name = LookupRoute.Name.GetTemplate)]
        public Entity.BaseResponse<List<Entity.LookupItem>> GetTemplate()
        {
            Entity.BaseResponse<List<Entity.LookupItem>> response = new Entity.BaseResponse<List<Entity.LookupItem>>(true);
            try
            {
                response.Data = _service.GetTemplate();
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<List<Entity.LookupItem>>(false, ex.Message);
            }
            return response;
        }
        [HttpGet]
        [Route(LookupRoute.Route.GetSensorsLookup, Name = LookupRoute.Name.GetSensorsLookup)]
        [EnsureGuidParameterAttribute("templateId", "Kit Type")]
        [EnsureGuidParameterAttribute("deviceId", "Kit Type")]
        public Entity.BaseResponse<List<Entity.LookupItem>> GetSensorsLookup(string templateId, string deviceId)
        {
            Entity.BaseResponse<List<Entity.LookupItem>> response = new Entity.BaseResponse<List<Entity.LookupItem>>(true);
            try
            {
                response.Data = _service.GetSensors(Guid.Parse(templateId),Guid.Parse(deviceId));
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<List<Entity.LookupItem>>(false, ex.Message);
            }
            return response;
        }
        [HttpGet]
        [Route(LookupRoute.Route.GetTagLookup, Name = LookupRoute.Name.GetTagLookup)]
        [EnsureGuidParameterAttribute("templateId", "Kit Type")]
        public Entity.BaseResponse<List<Entity.LookupItem>> GetTemplateAttribute(string templateId)
        {
            Entity.BaseResponse<List<Entity.LookupItem>> response = new Entity.BaseResponse<List<Entity.LookupItem>>(true);
            try
            {
                response.Data = _service.GetTemplateAttribute(Guid.Parse(templateId));
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<List<Entity.LookupItem>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(LookupRoute.Route.GetTemplateCommands, Name = LookupRoute.Name.GetTemplateCommands)]
        [EnsureGuidParameterAttribute("templateId", "Kit Type")]
        public Entity.BaseResponse<List<Entity.LookupItem>> GetTemplateCommands(string templateId)
        {
            Entity.BaseResponse<List<Entity.LookupItem>> response = new Entity.BaseResponse<List<Entity.LookupItem>>(true);
            try
            {
                response.Data = _service.GetTemplateCommands(Guid.Parse(templateId));
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<List<Entity.LookupItem>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(LookupRoute.Route.GetNestedGreenhouseLookup, Name = LookupRoute.Name.GetNestedGreenhouseLookup)]
        public Entity.BaseResponse<List<Entity.GreenhouseLookup>> GetNestedGreenhouseDetails()
        {
            Entity.BaseResponse<List<Entity.GreenhouseLookup>> response = new Entity.BaseResponse<List<Entity.GreenhouseLookup>>(true);
            try
            {
                response.Data = _service.GetNestedGreenhouseDetails();
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<List<Entity.GreenhouseLookup>>(false, ex.Message);
            }
            return response;
        }
    }
}