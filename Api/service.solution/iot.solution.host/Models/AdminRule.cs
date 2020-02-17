using System;
using System.Collections.Generic;

namespace iot.solution.host.Models
{
    public partial class AdminRule
    {
        public Guid Guid { get; set; }
        public Guid TemplateGuid { get; set; }
        public string AttributeGuid { get; set; }
        public short RuleType { get; set; }
        public string Name { get; set; }
        public string ConditionText { get; set; }
        public bool IgnorePreference { get; set; }
        public short? ApplyTo { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
        public Guid? EventSubscriptionGuid { get; set; }
        public Guid? SeverityLevelGuid { get; set; }
        public string Tag { get; set; }
    }
}
