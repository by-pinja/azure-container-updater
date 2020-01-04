using System;
using System.ComponentModel.DataAnnotations;

namespace Container.Updater.Options
{
    public class AzureAuthentication
    {
        [Required]
        public string TenantId { get; set; }

        [Required]
        public string SubscriptionId { get; set; }

        [Required]
        public string ClientId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string ClientSecret { get; set; }
    }
}