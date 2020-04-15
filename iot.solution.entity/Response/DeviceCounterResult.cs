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
}
