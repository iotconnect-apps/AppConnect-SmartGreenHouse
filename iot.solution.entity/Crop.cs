using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
namespace iot.solution.entity
{
    public class Crop
    {
        public Guid Guid { get; set; }
        public Guid CompanyGuid { get; set; }
        public Guid GreenhouseGuid { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public bool Isactive { get; set; }
        public bool Isdeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
    public class CropModel : Crop
    {
        public IFormFile ImageFile { get; set; }

    }
}
