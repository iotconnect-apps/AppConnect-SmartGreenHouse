using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity.Structs.Routes
{
    public class GreenHouseRoute
    {
        public struct Name
        {
            public const string Add = "greenhouse.add";
            public const string GetList = "greenhouse.list";
            public const string GetById = "greenhouse.getgreenhousebyid";
            public const string Delete = "greenhouse.deletegreenhouse";
            public const string BySearch = "greenhouse.search";
            public const string UpdateStatus = "greenhouse.updatestatus";
            public const string EnergyUsage = "greenhouse.energyusage";
            public const string WaterUsage = "greenhouse.waterusage";
            public const string SoilNutrition = "greenhouse.soilnutrition";
        }

        public struct Route
        {
            public const string Global = "api/greenhouse";
            public const string Manage = "manage";
            public const string GetList = "";
            public const string GetById = "{id}";
            public const string Delete = "delete/{id}";
            public const string UpdateStatus = "updatestatus/{id}/{status}";
            public const string BySearch = "search";
            public const string EnergyUsage = "getenergyusage/{greenhouseId}";
            public const string WaterUsage = "getwaterusage/{greenhouseId}";
            public const string SoilNutrition = "getsoilnutrition/{greenhouseId}";
        }
    }
}
