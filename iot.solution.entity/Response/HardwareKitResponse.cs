using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity
{
    public class HardwareKitResponse
    {
        public Guid Guid { get; set; }
        public Guid KitTypeGuid { get; set; }
        public string KitTypeName { get; set; }
        public string CompanyName { get; set; }
        public string KitCode { get; set; }
        public Guid? CompanyGuid { get; set; }
        public int DeviceCount { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
