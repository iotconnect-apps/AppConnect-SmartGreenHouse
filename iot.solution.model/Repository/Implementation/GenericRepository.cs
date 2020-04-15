using iot.solution.data;
//using iot.solution.entity;
using iot.solution.model.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Entity = iot.solution.entity;
using LogHandler = component.services.loghandler;

namespace iot.solution.model.Repository.Implementation
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class //, IEntityBase
    {
        public GenericRepository(IUnitOfWork unitOfWork, LogHandler.Logger logger)
        {
            ConnectionString = component.helper.SolutionConfiguration.Configuration.ConnectionString;

            if (unitOfWork == null)
                throw new ArgumentNullException("UnitOfWork cannot be null.");
            if (logger == null)
                throw new ArgumentNullException("LogManager cannot be null");

            _uow = unitOfWork;
            _logger = logger;
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
            _logger.InfoLog(LogHandler.Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            var list = Entities.AsNoTracking().Where(predicate);
            _logger.InfoLog(LogHandler.Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            return list;
        }
        public IQueryable<T> GetAll()
        {
            _logger.InfoLog(LogHandler.Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            IQueryable<T> list = Entities;
            _logger.InfoLog(LogHandler.Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            return list;
        }

        public T GetByUniqueId(Expression<Func<T, bool>> predicate)
        {
            _logger.InfoLog(LogHandler.Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            var obj = Entities.AsNoTracking().FirstOrDefault(predicate);
            _logger.InfoLog(LogHandler.Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            return obj;
        }
        public Entity.ActionStatus Insert(T entity)
        {
            _logger.InfoLog(LogHandler.Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
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
                _logger.InfoLog(LogHandler.Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                _actionStatus.Success = false;
                _actionStatus.Message = ex.Message;
            }
            finally
            {
                if (selfTran)
                {
                    if (_actionStatus.Success)
                    {
                        _logger.InfoLog("Entity Inserted successfully,Committing Transaction", null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                        var _tactionStatus = _uow.EndTransaction();
                        if (!_tactionStatus.Success) _actionStatus = _tactionStatus;
                    }
                    else
                    {
                        _logger.InfoLog("Having issues while Inserting entity,Rollbaking transaction", null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                        _uow.RollBack();
                    }
                }
            }

            return _actionStatus;
        }

        public Entity.ActionStatus InsertRange(List<T> entity)
        {
            _logger.InfoLog(LogHandler.Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
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
                _logger.InfoLog(LogHandler.Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                _actionStatus.Success = false;
                _actionStatus.Message = ex.Message;
            }
            finally
            {
                if (selfTran)
                {
                    if (_actionStatus.Success)
                    {
                        _logger.InfoLog("Entity Inserted successfully,Committing Transaction", null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                        var _tactionStatus = _uow.EndTransaction();
                        if (!_tactionStatus.Success) _actionStatus = _tactionStatus;
                    }
                    else
                    {
                        _logger.InfoLog("Having issues while Inserting entity,Rollbaking transaction", null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                        _uow.RollBack();
                    }
                }
            }

            return _actionStatus;
        }
        public virtual Entity.ActionStatus Update(T entity)
        {
            _logger.InfoLog(LogHandler.Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
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
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                _actionStatus.Success = false;
                _actionStatus.Message = ex.Message;
            }
            finally
            {
                if (selfTran)
                {
                    if (_actionStatus.Success)
                    {
                        _logger.InfoLog("Entity Updated successfully,Committing Transaction", null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                        var _tactionStatus = _uow.EndTransaction();
                        if (!_tactionStatus.Success) _actionStatus = _tactionStatus;
                    }
                    else
                    {
                        _logger.InfoLog("Having issues while Updating entity,Rollbaking transaction", null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                        _uow.RollBack();
                    }
                }
            }

            _logger.InfoLog(LogHandler.Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            return _actionStatus;
        }
        public Entity.ActionStatus Delete(T entity)
        {
            _logger.InfoLog(LogHandler.Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
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
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                _actionStatus.Success = false;
                _actionStatus.Message = ex.Message;
            }
            finally
            {
                if (selfTran)
                {
                    if (_actionStatus.Success)
                    {
                        _logger.InfoLog("Operation executed successfully,Committing Transaction", null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                        _actionStatus = _uow.EndTransaction();
                    }
                    else
                    {
                        _logger.InfoLog("Having issues while deleting entity,Rollbaking transaction", null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                        _uow.RollBack();
                    }
                }
            }

            _logger.InfoLog(LogHandler.Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            return _actionStatus;
        }
        public Entity.ActionStatus RemoveRange(Expression<Func<T, bool>> predicate)
        {
            _logger.InfoLog(LogHandler.Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
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
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                _actionStatus.Success = false;
                _actionStatus.Message = ex.Message;
            }
            finally
            {
                if (selfTran)
                {
                    if (_actionStatus.Success)
                    {
                        _logger.InfoLog("Operation executed successfully,Committing Transaction", null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                        _actionStatus = _uow.EndTransaction();
                    }
                    else
                    {
                        _logger.InfoLog("Having issues while deleting entity,Rollbaking transaction", null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                        _uow.RollBack();
                    }
                }
            }

            return _actionStatus;
        }
        public List<T> ExecuteStoredProcedure<T>(string spName, Dictionary<string, string> parameters) where T : new()
        {
            var result = new List<T>();
            try
            {
                _logger.InfoLog(LogHandler.Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<System.Data.Common.DbParameter> dbparameters = new List<System.Data.Common.DbParameter>();
                    if (parameters != null && parameters.Any())
                    {
                        foreach (var param in parameters)
                        {
                            dbparameters.Add(sqlDataAccess.CreateParameter(param.Key, param.Value, DbType.String, ParameterDirection.Input));
                        }
                    }

                    System.Data.Common.DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(
                            sqlDataAccess.CreateCommand(spName, System.Data.CommandType.StoredProcedure, null), dbparameters.ToArray());
                    result = DataUtils.DataReaderToList<T>(dbDataReader, null);
                }
                _logger.InfoLog(LogHandler.Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }
        public int ExecuteStoredProcedureNonQuery(string spName, Dictionary<string, string> parameters)
        {
            int result = 0;
            try
            {
                _logger.InfoLog(LogHandler.Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<System.Data.Common.DbParameter> dbparameters = new List<System.Data.Common.DbParameter>();
                    if (parameters != null && parameters.Any())
                    {
                        foreach (var param in parameters)
                        {
                            dbparameters.Add(sqlDataAccess.CreateParameter(param.Key, param.Value, DbType.String, ParameterDirection.Input));
                        }
                    }

                    result = sqlDataAccess.ExecuteStoredProcedureNonQuery(spName, dbparameters.ToArray());
                }
                _logger.InfoLog(LogHandler.Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
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
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                result.Message = ex.Message;
            }
            catch (DbUpdateException ese)
            {
                _logger.ErrorLog(ese, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                result.Message = ese.Message;
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                result.Message = ex.Message;
            }

            return result;
        }

        #endregion

        #region Variable Declaration
        private readonly LogHandler.Logger _logger;
        protected IUnitOfWork _uow;
        private DbSet<T> _entities;
        #endregion


    }
}