namespace iot.solution.entity.Structs.Routes
{
    public struct LookupRoute
    {
        public struct Name
        {
            public const string Get = "lookup.get";
            public const string GetTemplate = "lookup.template";
            public const string GetTagLookup = "lookup.attributes";
            public const string GetTemplateCommands = "lookup.GetTemplateCommands";
        }

        public struct Route
        {
            public const string Global = "api/lookup";
            public const string Get = "{type}/{param?}";
            public const string GetTemplate = "template";
            public const string GetTagLookup = "attributes/{templateId}";
            public const string GetTemplateCommands = "commands/{templateId}";
        }
    }
}
