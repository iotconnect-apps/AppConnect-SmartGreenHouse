using System;
using System.Collections.Generic;

namespace iot.solution.host.Models
{
    public partial class HardwareKit
    {
        public Guid Guid { get; set; }
        public Guid KitTypeGuid { get; set; }
        public string KitCode { get; set; }
        public Guid? CompanyGuid { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
