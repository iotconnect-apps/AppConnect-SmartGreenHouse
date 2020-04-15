using component.logger;
using iot.solution.model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using EF = iot.solution.model.Models;
using Entity = iot.solution.entity;
using LogHandler = component.services.loghandler;
using System.Reflection;


namespace iot.solution.model
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(DbContext dbContext, LogHandler.Logger logManager)
        {
            if (dbContext == null)
                throw new ArgumentNullException("DBContext cannot be null.");
            if (logManager == null)
                throw new ArgumentNullException("LogManager cannot be null");

            DbContext = (EF.qagreenhouseContext) dbContext;
            _logger = logManager;
        }

        private bool _disposed;
        private readonly LogHandler.Logger _logger;
        private IDbContextTransaction _transaction { get; set; }

        public bool InTransaction { get; private set; }
        public EF.qagreenhouseContext DbContext { get; }
        public virtual void BeginTransaction()
        {
            _logger.InfoLog(LogHandler.Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            try
            {
                InTransaction = true;
                _transaction = DbContext.Database.BeginTransaction();
                _logger.InfoLog(LogHandler.Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }
        public virtual Entity.ActionStatus EndTransaction()
        {
            _logger.InfoLog(LogHandler.Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            var status = new Entity.ActionStatus();
            try
            {
                if (_disposed) throw new ObjectDisposedException(GetType().FullName);
                DbContext.SaveChanges();
                _transaction.Commit();
                InTransaction = false;
                status.Success = true;
            }
            //catch (DbEntityValidationException dbEx)
            //{
            //    _logger.Error(Constants.ACTION_EXCEPTION + ":UnitofWork.EndTransaction", dbEx);
            //    status.Message = dbEx.Message;
            //    status.Success = false;
            //    _inTransaction = false;
            //}
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                status.Message = ex.Message;
                status.Success = false;
            }

            _logger.InfoLog(LogHandler.Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            return status;
        }
        public virtual void RollBack()
        {
            _logger.InfoLog(LogHandler.Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);

            try
            {
                _transaction.Rollback();
                _transaction.Dispose();
                InTransaction = false;
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                throw;
            }

            _logger.InfoLog(LogHandler.Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
        }
        public virtual Entity.ActionStatus SaveAndContinue()
        {
            _logger.InfoLog(LogHandler.Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            var status = new Entity.ActionStatus();
            try
            {
                DbContext.SaveChanges();
                status.Success = true;                
            }
            //catch (DbEntityValidationException dbEx)
            //{
            //    _logger.Error(Constants.ACTION_EXCEPTION + ":UnitofWork.SaveAndContinue", dbEx);
            //    var errorMessages = dbEx.EntityValidationErrors
            //        .SelectMany(x => x.ValidationErrors)
            //        .Select(x => x.ErrorMessage);

            //    status.Message = string.Join("; ", errorMessages);
            //    status.Success = false;
            //}
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                status.Message = ex.Message;
                status.Success = false;
            }

            _logger.InfoLog(LogHandler.Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            return status;
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing && DbContext != null && InTransaction) _transaction.Dispose();
            if (disposing && DbContext != null) DbContext.Dispose();

            _disposed = true;
        }
        public void Dispose()
        {
            _logger.InfoLog(LogHandler.Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            Dispose(true);
            GC.SuppressFinalize(this);
            _logger.InfoLog(LogHandler.Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
        }
    }
}