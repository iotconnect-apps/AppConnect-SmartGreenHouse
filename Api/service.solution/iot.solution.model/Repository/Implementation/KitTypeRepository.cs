using component.logger;
using Model = iot.solution.model.Models;
using iot.solution.model.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iot.solution.model.Repository.Implementation
{
    public class KitTypeRepository : GenericRepository<Models.KitType>, IKitTypeRepository
    {
        private readonly ILogger logger;
        public KitTypeRepository(IUnitOfWork unitOfWork, ILogger logManager) : base(unitOfWork, logManager)
        {
            logger = logManager;
            _uow = unitOfWork;
        }

        public Model.KitType GetAllKitTypeDetail(Guid templateId)
        {
            return _uow.DbContext.KitType.Where(t => t.Guid == templateId).FirstOrDefault();
        }

        public List<Model.KitType> GetAllKitTypes()
        {
            return _uow.DbContext.KitType.ToList();
        }

        public List<Model.KitTypeAttribute> GetKitTypeAttributes(Guid templateId)
        {
            return _uow.DbContext.KitTypeAttribute.Where(t => t.TemplateGuid == templateId).ToList();
        }

        public List<Model.KitTypeCommand> GetKitTypeCommands(Guid templateId)
        {
            return _uow.DbContext.KitTypeCommand.Where(t => t.TemplateGuid == templateId).ToList();
        }
    }
}