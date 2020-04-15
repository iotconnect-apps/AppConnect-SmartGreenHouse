using System;
using System.Collections.Generic;

namespace iot.solution.entity.Response
{
    public class EnergyUsageResponse
    {
        public string Month { get; set; }
        public string Value { get; set; }
    }
    public class WaterUsageResponse
    {
        public string Month { get; set; }
        public string Value { get; set; }
    }

    public class SoilNutritionResponse
    {
        public string PHLevel { get; set; }
        public string N { get; set; }
        public string P { get; set; }
        public string K { get; set; }
    }

    public class ConfgurationResponse
    {
        public string cpId { get; set; }
        public string host { get; set; }
        public int isSecure { get; set; }
        public string password { get; set; }
        public int port { get; set; }
        public string url { get; set; }
        public string user { get; set; }
        public string vhost { get; set; }
    }
}
