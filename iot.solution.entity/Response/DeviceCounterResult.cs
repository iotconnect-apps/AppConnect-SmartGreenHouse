using System;
using System.Collections.Generic;

namespace iot.solution.entity
{
    public class DeviceCounterResult
    {
        public int active { get; set; }
        public int inActive { get; set; }
        public int connected { get; set; }
        public int disConnected { get; set; }
        public int acquired { get; set; }
        public int available { get; set; }
        public int total { get; set; }
    }

    public class DeviceConnectionStatusResult
    {
        public bool IsConnected { get; set; }
    }

    public class DeviceTelemetryDataResult
    {
        public string templateAttributeGuid { get; set; }
        public string attributeName { get; set; }
        public string attributeValue { get; set; }
        public System.DateTime deviceUpdatedDate { get; set; }
        public int notificationCount { get; set; }
        public object aggregateType { get; set; }
        public string DataType { get; set; }
        public object aggregateTypeValues { get; set; }
    }

}
