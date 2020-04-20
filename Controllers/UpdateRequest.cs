using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Container.Updater.Domain;
using Container.Updater.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

public static class UpdateRequestController
{
    [FunctionName("UpdateRequest")]
    public static async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "post", Route = "update")]
    HttpRequest req, ILogger log, IOptions<AzureAuthentication> settings)
    {
        var azureResources = new AzureResources(settings);

        var request = JsonConvert.DeserializeObject<UpdateRequest>(await new StreamReader(req.Body).ReadToEndAsync());

        var containerResources = await azureResources.GetAzureWebAppsWithContainers();

        var matchingResources = containerResources.Where(x => x.Image == request.GetFullImageUri());

        foreach (var resourceWithContainer in matchingResources)
        {
            log.LogInformation($"Updating resource {resourceWithContainer.ResourceId} with matching container image {resourceWithContainer.Image}");
            await resourceWithContainer.ForceImageToUpdate();
        }

        return new OkObjectResult(matchingResources.Select(x => new ImageUpdateResult()));
    }
}