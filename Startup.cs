using Container.Updater;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Container.Updater.Options;
using Microsoft.Extensions.Configuration;
using Container.Updater.Controllers.CustomApiKeyAuth;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Container.Updater
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder
                .Services
                .AddOptions<AzureAuthentication>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("AzureAuthentication").Bind(settings);
                });

            builder
                .Services
                .AddOptions<ApiAuthSettings>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.Bind(settings);
                });

            builder.Services.AddScoped<CustomApiKeyAuth>();
        }
    }
}