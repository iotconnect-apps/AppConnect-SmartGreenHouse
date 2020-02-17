using component.common.model;
using component.helper;
using component.logger;
using iot.solution.entity;
using iot.solution.model.Repository.Interface;
using iot.solution.service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using Entity = iot.solution.entity;

namespace iot.solution.service.Data
{
    public class LookupService : ILookupService
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly IHardwareKitRepository _hardwareKitRepository;
        private readonly IGreenHouseRepository _greenHouseRepository;
        private readonly IKitTypeRepository _kitTypeRepository;
        private readonly IkitTypeCommandRepository _kitTypeCommandRepository;
        private readonly IKitTypeAttributeRepository _kitTypeAttributeRepository;
        private readonly ILogger _logger;
        private readonly IotConnectClient _iotConnectClient;

        public LookupService(ILogger logManager, IDeviceRepository deviceRepository
            , IGreenHouseRepository greenHouseRepository, IHardwareKitRepository hardwareKitRepository
            , IKitTypeRepository kitTypeRepository, IKitTypeAttributeRepository kitTypeAttributeRepository
            , IkitTypeCommandRepository kitTypeCommandRepository)
        {
            _logger = logManager;
            _deviceRepository = deviceRepository;
            _greenHouseRepository = greenHouseRepository;
            _hardwareKitRepository = hardwareKitRepository;
            _kitTypeAttributeRepository = kitTypeAttributeRepository;
            _kitTypeRepository = kitTypeRepository;
            _kitTypeCommandRepository = kitTypeCommandRepository;
            _iotConnectClient = new IotConnectClient(AppConfig.BearerToken, AppConfig.EnvironmentCode, AppConfig.SolutionKey);
        }

        public List<Entity.LookupItem> Get(string type, string param)
        {
            List<Entity.LookupItem> result = new List<Entity.LookupItem>();
            switch (type.ToLower())
            {
                case "iotentity":
                    var entity = _iotConnectClient.GreenHouse.All().Result;
                    if (entity != null && entity.data != null && entity.data.Any())
                    {
                        result = (from r in entity.data select new Entity.LookupItem() { Value = r.Guid, Text = r.Name }).ToList();
                    }
                    break;
                case "iotdevice":
                    if (string.IsNullOrWhiteSpace(param)) { throw new System.Exception("Companyid is missing in request"); }
                    var device = _iotConnectClient.Device.AllDevice(new IoTConnect.Model.AllDeviceModel{ templateGuid = param }).Result;
                    if (device != null && device.Data != null && device.Data.Any())
                    {
                        result = (from r in device.Data select new Entity.LookupItem() { Value = r.Guid, Text = r.DisplayName }).ToList();
                    }
                    break;
                case "iotuser":
                    var user = _iotConnectClient.User.Search(new IoTConnect.Model.SearchUserModel {SearchText="",PageNo=-1,PageSize=-1,SortBy="" }).Result;
                    if (user != null && user.data != null && user.data.data.Any())
                    {
                        result = (from r in user.data.data select new Entity.LookupItem() { Value = r.Guid, Text = r.Name }).ToList();
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
                        result = (from r in roles.data select new Entity.LookupItem() { Value = r.Guid, Text = r.Name }).ToList();
                    }
                    break;
                case "country":
                    var countries = _iotConnectClient.Master.Countries().Result;
                    if (countries != null && countries.data != null && countries.data.Any())
                    {
                        result = (from r in countries.data select new Entity.LookupItem() { Value = r.guid, Text = r.name }).ToList();
                    }
                    break;
                case "state":
                    if (string.IsNullOrWhiteSpace(param)) { throw new System.Exception("CountryId is missing in request"); }
                    var states = _iotConnectClient.Master.States(param).Result;
                    if (states != null && states.data != null && states.data.Any())
                    {
                        result = (from r in states.data select new Entity.LookupItem() { Value = r.guid, Text = r.name }).ToList();
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
                    result.Add(new LookupItem() { Text = "is not equal to", Value = "!" });
                    result.Add(new LookupItem() { Text = "is greater than", Value = ">" });
                    result.Add(new LookupItem() { Text = "is greater than or equal to", Value = ">=" });
                    result.Add(new LookupItem() { Text = "is less than", Value = "<" });
                    result.Add(new LookupItem() { Text = "is less than or equal to", Value = "<=" });
                    break;
                default:
                    result = new List<Entity.LookupItem>();
                    break;
            }
            return result;
        }
        public List<Entity.LookupItem> GetTemplate()
        {
            List<Entity.LookupItem> result = new List<Entity.LookupItem>();

            result = (from t in _kitTypeRepository.FindBy(r => r.IsActive && !r.IsDeleted)
                      select new Entity.LookupItem()
                      {
                          Text = t.Name,
                          Value = t.Guid.ToString().ToUpper()
                      }).ToList();

            //var templates = _iotConnectClient.Template.All(new IoTConnect.Model.PagingModel() { PageNo = 1, PageSize = 1000 }).Result;
            //if (templates != null && templates.data != null && templates.data.Any())
            //{
            //    result = (from r in templates.data.Where(t => (!string.IsNullOrWhiteSpace(t.Tag)) == isGateway) select new Entity.LookupItem() { Value = r.Guid, Text = r.Name }).ToList();
            //}
            return result;
        }
        public List<Entity.LookupItem> GetTemplateAttribute(Guid templateId)
        {
            List<Entity.LookupItem> result = new List<Entity.LookupItem>();

            var template = _kitTypeRepository.FindBy(t => t.Guid == templateId).FirstOrDefault();
            if (template != null)
            {
                //result.Add(new Entity.LookupItem() { tag = template.Tag, templateTag = true });
                result.AddRange(from t in _kitTypeAttributeRepository.GetAll()
                                select new Entity.LookupItem()
                                {
                                    Text = string.Format("{0}({1})", t.LocalName, t.Tag),
                                    Value = t.Guid.ToString()
                                });
            }
            return result;
        }
        public List<Entity.LookupItem> GetTemplateCommands(Guid templateId)
        {
            List<Entity.LookupItem> result = new List<Entity.LookupItem>();
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
            return result;
        }
    }
}