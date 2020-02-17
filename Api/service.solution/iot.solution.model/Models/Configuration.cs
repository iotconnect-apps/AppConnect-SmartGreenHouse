using System;
using System.Collections.Generic;

namespace iot.solution.model.Models
{
    public partial class Configuration
    {
        public Guid Guid { get; set; }
        public string ConfigKey { get; set; }
        public string Type { get; set; }
        public string DefaultValue { get; set; }
        public bool MustOverride { get; set; }
        public string Options { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
