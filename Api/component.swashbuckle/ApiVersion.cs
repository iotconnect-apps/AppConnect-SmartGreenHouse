namespace component.swashbuckle
{
    public class ApiVersion
    {
        /// <summary>
        /// Api Tile
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// version name e.g. v1, v2
        /// </summary>
        public string VersionName { get; set; }
        /// <summary>
        /// Version Number e.g. v1.0, v1.1
        /// </summary>
        public string VersionNumber { get; set; }
        /// <summary>
        /// Api Description
        /// </summary>
        public string Description { get; set; }
    }
}
