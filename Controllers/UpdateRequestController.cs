using azure_container_updater.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Container.Updater.Controllers
{
    [ApiController]
    public class UpdateRequestController : ControllerBase
    {
        [HttpPost("/api/update")]
        [Authorize(AuthenticationSchemes = "ApiKey")]
        public ActionResult<ImageUpdateResult> UpdateImage()
        {
            return Ok();
        }
    }
}
