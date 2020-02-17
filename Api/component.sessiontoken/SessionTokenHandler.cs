using Microsoft.AspNetCore.Http;
using System.Linq;


namespace component.sessiontoken
{
    public class SessionTokenHandler : ISessionTokenHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SessionTokenInfo SessionToken { get; }

        public SessionTokenHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            SessionToken = _getCurrentSessionToken();
        }

        private SessionTokenInfo _getCurrentSessionToken()
        {
            var sessionTokenInfo = new SessionTokenInfo();
            var claims = ((System.Security.Claims.ClaimsIdentity)_httpContextAccessor.HttpContext.User.Identity).Claims.ToList();

            //sessionTokenInfo.UserId = claims.FirstOrDefault(c => c.Type == ClaimType.UserId)?.Value;

            return sessionTokenInfo;
        }
    }
}
