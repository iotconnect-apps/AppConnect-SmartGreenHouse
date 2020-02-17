using System;
using System.Collections.Generic;

namespace iot.solution.entity
{
    public class Notification
    {
        public Guid Guid { get; set; }
        public Guid TemplateGuid { get; set; }
        public string AttributeGuid { get; set; }
        public string EntityGuid { get; set; }
        public Guid EventSubscriptionGuid { get; set; }
        public Guid SeverityLevelGuid { get; set; }
        public Int16 RuleType { get; set; }
        public string Name { get; set; }
        public string ParameterValue { get; set; }
        public string ConditionText { get; set; }
        public List<string> DeliveryMethod { get; set; }
        public bool IgnorePreference { get; set; }
        public Int16 ApplyTo { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool Tag { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}