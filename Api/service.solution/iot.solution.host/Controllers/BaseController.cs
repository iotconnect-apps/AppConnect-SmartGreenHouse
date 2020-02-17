using component.common.model;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace host.iot.solution.Controllers
{
    [ProducesResponseType(typeof(UnauthorizeError), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ModelStateError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(GenericError), (int)HttpStatusCode.InternalServerError)]
    [ProducesResponseType(typeof(NotFoundError), (int)HttpStatusCode.NotFound)]
    [ApiController]
    [ApiVersionNeutral]
    [Authorize]
    public class BaseController : ControllerBase
    {
        public BaseController()
        {

        }

        public string CurrentUserId
        {
            get
            {
                return User.Identity.Name;
            }
        }
    }
}
