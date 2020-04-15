using iot.solution.entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity = iot.solution.entity;

namespace iot.solution.service.Interface
{
    public interface ILookupService
    {
        List<Entity.LookupItem> Get(string type, string param);
        List<Entity.LookupItem> GetTemplate();
        List<Entity.LookupItem> GetAllTemplate();
        List<Entity.LookupItem> GetSensors(Guid templateId, Guid deviceId);
        List<Entity.LookupItem> GetTemplateAttribute(Guid templateId);
        List<Entity.LookupItem> GetTemplateCommands(Guid templateId);
        string GetIotTemplateGuidByName(string templateName);
        List<Entity.LookupItem> GetAllTemplateFromIoT();
        List<Entity.KitTypeAttribute> GetAllAttributesFromIoT(string templateGuid);
        List<Entity.LookupItem> GetAllCommandsFromIoT(string templateGuid);
        List<Entity.GreenhouseLookup> GetNestedGreenhouseDetails();




    }
}