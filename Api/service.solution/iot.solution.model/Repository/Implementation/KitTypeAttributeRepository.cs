using component.logger;
using iot.solution.model.Models;
using iot.solution.model.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.model.Repository.Implementation
{
    public class KitTypeAttributeRepository : GenericRepository<KitTypeAttribute>, IKitTypeAttributeRepository
    {
        private readonly ILogger logger;
        public KitTypeAttributeRepository(IUnitOfWork unitOfWork, ILogger logManager) : base(unitOfWork, logManager)
        {
            logger = logManager;
            _uow = unitOfWork;
        }
    }
}