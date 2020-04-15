namespace iot.solution.entity.Structs.Routes
{
    public struct GatewayRoute
    {
        public struct Name
        {
            public const string Add = "gateway.add";
            public const string GetList = "gateway.list";
            public const string GetById = "gateway.getdevicebyid";
            public const string Delete = "gateway.deletedevice";
            public const string BySearch = "gateway.search";
            public const string UpdateStatus = "gateway.updatestatus";
            public const string AllChildDevice = "gateway.allchilddevice";
        }

        public struct Route
        {
            public const string Global = "api/gateway";
            public const string Manage = "manage";
            public const string GetList = "";
            public const string GetById = "{id}";
            public const string Delete = "delete/{id}";
            public const string UpdateStatus = "updatestatus/{id}/{status}";
            public const string BySearch = "search";
            public const string AllChildDevice = "allchilddevice";
        }
    }
}
