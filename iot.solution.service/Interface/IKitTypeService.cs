﻿using System;
using System.Collections.Generic;
using Entity = iot.solution.entity;

namespace iot.solution.service.Interface
{
    public interface IKitTypeService
    {
        List<Entity.KitType> GetAllKitTypes();
        Entity.KitType GetAllKitTypeDetail(Guid templateId);
        List<Entity.KitTypeAttribute> GetKitTypeAttributes(Guid templateId);
        List<Entity.KitTypeCommand> GetKitTypeCommands(Guid templateId);
        string GetAttributeNameFromSolutionDB(Guid attributeId);
        Guid GetAttributeGuidFromName(string attibuteName);
        Guid GetTemplateDetailsFromIoT(string templateName);
        string GetTemplateNameFromSolutionDB(Guid templateGuid);

    }
}
