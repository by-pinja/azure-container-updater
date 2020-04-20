using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Container.Updater.Options;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Extensions.Options;

namespace Container.Updater.Domain
{

    public class AzureResources
    {
        private readonly IOptions<AzureAuthentication> _authOptions;

        public AzureResources(IOptions<AzureAuthentication> authOptions)
        {
            _authOptions = authOptions;
        }

        public async Task<IEnumerable<AzureContainerResource>> GetAzureWebAppsWithContainers()
        {
            var azure = GetAzureConnection();

            var webApps = await azure.WebApps.ListAsync();

            var linuxContainerApps = webApps
                .Where(x => x.Inner.Kind == "app,linux,container")
                .Select(x => x.Refresh())
                .Select(x => new AzureContainerResource(x))
                .ToList();

            return linuxContainerApps;
        }

        private IAzure GetAzureConnection()
        {
            if (_authOptions.Value.ManualAuthentication)
            {
                var servicePrincipal = new ServicePrincipalLoginInformation()
                {
                    ClientId = _authOptions.Value.ClientId,
                    ClientSecret = _authOptions.Value.ClientSecret
                };

                var azureCred = new AzureCredentials(servicePrincipal, _authOptions.Value.TenantId, AzureEnvironment.AzureGlobalCloud);

                return Azure.Authenticate(azureCred).WithSubscription(_authOptions.Value.SubscriptionId);
            }
            else
            {
                var loginInfo = new MSILoginInformation(MSIResourceType.AppService);
                var credentials = SdkContext.AzureCredentialsFactory.FromMSI(loginInfo, AzureEnvironment.AzureGlobalCloud);
                return Azure.Authenticate(credentials).WithSubscription(_authOptions.Value.SubscriptionId);
            }
        }
    }
}