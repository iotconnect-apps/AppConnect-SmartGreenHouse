using component.logger;
using iot.solution.model.Repository.Interface;
using iot.solution.service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using Entity = iot.solution.entity;

namespace iot.solution.service.Implementation
{
    public class KitTypeService : IKitTypeService
    {
        private readonly IKitTypeRepository _kitTypeRepository;
        private readonly ILogger _logger;

        public KitTypeService(IKitTypeRepository kitTypeRepository, ILogger logManager)
        {
            _kitTypeRepository = kitTypeRepository;
            _logger = logManager;
        }

        public List<Entity.KitType> GetAllKitTypes()
        {
            return _kitTypeRepository.GetAllKitTypes().Select(k => Mapper.Configuration.Mapper.Map<Entity.KitType>(k)).ToList();
        }

        public Entity.KitType GetAllKitTypeDetail(Guid templateId)
        {
            return Mapper.Configuration.Mapper.Map<Entity.KitType>(_kitTypeRepository.GetAllKitTypeDetail(templateId));
        }

        public List<Entity.KitTypeAttribute> GetKitTypeAttributes(Guid templateId)
        {
            return   (from r in _kitTypeRepository.GetKitTypeAttributes(templateId) select new Entity.KitTypeAttribute() { LocalName=r.LocalName,Guid=r.Guid.ToString().ToUpper(),ParentTemplateAttributeGuid=r.ParentTemplateAttributeGuid.ToString().ToUpper(),TemplateGuid=r.TemplateGuid.ToString().ToUpper(),Tag=r.Tag }).ToList();                        
        }

        public List<Entity.KitTypeCommand> GetKitTypeCommands(Guid templateId)
        {
            return _kitTypeRepository.GetKitTypeCommands(templateId).Select(k => Mapper.Configuration.Mapper.Map<Entity.KitTypeCommand>(k)).ToList();
        }
    }
}




