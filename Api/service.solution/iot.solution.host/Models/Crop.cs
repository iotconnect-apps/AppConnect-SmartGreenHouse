﻿
using System;
using System.Collections.Generic;

namespace iot.solution.host.Models
{
    public partial class Crop
    {
        public Guid Guid { get; set; }
        public Guid CompanyGuid { get; set; }
        public Guid GreenHouseGuid { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
    
}
