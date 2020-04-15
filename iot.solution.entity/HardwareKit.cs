using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity
{
    public class HardwareKit
    {
        public Guid Guid { get; set; }
        public Guid TemplateGuid { get; set; }
        public Guid KitTypeGuid { get; set; }
        public string KitType { get; set; }
        public string KitCode { get; set; }
        public Guid? CompanyGuid { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public Guid KitGuid { get; set; }
        public Guid? ParentDeviceGuid { get; set; }
        public string ParentUniqueId { get; set; }
        public string UniqueId { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public string Tag { get; set; }
        public bool? IsProvisioned { get; set; }
        public Guid TagGuid { get; set; }

        public List<KitDevice> KitDevices { get; set; }
    }
}
