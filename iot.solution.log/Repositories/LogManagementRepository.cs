using component.logger.data.log.Context;
using component.logger.data.log.Enum;
using component.logger.data.log.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace component.logger.data.log.Repositories
{
    /// <summary>
    /// LogManagementRepository
    /// </summary>
    public class LogManagementRepository
    {
        /// <summary>
        /// The context
        /// </summary>
        private LogDataContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogManagementRepository"/> class.
        /// </summary>
        public LogManagementRepository()
        {
            _context = new LogDataContext();
        }

        /// <summary>
        /// Components the list.
        /// </summary>
        /// <returns></returns>
        public DataResponse<List<Component>> ComponentList()
        {
            try
            {
                List<Component> componentList = _context.Components.Select(x => x).ToList();

                DataResponse<List<Component>> responseList = new DataResponse<List<Component>>
                {
                    Status = DataResponseStatus.Success,
                    Data = componentList
                };

                return responseList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Adds the component configuration.
        /// </summary>
        /// <param name="applicationCode">The application code.</param>
        /// <param name="component">The component.</param>
        /// <returns></returns>
        public bool AddComponentConfiguration(string applicationCode, string component)
        {
            try
            {
                Component dbCompon = _context.Components.Where(x => x.KeyName.ToUpper() == $"{component}Key".ToUpper()).FirstOrDefault();
                Guid componentId = new Guid();

                if (dbCompon != null)
                    componentId = dbCompon.ComponentId;
                else
                {
                    componentId = Guid.NewGuid();
                    Component comp = new Component()
                    {
                        ComponentId = componentId,
                        DisplayName = component,
                        KeyName = $"{component}Key",
                        Name = component,
                        CreatedDate = DateTime.UtcNow,
                        ModifiedDate = DateTime.UtcNow
                    };

                    _context.Components.Add(comp);
                    _context.SaveChanges();
                }




                List<ComponentConfiguration> dbComponConfig = _context.ComponentConfigurations.Where(x => x.KeyName.ToUpper() == $"{component}Key".ToUpper()).ToList();

                if (dbComponConfig != null)
                {
                    List<ComponentConfiguration> componConfigList = new List<ComponentConfiguration>();

                    if (!dbComponConfig.Any(x => Convert.ToString(x.ApplicationCode) == string.Empty && x.KeyName.ToUpper() == $"{component}Key".ToUpper()))
                    {
                        componConfigList.Add(new ComponentConfiguration()
                        {
                            ApplicationCode = string.Empty,
                            ComponentConfigId = Guid.NewGuid(),
                            ComponentId = componentId,
                            KeyName = $"{component}Key",
                            Debug = true,
                            Error = true,
                            Fatal = true,
                            Info = true,
                            IsOmsLog = false,
                            Warn = true,
                            ModifiedDate = DateTime.UtcNow,
                            CreatedDate = DateTime.UtcNow,
                            CreatedBy = Guid.Parse("8A21C95A-553D-49D7-AB7E-1F72F0576B02"),
                            ModifiedBy = Guid.Parse("8A21C95A-553D-49D7-AB7E-1F72F0576B02"),
                            SubscribeDebug = true,
                            SubscribeError = true,
                            CronJobDebug = true,
                            CronJobError = true,
                            CronJobFatal = true,
                            CronJobInfo = true,
                            CronJobWarn = true,
                        });
                    }

                    if (!string.IsNullOrWhiteSpace(applicationCode))
                    {
                        if (!dbComponConfig.Any(x => x.ApplicationCode != null && x.ApplicationCode != string.Empty && Convert.ToString(x.ApplicationCode).ToUpper() == applicationCode.ToUpper() && x.KeyName.ToUpper() == $"{component}Key".ToUpper()))
                        {
                            componConfigList.Add(new ComponentConfiguration()
                            {
                                ApplicationCode = applicationCode,
                                ComponentConfigId = Guid.NewGuid(),
                                ComponentId = componentId,
                                KeyName = $"{component}Key",
                                Debug = true,
                                Error = true,
                                Fatal = true,
                                Info = true,
                                IsOmsLog = false,
                                Warn = true,
                                ModifiedDate = DateTime.UtcNow,
                                CreatedDate = DateTime.UtcNow,
                                CreatedBy = Guid.Parse("8A21C95A-553D-49D7-AB7E-1F72F0576B02"),
                                ModifiedBy = Guid.Parse("8A21C95A-553D-49D7-AB7E-1F72F0576B02"),
                                SubscribeDebug = true,
                                SubscribeError = true,
                                CronJobDebug = true,
                                CronJobError = true,
                                CronJobFatal = true,
                                CronJobInfo = true,
                                CronJobWarn = true,
                            });
                        }
                    }

                    if (componConfigList.Count > 0)
                    {
                        _context.ComponentConfigurations.AddRange(componConfigList);
                        _context.SaveChanges();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }

        /// <summary>
        /// Logs the component list.
        /// </summary>
        /// <param name="applicationCode">The application code.</param>
        /// <returns></returns>
        public DataResponse<List<ComponentConfiguration>> LogComponentList(string applicationCode)
        {
            try
            {
                List<ComponentConfiguration> componentConfigurationList = new List<ComponentConfiguration>();

                componentConfigurationList = (from p in _context.ComponentConfigurations
                                              join q in _context.Components on p.ComponentId equals q.ComponentId
                                              where p.ApplicationCode == applicationCode
                                              select new ComponentConfiguration()
                                              {
                                                  Component = q,
                                                  ApplicationCode = p.ApplicationCode,
                                                  ComponentConfigId = p.ComponentConfigId,
                                                  ComponentId = p.ComponentId,
                                                  CreatedBy = p.CreatedBy,
                                                  CreatedDate = p.CreatedDate,
                                                  Debug = p.Debug,
                                                  Error = p.Error,
                                                  Fatal = p.Fatal,
                                                  Info = p.Info,
                                                  KeyName = p.KeyName,
                                                  ModifiedBy = p.ModifiedBy,
                                                  ModifiedDate = p.ModifiedDate,
                                                  Warn = p.Warn,
                                                  IsOmsLog = p.IsOmsLog,
                                                  SubscribeDebug = p.SubscribeDebug,
                                                  SubscribeError = p.SubscribeError,
                                                  CronJobDebug = p.CronJobDebug ?? false,
                                                  CronJobError = p.CronJobError ?? false,
                                                  CronJobFatal = p.CronJobFatal ?? false,
                                                  CronJobInfo = p.CronJobInfo ?? false,
                                                  CronJobWarn = p.CronJobWarn ?? false,
                                              }).ToList();



                DataResponse<List<ComponentConfiguration>> responseList = new DataResponse<List<ComponentConfiguration>>
                {
                    Status = DataResponseStatus.Success,
                    Data = componentConfigurationList
                };

                return responseList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Adds the application configuration.
        /// </summary>
        /// <param name="applicationCode">The application code.</param>
        /// <param name="enableOmsConfig">if set to <c>true</c> [enable oms configuration].</param>
        /// <returns></returns>
        public DataResponse<string> AddApplicationConfiguration(string applicationCode, bool enableOmsConfig)
        {
            try
            {
                List<Component> componentList = _context.Components.ToList();
                foreach (Component item in componentList)
                {
                    ComponentConfiguration appComponentConfiguration = _context.ComponentConfigurations.Where(x => x.ApplicationCode == applicationCode && x.ComponentId == item.ComponentId).FirstOrDefault();
                    if (appComponentConfiguration != null)
                    {
                        appComponentConfiguration.IsOmsLog = enableOmsConfig;
                        appComponentConfiguration.ModifiedDate = DateTime.UtcNow;
                        appComponentConfiguration.ModifiedBy = Guid.Parse("200EDCFA-8FF1-4837-91B1-7D5F967F5129");
                        _context.SaveChanges();
                    }
                    else
                    {
                        ComponentConfiguration componentConfig = new ComponentConfiguration()
                        {
                            ApplicationCode = applicationCode,
                            ComponentConfigId = Guid.NewGuid(),
                            ComponentId = item.ComponentId,
                            CreatedDate = DateTime.UtcNow,
                            Debug = false,
                            Error = true,
                            Fatal = true,
                            Info = true,
                            IsOmsLog = enableOmsConfig,
                            KeyName = $"{item.Name.Trim()}Key",
                            Warn = true,
                            ModifiedDate = DateTime.UtcNow,
                            CreatedBy = Guid.Parse("200EDCFA-8FF1-4837-91B1-7D5F967F5129"),
                            ModifiedBy = Guid.Parse("200EDCFA-8FF1-4837-91B1-7D5F967F5129"),
                            SubscribeError = true,
                            SubscribeDebug = true,
                            CronJobDebug = false,
                            CronJobError = true,
                            CronJobFatal = true,
                            CronJobInfo = true,
                            CronJobWarn = true,
                        };
                        _context.Add(componentConfig);
                        _context.SaveChanges();
                    }
                }

                return new DataResponse<string>()
                {
                    Status = DataResponseStatus.Success,
                    Data = string.Empty
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the severity list.
        /// </summary>
        /// <returns></returns>
        public DataResponse<List<LogConfiguration>> GetSeverityList()
        {
            try
            {
                List<LogConfiguration> componentConfigurationList = new List<LogConfiguration>();
                if (componentConfigurationList != null)
                {
                    componentConfigurationList = _context.LogConfigurations.ToList();
                }
                DataResponse<List<LogConfiguration>> responseList = new DataResponse<List<LogConfiguration>>
                {
                    Status = DataResponseStatus.Success,
                    Data = componentConfigurationList
                };

                return responseList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Cleans the application log.
        /// </summary>
        /// <param name="logsToKeepForDays">The logs to keep for days.</param>
        /// <returns></returns>
        public bool CleanApplicationLog(int logsToKeepForDays)
        {
            //_context.ErrorLogs.RemoveRange(_context.ErrorLogs.Where(c => Convert.ToDateTime(c.LogDate).Date < DateTime.UtcNow.AddDays(-1 * logsToKeepForDays).Date).Take(2000));
            //_context.SaveChanges();
            return true;
        }

        /// <summary>
        /// Get app setting by Key
        /// </summary>
        /// <returns></returns>
        public List<AppSetting> GetAppSetting()
        {
            List<AppSetting> appSettingList = _context.AppSetting.ToList();

            return appSettingList;
        }
    }
}
