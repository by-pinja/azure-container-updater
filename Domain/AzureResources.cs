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
        private IAzure _azure;

        public AzureResources(IOptions<AzureAuthentication> authOptions)
        {
            var servicePrincipal = new ServicePrincipalLoginInformation()
            {
                ClientId = authOptions.Value.ClientId.ToString(),
                ClientSecret = authOptions.Value.ClientSecret
            };

            var foo = new AzureCredentials(servicePrincipal, authOptions.Value.TenantId.ToString(), AzureEnvironment.AzureGlobalCloud);

            _azure = Azure.Authenticate(foo).WithSubscription(authOptions.Value.SubscriptionId.ToString());
        }

        public async Task<IEnumerable<object>> GetResourcesWithContainers()
        {
            var foo = await _azure.WebApps.ListAsync();
            return null;
        }

        public void UpdateResource(AzureResource resource)
        {
        }
    }
}