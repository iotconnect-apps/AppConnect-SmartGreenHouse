using component.logger;
using iot.solution.model.Repository.Interface;
using iot.solution.service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using Entity = iot.solution.entity;
using LogHandler = component.services.loghandler;
using System.Reflection;
using component.helper;

namespace iot.solution.service.Implementation
{
    public class KitTypeService : IKitTypeService
    {
        private readonly IKitTypeRepository _kitTypeRepository;
        private readonly IKitTypeAttributeRepository _kitTypeAttributeRepository;

        private readonly LogHandler.Logger _logger;

        public KitTypeService(IKitTypeRepository kitTypeRepository, IKitTypeAttributeRepository kitTypeAttributeRepository, LogHandler.Logger logger)
        {
            _kitTypeRepository = kitTypeRepository;
            _logger = logger;
            _kitTypeAttributeRepository = kitTypeAttributeRepository;
        }

        public List<Entity.KitType> GetAllKitTypes()
        {
            List<Entity.KitType> result = new List<Entity.KitType>();
            try
            {
                result = _kitTypeRepository.GetAllKitTypes().Select(k => Mapper.Configuration.Mapper.Map<Entity.KitType>(k)).ToList();
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }

        public Entity.KitType GetAllKitTypeDetail(Guid templateId)
        {
            Entity.KitType result = new Entity.KitType();
            try
            {
                result = Mapper.Configuration.Mapper.Map<Entity.KitType>(_kitTypeRepository.GetAllKitTypeDetail(templateId));
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }

        public List<Entity.KitTypeAttribute> GetKitTypeAttributes(Guid templateId)
        {
            List<Entity.KitTypeAttribute> result = new List<Entity.KitTypeAttribute>();
            try
            {
                result = (from r in _kitTypeRepository.GetKitTypeAttributes(templateId) select new Entity.KitTypeAttribute() { LocalName = r.LocalName, Guid = r.Guid.ToString().ToUpper(), ParentTemplateAttributeGuid = r.ParentTemplateAttributeGuid.ToString().ToUpper(), TemplateGuid = r.KittypeGuid.ToString().ToUpper(), Tag = r.Tag }).ToList();
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }

        public List<Entity.KitTypeCommand> GetKitTypeCommands(Guid templateId)
        {
            List<Entity.KitTypeCommand> result = new List<Entity.KitTypeCommand>();
            try
            {
                result = _kitTypeRepository.GetKitTypeCommands(templateId).Select(k => Mapper.Configuration.Mapper.Map<Entity.KitTypeCommand>(k)).ToList();
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }

        public string GetAttributeNameFromSolutionDB(Guid attributeId)
        {
            string attributeName = string.Empty;
            try
            {
                attributeName = _kitTypeAttributeRepository.FindBy(x => x.Guid.Equals(attributeId)).FirstOrDefault().LocalName;
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }

            return attributeName;
        }

        public Guid GetTemplateDetailsFromIoT(string templateName)
        {
            Guid result = Guid.Empty;
            try
            {
                result = TemplateHelper.GetTemplateDetailsFromIoT(templateName);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex,this.GetType().Name,MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }

        public Guid GetAttributeGuidFromName(string attibuteName)
        {
            Guid result = Guid.Empty;
            try
            {
                result = TemplateHelper.GetAttributeGuidFromName(attibuteName);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }

            return result;
        }


        public string GetTemplateNameFromSolutionDB(Guid templateGuid)
        {
            string result = string.Empty;
            try
            {
                result = _kitTypeRepository.FindBy(x => x.Guid.Equals(templateGuid)).FirstOrDefault().Name;
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }

            return result;
        }
    }
}




