using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity
{
    public class DeviceResponse
    {
        public Guid Guid { get; set; }
        public Guid CompanyGuid { get; set; }
        public Guid GreenhouseGuid { get; set; }
        public Guid GatewayGuid { get; set; }
        public byte? Type { get; set; }
        public string UniqueId { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public string Tag { get; set; }
        public string Image { get; set; }
        public bool? isProvisioned { get; set; }

        public bool? Isactive { get; set; }
        public bool Isdeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
    public class DeviceSearchResponse : DeviceResponse
    {
        public string GreenHouseName { get; set; }
        public int Count { get; set; }
        public string Acquired { get; set; }
        
    }
    public class DeviceDetailResponse : Device
    {
       
        public string GreenHouseName { get; set; }  

    }
}
