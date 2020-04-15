namespace iot.solution.entity
{
    public class OverviewResponse
    {

        public int TotalGreenhouse { get; set; }
        public int TotalCorp { get; set; }
        public int TotalConnectedDevices { get; set; }
        public int TotalDisconnectedDevices { get; set; }
    }
    public class DashboardOverviewResponse
    {
        //public System.Guid? Guid { get; set; }
        public int GreenHouseCount { get; set; }
        public int CropCount { get; set; }
        public int ConnectedDeviceCount { get; set; }
        public int DisconnectedDeviceCount { get; set; }
        public int AlertsCount { get; set; }
    }
}
