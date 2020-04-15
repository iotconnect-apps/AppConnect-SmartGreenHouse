using System;
using System.Collections.Generic;

namespace iot.solution.model.Models
{
    public partial class TelemetrySummaryHourwise
    {
        public Guid Guid { get; set; }
        public Guid DeviceGuid { get; set; }
        public DateTime Date { get; set; }
        public string Attribute { get; set; }
        public int? Min { get; set; }
        public int? Max { get; set; }
        public int? Avg { get; set; }
        public int? Latest { get; set; }
        public long? Sum { get; set; }
    }
}
