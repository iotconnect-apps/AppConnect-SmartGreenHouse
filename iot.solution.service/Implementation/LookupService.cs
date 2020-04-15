using component.helper;
using iot.solution.entity;
using iot.solution.model.Repository.Interface;
using iot.solution.service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Entity = iot.solution.entity;
using LogHandler = component.services.loghandler;

namespace iot.solution.service.Data
{
    public class LookupService : ILookupService
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly IHardwareKitRepository _hardwareKitRepository;
        private readonly IGreenHouseRepository _greenHouseRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IKitTypeRepository _kitTypeRepository;
        private readonly IkitTypeCommandRepository _kitTypeCommandRepository;
        private readonly IKitTypeAttributeRepository _kitTypeAttributeRepository;
        private readonly LogHandler.Logger _logger;
        private readonly IotConnectClient _iotConnectClient;

        public LookupService(LogHandler.Logger logger, IDeviceRepository deviceRepository
            , IGreenHouseRepository greenHouseRepository, IHardwareKitRepository hardwareKitRepository
            , ICompanyRepository companyRepository
            , IKitTypeRepository kitTypeRepository, IKitTypeAttributeRepository kitTypeAttributeRepository
            , IkitTypeCommandRepository kitTypeCommandRepository)
        {
            _logger = logger;
            _deviceRepository = deviceRepository;
            _greenHouseRepository = greenHouseRepository;
            _hardwareKitRepository = hardwareKitRepository;
            _companyRepository = companyRepository;
            _kitTypeAttributeRepository = kitTypeAttributeRepository;
            _kitTypeRepository = kitTypeRepository;
            _kitTypeCommandRepository = kitTypeCommandRepository;
            _iotConnectClient = new IotConnectClient(SolutionConfiguration.BearerToken, SolutionConfiguration.Configuration.EnvironmentCode, SolutionConfiguration.Configuration.SolutionKey);
        }
        
        public string GetIotTemplateGuidByName(string templateName)
        {
            string templateGuid = string.Empty;
            var templates = _iotConnectClient.Template.All(new IoTConnect.Model.PagingModel() { SearchText = templateName, PageNo = 1, PageSize = 1000 }).Result;
            if (templates != null && templates.data != null && templates.data.Any())
            {
                templateGuid = templates.data[0].Guid;
            }
            return templateGuid;
        }
        public List<Entity.LookupItem> Get(string type, string param)
        {
            List<Entity.LookupItem> result = new List<Entity.LookupItem>();
            try
            {
                switch (type.ToLower())
                {
                    case "templates":
                        var templates = _iotConnectClient.Template.All(new IoTConnect.Model.PagingModel() { PageNo = 1, PageSize = 1000 }).Result;
                        if (templates != null && templates.data != null && templates.data.Any())
                        {
                            result = (from r in templates.data.Where(t => (!string.IsNullOrWhiteSpace(t.Tag)) == false) select new Entity.LookupItem() { Value = r.Guid, Text = r.Name }).ToList();
                        }
                        break;
                    case "iotentity":
                        var entity = _iotConnectClient.GreenHouse.All().Result;
                        if (entity != null && entity.data != null && entity.data.Any())
                        {
                            result = (from r in entity.data select new Entity.LookupItem() { Value = r.Guid, Text = r.Name }).ToList();
                        }
                        break;
                    case "iotdevice":
                        if (string.IsNullOrWhiteSpace(param)) { throw new System.Exception("Kit Type is missing in request"); }
                        var device = _iotConnectClient.Device.AllDevice(new IoTConnect.Model.AllDeviceModel { templateGuid = param }).Result;
                        if (device != null && device.Data != null && device.Data.Any())
                        {
                            result = (from r in device.Data select new Entity.LookupItem() { Value = r.Guid, Text = r.DisplayName }).ToList();
                        }
                        break;
                    case "iotuser":
                        var user = _iotConnectClient.User.Search(new IoTConnect.Model.SearchUserModel { SearchText = "", PageNo = -1, PageSize = -1, SortBy = "" }).Result;
                        if (user != null && user.data != null && user.data.data.Any())
                        {
                            result = (from r in user.data.data.Where(t => t.IsActive) select new Entity.LookupItem() { Value = r.Guid, Text = r.Name }).ToList();
                        }
                        break;
                    case "devicetags":
                        if (string.IsNullOrWhiteSpace(param)) { throw new System.Exception("templateGuid is missing in request"); }
                        var tags = _iotConnectClient.Master.AllAttributeLookup(param).Result;
                        if (tags != null && tags.data != null && tags.data.Any())
                        {
                            result = (from r in tags.data select new Entity.LookupItem() { Value = r.guid, Text = r.localname }).ToList();
                        }
                        break;
                    case "device":
                        result = _deviceRepository.GetDeviceLookup();
                        break;
                    case "gateway":
                        result = _deviceRepository.GetGetwayLookup();
                        break;
                    case "greenhouse":
                        if (string.IsNullOrWhiteSpace(param)) { throw new System.Exception("Companyid is missing in request"); }
                        result = _greenHouseRepository.GetLookup(System.Guid.Parse(param));
                        break;
                    case "role":
                        var roles = _iotConnectClient.Master.AllRoleLookup().Result;
                        if (roles != null && roles.data != null && roles.data.Any())
                        {
                            result = (from r in roles.data.Where(t => t.IsActive) select new Entity.LookupItem() { Value = r.Guid, Text = r.Name }).ToList();
                        }
                        break;
                    case "country":
                        var countries = _iotConnectClient.Master.Countries().Result;
                        if (countries != null && countries.data != null && countries.data.Any())
                        {
                            result = (from r in countries.data select new Entity.LookupItem() { Value = r.guid.ToLower(), Text = r.name }).ToList();
                        }
                        break;
                    case "state":
                        if (string.IsNullOrWhiteSpace(param)) { throw new System.Exception("CountryId is missing in request"); }
                        var states = _iotConnectClient.Master.States(param).Result;
                        if (states != null && states.data != null && states.data.Any())
                        {
                            result = (from r in states.data select new Entity.LookupItem() { Value = r.guid.ToLower(), Text = r.name }).ToList();
                        }
                        break;
                    case "timezone":
                        var timeZones = _iotConnectClient.Master.TimeZones().Result;
                        if (timeZones != null && timeZones.data != null && timeZones.data.Any())
                        {
                            result = (from r in timeZones.data select new Entity.LookupItem() { Value = r.guid, Text = r.name }).ToList();
                        }
                        break;
                    case "severitylevel":
                        result = new List<Entity.LookupItem>();
                        result.Add(new LookupItem() { Text = "Critical ", Value = "48C15691-F2EB-40BC-9BF2-0091821AE89B" });
                        result.Add(new LookupItem() { Text = "Information ", Value = "AB1D53A6-009C-4867-8E0E-EC34011EEBC0" });
                        result.Add(new LookupItem() { Text = "Major ", Value = "D6392057-8E35-428D-9281-EFD2BA3C6ED7" });
                        result.Add(new LookupItem() { Text = "Minor ", Value = "6E6D2DCD-E432-442D-9EAC-23CAE1F0CE03" });
                        result.Add(new LookupItem() { Text = "Warning ", Value = "704F4CA0-DB95-4F22-85D3-670F66DEEBA7" });
                        break;
                    case "condition":
                        result = new List<Entity.LookupItem>();
                        result.Add(new LookupItem() { Text = "is equal to", Value = "=" });
                        result.Add(new LookupItem() { Text = "is not equal to", Value = "!=" });
                        result.Add(new LookupItem() { Text = "is greater than", Value = ">" });
                        result.Add(new LookupItem() { Text = "is greater than or equal to", Value = ">=" });
                        result.Add(new LookupItem() { Text = "is less than", Value = "<" });
                        result.Add(new LookupItem() { Text = "is less than or equal to", Value = "<=" });
                        break;
                    default:
                        result = new List<Entity.LookupItem>();
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }
        public List<Entity.LookupItem> GetAllTemplate()
        {
            List<Entity.LookupItem> result = new List<Entity.LookupItem>();
            try
            {

                result = (from template in _kitTypeRepository.FindBy(r => r.IsActive && !r.IsDeleted)
                          select new Entity.LookupItem()
                          {
                              Text = template.Name,
                              Value = template.Guid.ToString().ToUpper()
                          }).ToList();

            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }
        public List<Entity.GreenhouseLookup> GetNestedGreenhouseDetails()
        {
            List<Entity.GreenhouseLookup> result = new List<Entity.GreenhouseLookup>();
            try
            {
                var entity = _iotConnectClient.GreenHouse.All().Result;
                if (entity != null && entity.data != null && entity.data.Any())
                {
                    result = entity.data.Select(x => new Entity.GreenhouseLookup()
                    {
                        Value = x.Guid,
                        Text = x.Name,
                        ParentEntityGuid = x.ParentEntityGuid
                    }).OrderByDescending(x => x.ParentEntityGuid == null).ToList();                    
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }
        public List<Entity.LookupItem> GetAllTemplateFromIoT()
        {
            List<Entity.LookupItem> result = new List<Entity.LookupItem>();
            try
            {
                var templateList = _iotConnectClient.Template.All(new IoTConnect.Model.PagingModel()
                {
                    PageNo = 1,
                    PageSize = 50,
                    SearchText = "",
                    SortBy = ""
                }).Result.data;

                result = templateList.Select(x => new Entity.LookupItem()
                {
                    Text = x.Name,
                    Value = x.Guid.ToString().ToUpper()
                }).ToList();

            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }
        public List<Entity.KitTypeAttribute> GetAllAttributesFromIoT(string templateGuid)
        {
            List<Entity.KitTypeAttribute> result = new List<Entity.KitTypeAttribute>();
            try
            {
                List<IoTConnect.Model.AttributeResult> attributeList = _iotConnectClient.Template.AllAttribute(templateGuid, new IoTConnect.Model.PagingModel() { }, "").Result.data;

                result = attributeList.Select(x => new Entity.KitTypeAttribute()
                {
                    LocalName = x.localName,
                    Guid = x.guid,
                    Tag = x.tag.ToString()
                }).ToList();

            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }

            return result;
        }
        public List<Entity.LookupItem> GetAllCommandsFromIoT(string templateGuid)
        {
            List<Entity.LookupItem> result = new List<Entity.LookupItem>();
            try
            {
                List<IoTConnect.Model.AllCommandResult> attributeList = _iotConnectClient.Template.AllTemplateCommand(templateGuid, new IoTConnect.Model.PagingModel() { }).Result.data;

                return attributeList.Select(x => new Entity.LookupItem()
                {
                    Text = x.name,
                    Value = x.guid.ToUpper()
                }).ToList();

            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }
        public List<Entity.LookupItem> GetTemplate()
        {
            List<Entity.LookupItem> result = new List<Entity.LookupItem>();
            try
            {

                result = (from t in _kitTypeRepository.FindBy(r => r.IsActive && !r.IsDeleted)
                          select new Entity.LookupItem()
                          {
                              Text = t.Name,
                              Value = t.Guid.ToString().ToUpper()
                          }).ToList();

            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }
        public List<Entity.LookupItem> GetSensors(Guid templateId, Guid deviceId)
        {
            List<Entity.LookupItem> result = new List<Entity.LookupItem>();
            try
            {
                var template = _kitTypeRepository.FindBy(t => !t.IsDeleted).FirstOrDefault();//.Name == SolutionConfiguration.DefaultIoTTemplateName
                if (template != null)
                {
                    var childAttribute = (from child in _kitTypeAttributeRepository.FindBy(t => t.KittypeGuid == template.Guid && t.ParentTemplateAttributeGuid != null)
                                          select child.ParentTemplateAttributeGuid).ToList();


                    result.AddRange(from device in _deviceRepository.FindBy(t => t.Guid == deviceId)
                                    join attribute in _kitTypeAttributeRepository.FindBy(t => t.KittypeGuid == template.Guid) on device.Tag equals attribute.Tag
                                    where !childAttribute.Contains(attribute.Guid)
                                    select new Entity.LookupItem()
                                    {
                                        Text = string.Format("{0}", attribute.LocalName),
                                        Value = attribute.Guid.ToString()
                                    });
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }
        public List<Entity.LookupItem> GetTemplateAttribute(Guid templateId)
        {
            List<Entity.LookupItem> result = new List<Entity.LookupItem>();
            try
            {
                var template = _kitTypeRepository.FindBy(t => t.Guid == templateId).FirstOrDefault();
                if (template != null)
                {
                    result.Add(new LookupItem() { Text = string.Format("{0} (Used by Parent)", template.Tag), Value = "" });
                    result.AddRange(from t in _kitTypeAttributeRepository.FindBy(t => t.KittypeGuid == templateId).ToList()
                                    select new Entity.LookupItem()
                                    {
                                        Text = string.Format("{0}({1})", t.LocalName, t.Tag),
                                        Value = t.Guid.ToString().ToUpper() //string.Format("{0}({1})", t.LocalName, t.Tag)
                                    });
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }
        public List<Entity.LookupItem> GetTemplateCommands(Guid templateId)
        {
            List<Entity.LookupItem> result = new List<Entity.LookupItem>();
            try
            {
                var template = _kitTypeRepository.FindBy(t => t.Guid == templateId).FirstOrDefault();
                if (template != null)
                {
                    result = (from t in _kitTypeCommandRepository.GetAll()
                              select new Entity.LookupItem()
                              {
                                  Text = t.Name,
                                  Value = t.Guid.ToString().ToUpper()
                              }).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }
    }
}