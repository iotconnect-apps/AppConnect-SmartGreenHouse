using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;

namespace component.discoveryclient
{
    public static class DiscoveryClient
    {
        public static DiscoveryModel Get(string url)
        {
            var discoveryModel = new DiscoveryModel() { HasError = true };
            using (var httpClient = new HttpClient())
            {
                try
                {
                    using (var response = httpClient.GetAsync(url)?.Result)
                    {
                        if (response != null && (int)response.StatusCode == (int)HttpStatusCode.OK)
                        {
                            return JsonConvert.DeserializeObject<DiscoveryModel>(response.Content.ReadAsStringAsync().Result);
                        }
                        discoveryModel.Error = $"Discovery failed with status code : {response.StatusCode.ToString()} for url : {url}";
                    }
                }
                catch (Exception ex)
                {
                    discoveryModel.Error = $"Discovery failed with exception : {ex.Message} for url : {url}";
                }
            }
            return discoveryModel;
        }
    }
}
