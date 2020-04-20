using System.ComponentModel.DataAnnotations;

namespace Container.Updater.Options
{
    public class AzureAuthentication
    {
        public bool ManualAuthentication { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string SubscriptionId { get; set; }
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}