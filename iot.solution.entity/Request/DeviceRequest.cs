using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace iot.solution.entity
{
    public class DeviceRequest
    {
        [Key]
        public Guid Guid { get; set; }
        [Required]
        public Guid CompanyGuid { get; set; }
        [Required]
        public Guid GreenhouseGuid { get; set; }
        public Guid GatewayGuid { get; set; }
        public byte? Type { get; set; }
        [MaxLength(500)]
        public string UniqueId { get; set; }
        [Required]
        [MaxLength(500)]
        public string Name { get; set; }
        [MaxLength(1000)]
        public string Note { get; set; }
        [MaxLength(50)]
        public string Tag { get; set; }
        [MaxLength(200)]
        public string Image { get; set; }
        public bool? Isactive { get; set; }
        public bool Isdeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
