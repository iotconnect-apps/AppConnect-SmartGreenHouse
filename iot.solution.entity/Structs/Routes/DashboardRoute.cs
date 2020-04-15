using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity.Structs.Routes
{
    public class DashboardRoute
    {
        public struct Name
        {
            public const string GetFarms = "dashboard.getfarmlookup";
            public const string GetOverview = "dashboard.getoverview";
            public const string GetGreenHouseDetail = "dashboard.getgreenhousedetail";
            public const string GetDeviceDetail = "dashboard.getdevicedetail";
            public const string GetGreenHouseCorp = "dashboard.getgreenhousecorp";
            public const string GetGreenHouseDevices = "dashboard.getgreenhousedevices";
            public const string GetGreenHouseChildDevices = "dashboard.getgreenhousechilddevices";
        }
        public struct Route
        {
            public const string Global = "api/dashboard";
            public const string GetFarms = "getcompanygreenhouse/{companyId}";
            public const string GetOverview = "overview/{companyId}";
            public const string GetGreenHouseDetail = "getgreenhousedetail/{greenhouseId}";
            public const string GetDeviceDetail = "getdevicedetail/{deviceId}";
            public const string GetGreenHouseCorp = "getgreenhousecorp/{greenhouseId}";
            public const string GetGreenHouseDevices = "getgreenhousedevices/{greenhouseId}";
            public const string GetGreenHouseChildDevices = "getgreenhousechilddevices/{deviceId}";
        }
    }
}
