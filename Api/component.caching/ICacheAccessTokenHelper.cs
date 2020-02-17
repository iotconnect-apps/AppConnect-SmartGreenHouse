using System.Threading.Tasks;

namespace component.caching
{
    public interface ICacheAccessTokenHelper
    {
        Task<string> GetAccessTokenAsync();
    }
}
