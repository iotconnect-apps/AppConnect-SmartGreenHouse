﻿using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity
{
    public class KitDevice
    {
        public Guid Guid { get; set; }
        public Guid KitGuid { get; set; }
        public Guid? ParentDeviceGuid { get; set; }
        public string UniqueId { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public Guid? TagGuid { get; set; }
        public bool? IsProvisioned { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
        public Guid TemplateGuid { get; set; }
        public string TemplateName { get; set; }
    }
}
