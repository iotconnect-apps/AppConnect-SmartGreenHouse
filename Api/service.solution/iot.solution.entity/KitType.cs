using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity
{
    public class KitType
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

    public partial class KitTypeAttribute
    {
        public string Guid { get; set; }
        public string? ParentTemplateAttributeGuid { get; set; }
        public string TemplateGuid { get; set; }
        public string LocalName { get; set; }
        public string Tag { get; set; }
    }

    public partial class KitTypeCommand
    {
        public Guid Guid { get; set; }
        public Guid TemplateGuid { get; set; }
        public string Name { get; set; }
        public string Command { get; set; }
        public bool RequiredParam { get; set; }
        public bool RequiredAck { get; set; }
        public bool IsOtacommand { get; set; }
        public string Tag { get; set; }
    }
}
