using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity
{
    public class SingleRuleResponse
    {
        public List<string> Roles { get; set; }
        public string Tag { get; set; }
        public string EventSubscriptionGuid { get; set; }
        public bool IsActive { get; set; }
        public int ApplyTo { get; set; }
        public string EntityGuid { get; set; }
        public bool IgnorePreference { get; set; }
        public string ConditionText { get; set; }
        public string Name { get; set; }
        public string ParentAttributeGuid { get; set; }
        public int RuleType { get; set; }
        public List<string> AttributeGuid { get; set; }
        public string TemplateGuid { get; set; }
        public string CompanyGuid { get; set; }
        public string Guid { get; set; }
        public List<string> Users { get; set; }
        public List<string> Devices { get; set; }
    }
}
