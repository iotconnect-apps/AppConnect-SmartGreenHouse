using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity.Structs.Routes
{
    public struct DeviceRoute
    {
        public struct Name
        {
            public const string Add = "device.add";
            public const string GetList = "device.list";
            public const string GetById = "device.getdevicebyid";
            public const string Delete = "device.deletedevice";
            public const string BySearch = "device.search";
            public const string UpdateStatus = "device.updatestatus";
            public const string ChildDevice = "device.childdevicelist";
            public const string ValidateKit = "device.validatekit";
            public const string ProvisionKit = "device.provisionkit";
            public const string GetGreenHouseDevices = "device.getgreenhousedevices";
            public const string GetGreenHouseDevicesDetails = "device.getgreenhousedevicesdetails";
        }

        public struct Route
        {
            public const string Global = "api/device";
            public const string Manage = "manage";
            public const string GetList = "";
            public const string GetById = "{id}";
            public const string Delete = "delete/{id}";
            public const string UpdateStatus = "updatestatus/{id}/{status}";
            public const string BySearch = "search";
            public const string ChildDevice = "childdevicelist";
            public const string ValidateKit = "validatekit/{kitCode}";
            public const string ProvisionKit = "provisionkit";
            public const string GetGreenHouseDevices = "getgreenhousedevices/{greenhouseId}";
            public const string GetGreenHouseDevicesDetails = "getgreenhousedevicesdetails/{greenhouseId}";
        }
    }
}
