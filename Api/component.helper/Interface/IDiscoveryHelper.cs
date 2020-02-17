using System.Threading.Tasks;

namespace component.helper.Interface
{
    public interface IDiscoveryHelper
    {
        Task<string> GetEndpointUrl(string apiRoute, string apiKey);
    }
}
