using iot.solution.model.Models;
using component.logger;
using iot.solution.model.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.model.Repository.Implementation
{
    public class DashboardRepository:GenericRepository<GreenHouse>,IDashboardRepository
    {
        private readonly ILogger logger;
        public DashboardRepository(IUnitOfWork unitOfWork, ILogger logManager) : base(unitOfWork, logManager)
        {
            logger = logManager;
            _uow = unitOfWork;
        }
    }
}
