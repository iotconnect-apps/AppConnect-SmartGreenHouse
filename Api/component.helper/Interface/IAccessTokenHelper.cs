using System.Threading.Tasks;

namespace component.helper.Interface
{
    public interface IAccessTokenHelper
    {
        Task<string> GetAccessTokenAsync();
    }
}
