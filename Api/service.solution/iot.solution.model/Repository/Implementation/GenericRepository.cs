using component.common.model;
using component.logger;
using iot.solution.data;
//using iot.solution.entity;
using iot.solution.model.Models;
using iot.solution.model.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Entity = iot.solution.entity;

namespace iot.solution.model.Repository.Implementation
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class //, IEntityBase
    {
        public GenericRepository(IUnitOfWork unitOfWork, ILogger logManager)
        {
            ConnectionString = AppConfig.Configuration.GetSection("ConnectionStrings:DefaultConnection").Value;

            if (unitOfWork == null)
                throw new ArgumentNullException("UnitOfWork cannot be null.");
            if (logManager == null)
                throw new ArgumentNullException("LogManager cannot be null");

            _uow = unitOfWork;
            _logger = logManager;
        }
        private DbSet<T> Entities
        {
            get
            {
                if (_entities == null) _entities = _uow.DbContext.Set<T>();
                return _entities;
            }
        }

        public string ConnectionString { get; }

        public virtual void SetModified<K>(K entity) where K : class
        {
            _uow.DbContext.Entry(entity).State = EntityState.Modified;
        }
        public virtual IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            _logger.Information(Constants.ACTION_ENTRY, "GenericRepository.FindBy");
            var list = Entities.AsNoTracking().Where(predicate);
            _logger.Information(Constants.ACTION_EXIT, "GenericRepository.FindBy");
            return list;
        }
        public IQueryable<T> GetAll()
        {
            _logger.Information(Constants.ACTION_ENTRY, "GenericRepository.GetAll");
            IQueryable<T> list = Entities;
            _logger.Information(Constants.ACTION_EXIT, "GenericRepository.GetAll");
            return list;
        }
        
        public T GetByUniqueId(Expression<Func<T, bool>> predicate)
        {
            _logger.Information(Constants.ACTION_ENTRY, "GenericRepository.GetByUniqueId");
            var obj = Entities.AsNoTracking().FirstOrDefault(predicate);
            _logger.Information(Constants.ACTION_EXIT, "GenericRepository.GetByUniqueId");
            return obj;
        }
        public Entity.ActionStatus Insert(T entity)
        {
            _logger.Information(Constants.ACTION_ENTRY, "GenericRepository.Insert");
            if (entity == null) throw new ArgumentNullException("entity");
            var selfTran = false;
            if (!_uow.InTransaction)
            {
                _uow.BeginTransaction();
                selfTran = true;
            }

            var _actionStatus = new Entity.ActionStatus();
            try
            {
                // entity.RefId = entity.RefId == Guid.Empty ? Guid.NewGuid() : entity.RefId;
                Entities.Add(entity);
                _actionStatus = ApplyChanges();

                if (!_actionStatus.Success) throw new Exception(_actionStatus.Message);
                // _actionStatus.Result = entity.RecordId;
                _actionStatus.Data = entity;
                _logger.Information(Constants.ACTION_EXIT, "GenericRepository.Insert");
            }
            catch (Exception ex)
            {
                _logger.Error("GenericRepository.Insert", ex);
                _actionStatus.Success = false;
                _actionStatus.Message = ex.Message;
            }
            finally
            {
                if (selfTran)
                {
                    if (_actionStatus.Success)
                    {
                        _logger.Information("GenericRepository.Insert", "Entity Inserted successfully,Committing Transaction");
                        var _tactionStatus = _uow.EndTransaction();
                        if (!_tactionStatus.Success) _actionStatus = _tactionStatus;
                    }
                    else
                    {
                        _logger.Information("GenericRepository.Insert",
                            "Having issues while Inserting entity,Rollbaking transaction");
                        _uow.RollBack();
                    }
                }
            }

            return _actionStatus;
        }

        public Entity.ActionStatus InsertRange(List<T> entity)
        {
            _logger.Information(Constants.ACTION_ENTRY, "GenericRepository.InsertRange");
            if (entity == null) throw new ArgumentNullException("entity");
            var selfTran = false;
            if (!_uow.InTransaction)
            {
                _uow.BeginTransaction();
                selfTran = true;
            }

            var _actionStatus = new Entity.ActionStatus();
            try
            {
                // entity.RefId = entity.RefId == Guid.Empty ? Guid.NewGuid() : entity.RefId;
                Entities.AddRange(entity);
                _actionStatus = ApplyChanges();

                if (!_actionStatus.Success) throw new Exception(_actionStatus.Message);
                // _actionStatus.Result = entity.RecordId;
                _actionStatus.Data = entity;
                _logger.Information(Constants.ACTION_EXIT, "GenericRepository.InsertRange");
            }
            catch (Exception ex)
            {
                _logger.Error("GenericRepository.InsertRange", ex);
                _actionStatus.Success = false;
                _actionStatus.Message = ex.Message;
            }
            finally
            {
                if (selfTran)
                {
                    if (_actionStatus.Success)
                    {
                        _logger.Information("GenericRepository.InsertRange", "Entity Inserted successfully,Committing Transaction");
                        var _tactionStatus = _uow.EndTransaction();
                        if (!_tactionStatus.Success) _actionStatus = _tactionStatus;
                    }
                    else
                    {
                        _logger.Information("GenericRepository.InsertRange",
                            "Having issues while Inserting entity,Rollbaking transaction");
                        _uow.RollBack();
                    }
                }
            }

            return _actionStatus;
        }
        public virtual Entity.ActionStatus Update(T entity)
        {
            _logger.Information(Constants.ACTION_ENTRY, "GenericRepository.Update");
            if (entity == null) throw new ArgumentNullException("entity");
            var selfTran = false;
            if (!_uow.InTransaction)
            {
                _uow.BeginTransaction();
                selfTran = true;
            }

            var _actionStatus = new Entity.ActionStatus();
            try
            {
                //code writtern by jully need to be verified
                #region code verification needed
                //if (entity.GetType().Equals(typeof(GreenHouse)))
                //{
                //    GreenHouse greenhouse = (GreenHouse)(object)entity;
                //    if ((greenhouse.IsActive.HasValue && !greenhouse.IsActive.Value) || greenhouse.IsDeleted)
                //    {
                //        if (_uow.DbContext.Crop.Where(t => t.GreenHouseGuid == greenhouse.Guid).Count() > 0)
                //        {
                //            _actionStatus.Success = false;
                //            _actionStatus.Message = "Crop exists";
                //            throw new Exception(_actionStatus.Message);
                //        }
                //        else if (_uow.DbContext.Device.Where(t => t.GreenHouseGuid == greenhouse.Guid).Count() > 0)
                //        {
                //            _actionStatus.Success = false;
                //            _actionStatus.Message = "Device exists";
                //            throw new Exception(_actionStatus.Message);
                //        }
                //    }
                //}
                //else if (entity.GetType().Equals(typeof(Role))) {
                //    Role role = (Role)(object)entity;
                //    if ((role.IsActive.HasValue && !role.IsActive.Value) || role.IsDeleted)
                //    {
                //        if (_uow.DbContext.User.Where(t => t.RoleGuid == role.Guid).Count() > 0)
                //        {
                //            _actionStatus.Success = false;
                //            _actionStatus.Message = "User exists";
                //            throw new Exception(_actionStatus.Message);
                //        }

                //    }
                //}
                #endregion
                SetModified(entity);
                _actionStatus = ApplyChanges();

                if (!_actionStatus.Success) throw new Exception(_actionStatus.Message);
                //_actionStatus.Result = entity.RecordId;
                _actionStatus.Data = entity;
            }
            catch (Exception ex)
            {
                _logger.Error("GenericRepository.Update", ex);
                _actionStatus.Success = false;
                _actionStatus.Message = ex.Message;
            }
            finally
            {
                if (selfTran)
                {
                    if (_actionStatus.Success)
                    {
                        _logger.Information("GenericRepository.Update", "Entity Updated successfully,Committing Transaction");
                        var _tactionStatus = _uow.EndTransaction();
                        if (!_tactionStatus.Success) _actionStatus = _tactionStatus;
                    }
                    else
                    {
                        _logger.Information("GenericRepository.Update",
                            "Having issues while Updating entity,Rollbaking transaction");
                        _uow.RollBack();
                    }
                }
            }

            _logger.Information(Constants.ACTION_EXIT, "GenericRepository.Update");
            return _actionStatus;
        }
        public Entity.ActionStatus Delete(T entity)
        {
            _logger.Information(Constants.ACTION_ENTRY, "GenericRepository.Delete");

            if (entity == null) throw new ArgumentNullException("entity");

            var selfTran = false;
            if (!_uow.InTransaction)
            {
                _uow.BeginTransaction();
                selfTran = true;
            }

            var _actionStatus = new Entity.ActionStatus();
            try
            {
              

                Entities.Remove(entity);
                _actionStatus = ApplyChanges();

                if (!_actionStatus.Success) throw new Exception(_actionStatus.Message);
            }
            catch (Exception ex)
            {
                _logger.Error("GenericRepository.Delete", ex);
                _actionStatus.Success = false;
                _actionStatus.Message = ex.Message;
            }
            finally
            {
                if (selfTran)
                {
                    if (_actionStatus.Success)
                    {
                        _logger.Information("GenericRepository.Delete",
                            "Operation executed successfully,Committing Transaction");
                        _actionStatus = _uow.EndTransaction();
                    }
                    else
                    {
                        _logger.Information("GenericRepository.Delete",
                            "Having issues while deleting entity,Rollbaking transaction");
                        _uow.RollBack();
                    }
                }
            }

            _logger.Information(Constants.ACTION_EXIT, "GenericRepository.Delete");
            return _actionStatus;
        }
        public Entity.ActionStatus RemoveRange(Expression<Func<T, bool>> predicate)
        {
            _logger.Information(Constants.ACTION_ENTRY, "GenericRepository.RemoveRange");
            var _actionStatus = new Entity.ActionStatus(false, string.Empty, string.Empty, null);
            var entityList = Entities.Where(predicate);
            if (entityList != null && entityList.Count() > 0)
            {
                _actionStatus.Success = true;
                return _actionStatus;
            }

            var selfTran = false;
            if (!_uow.InTransaction)
            {
                _uow.BeginTransaction();
                selfTran = true;
            }

            try
            {
                _entities.RemoveRange(entityList);
                _actionStatus = ApplyChanges();

                if (!_actionStatus.Success) throw new Exception(_actionStatus.Message);
            }
            catch (Exception ex)
            {
                _logger.Error("GenericRepository.RemoveRange", ex);
                _actionStatus.Success = false;
                _actionStatus.Message = ex.Message;
            }
            finally
            {
                if (selfTran)
                {
                    if (_actionStatus.Success)
                    {
                        _logger.Information("GenericRepository.RemoveRange",
                            "Operation executed successfully,Committing Transaction");
                        _actionStatus = _uow.EndTransaction();
                    }
                    else
                    {
                        _logger.Information("GenericRepository.RemoveRange",
                            "Having issues while deleting entity,Rollbaking transaction");
                        _uow.RollBack();
                    }
                }
            }

            return _actionStatus;
        }

        #region Private Methods
        private Entity.ActionStatus ApplyChanges()
        {
            var result = new Entity.ActionStatus();
            try
            {
                result = _uow.SaveAndContinue();
                if (!result.Success) throw new Exception(result.Message);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.Error("GenericRepository.SaveChanges", ex);
                result.Message = ex.Message;
                _logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            catch (DbUpdateException ese)
            {
                _logger.Error("GenericRepository.SaveChanges", ese);

                result.Message = ese.Message;
                _logger.Error(Constants.ACTION_EXCEPTION, ese.Message);
            }


            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, ex);
                result.Message = ex.Message;
            }

            return result;
        }
      
        #endregion

        #region Variable Declaration
        protected readonly ILogger _logger;
        protected IUnitOfWork _uow;
        private DbSet<T> _entities;
        #endregion


    }
}