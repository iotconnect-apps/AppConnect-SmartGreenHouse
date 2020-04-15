using System;

namespace iot.solution.entity.Response
{
    public class GreenHouseCropResponse
    {
        public Guid CropGuid { get; set; }
        public string CropName { get; set; }
        public string Image { get; set; }
    }
}
