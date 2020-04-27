using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Container.Updater.Controllers.CustomApiKeyAuth;
using Container.Updater.Domain;
using Container.Updater.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

public class UpdateRequestController
{
    private readonly IOptions<AzureAuthentication> _settings;
    private readonly CustomApiKeyAuth _apiKeyAuth;

    public UpdateRequestController(IOptions<AzureAuthentication> settings, CustomApiKeyAuth apiKeyAuth)
    {
        _settings = settings;
        _apiKeyAuth = apiKeyAuth;
    }

    [FunctionName("UpdateRequest")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "update")]HttpRequest req,
        ILogger log)
    {
        if (!_apiKeyAuth.Validate(req))
            return new UnauthorizedResult();

        var azureResources = new AzureResources(_settings);

        var request = JsonConvert.DeserializeObject<UpdateRequest>(await new StreamReader(req.Body).ReadToEndAsync());

        var containerResources = await azureResources.GetAzureWebAppsWithContainers();

        var matchingResources = containerResources.Where(x => x.Image == request.GetFullImageUri());

        foreach (var resourceWithContainer in matchingResources)
        {
            log.LogInformation($"Updating resource {resourceWithContainer.ResourceId} with matching container image {resourceWithContainer.Image}");
            await resourceWithContainer.ForceImageToUpdate();
        }

        return new OkObjectResult(matchingResources.Select(x => new ImageUpdateResult
        {
            Image = x.Image,
            TimeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            ResourceId = x.ResourceId,
            Message = "Image updated by stopping and then starting application"
        }));
    }
}