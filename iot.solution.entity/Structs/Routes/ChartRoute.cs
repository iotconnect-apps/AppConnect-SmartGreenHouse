using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity.Structs.Routes
{
    public class ChartRoute
    {
        public struct Name
        {
            public const string EnergyUsage = "chart.energyusage";
            public const string WaterUsage = "chart.waterusage";
            public const string SoilNutrition = "chart.soilnutrition";
        }

        public struct Route
        {
            public const string Global = "api/chart";
            public const string EnergyUsage = "getenergyusage";
            public const string WaterUsage = "getwaterusage";
            public const string SoilNutrition = "getsoilnutrition";
        }
    }
}
