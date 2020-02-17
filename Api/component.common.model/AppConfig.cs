using Microsoft.Extensions.Configuration;
using System;

namespace component.common.model
{
    public static class AppConfig
    {
        public static IConfiguration Configuration { get; set; }

        public static Guid CurrentUserId { get; set; }
        public static Guid SolutionId { get; set; }
        public static Guid CompanyId { get; set; }
        public static string BearerToken { get; set; }
        public static string SolutionKey { get { return Configuration.GetSection("RDK:SolutionKey").Value; } }
        public static string EnvironmentCode { get { return Configuration.GetSection("RDK:EnvCode").Value; } }

        public static string Culture { get { return "en-Us"; } }
        public static char EnableDebugInfo { get { return '0'; } }
        public static string Version { get; set; } = "v1";

        public static string UploadBasePath { get; set; } = "wwwroot/";
        public static string CropImageBasePath { get; set; } = "CropImages/";
        public static string GreenHouseImageBasePath { get; set; } = "GreenHouseImages/";
    }
}
