namespace component.authentication
{
    public class ApiAuthentication
    {
        public string ApiName { get; set; }
        public string ApiSecret { get; set; }
        public bool EnableCaching { get; set; }
        public int CacheDuration { get; set; }
        public bool RequireHttpsMetadata { get; set; }
    }
}
