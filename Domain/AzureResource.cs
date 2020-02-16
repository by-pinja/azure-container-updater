using System.Threading.Tasks;
using Microsoft.Azure.Management.AppService.Fluent;

namespace Container.Updater.Domain
{
    public class AzureContainerResource
    {
        private readonly IWebApp _app;

        public AzureContainerResource(IWebApp app)
        {
            _app = app;
        }

        public string Image => _app.LinuxFxVersion.Replace("DOCKER|", "");

        public async Task ForceImageToUpdate()
        {
            await _app.StopAsync();
            await _app.StartAsync();
        }
    }
}