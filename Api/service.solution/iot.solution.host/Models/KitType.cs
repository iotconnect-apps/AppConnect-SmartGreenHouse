using System;
using System.Collections.Generic;

namespace iot.solution.host.Models
{
    public partial class KitType
    {
        public Guid Guid { get; set; }
        public Guid TemplateGuid { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
