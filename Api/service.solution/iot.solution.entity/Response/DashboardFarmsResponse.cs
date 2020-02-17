using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity.Response
{
    public class DashboardFarmsResponse
    {
        public Guid CompanyGuid { get; set; }
        public Guid GreenhouseName { get; set; }
        public string FarmsName { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string Zipcode { get; set; }
        public Guid? StateGuid { get; set; }
        public Guid? CountryGuid { get; set; }
        public string Image { get; set; }
        public float EnergyUsages { get; set; }
        public float Temperature { get; set; }
        public float Moisture { get; set; }
        public float Humidity { get; set; }
        public float WaterUsages { get; set; }
        public float TotalDevices { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
    }
}
