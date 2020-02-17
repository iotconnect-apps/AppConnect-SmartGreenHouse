using System.Collections.Generic;

namespace component.swashbuckle
{
    public class ApiAuthentication
    {
        public string ClientId { get; set; }
        public string Realm { get; set; }
        public string ApiName { get; set; }
        public string AuthorityUrl { get; set; }
        public Dictionary<string, string> Scopes { get; set; }
        public Dictionary<string, string> AdditionalQueryStringParams { get; set; }
    }
}
