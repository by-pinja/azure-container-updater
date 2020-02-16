using System.ComponentModel.DataAnnotations;

namespace Container.Updater.Options
{
    public class AzureAuthentication
    {
        [Required(AllowEmptyStrings = false)]
        public string TenantId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string SubscriptionId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string ClientId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string ClientSecret { get; set; }
    }
}