using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using azure_container_updater.Controllers;
using Container.Updater.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Container.Updater.Controllers
{
    [ApiController]
    public class UpdateRequestController : ControllerBase
    {
        private readonly AzureResources _azureResources;

        public UpdateRequestController(AzureResources azureResources)
        {
            _azureResources = azureResources;
        }

        [HttpPost("/api/update")]
        [Authorize(AuthenticationSchemes = "ApiKey")]
        public async Task<ActionResult<ImageUpdateResult>> UpdateImage([Required][FromBody] UpdateRequest request)
        {
            var containerResources = await _azureResources.GetAzureWebAppsWithContainers();

            var matchingResources = containerResources.Where(x => x.Image == request.GetFullImageUri());

            foreach(var resourceWithContainer in matchingResources)
            {
                await resourceWithContainer.ForceImageToUpdate();
            }

            return Ok(matchingResources.Select(x => new ImageUpdateResult()));
        }
    }
}
