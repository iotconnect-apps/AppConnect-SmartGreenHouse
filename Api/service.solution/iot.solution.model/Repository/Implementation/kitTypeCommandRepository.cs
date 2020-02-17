using component.logger;
using iot.solution.model.Models;
using iot.solution.model.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.model.Repository.Implementation
{
    public class kitTypeCommandRepository : GenericRepository<KitTypeCommand>, IkitTypeCommandRepository
    {
        private readonly ILogger logger;
        public kitTypeCommandRepository(IUnitOfWork unitOfWork, ILogger logManager) : base(unitOfWork, logManager)
        {
            logger = logManager;
            _uow = unitOfWork;
        }
    }
}