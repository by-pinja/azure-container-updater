using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Container.Updater.Controllers.CustomApiKeyAuth
{
    // We don't use standard azure function api key authentication here because we wan't that same client
    // can be used constintly to call all container updaters through without implementing special cases
    // for each cloud and platform.
    // For example client can just call "https://api-in-azure..., https://api-in-k8s-google...,  https://api-in-azure" to make sure that are
    // platforms are constistenlty updates with latest images instead of impelementing each case separately.
    public class CustomApiKeyAuth
    {
        private readonly IOptions<ApiAuthSettings> _settings;
        private readonly ILogger _logger;

        public CustomApiKeyAuth(IOptions<ApiAuthSettings> settings, ILogger logger)
        {
            _logger = logger;
            _settings = settings;
        }

        public bool Validate(HttpRequest httpRequest)
        {
            var matchingHeader = httpRequest.Headers.FirstOrDefault(x => x.Key == "Authorization");

            if(matchingHeader.Equals(default) || matchingHeader.Value.Count() != 1)
            {
                _logger.LogDebug($"Authorization failed because no matching header found, available headers: {string.Join(", ", httpRequest.Headers.Select(x => x.Key))}");
                return false;
            }

            var tokens = matchingHeader.Value.First().Split(" ");

            if(tokens.Length != 2 || !tokens[0].Equals("apikey", StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.LogDebug("Authorization failed because apikey didn't match 'ApiKey yourkey' pattern.");
                return false;
            }

            if(tokens[1] != _settings.Value.ApiKey)
            {
                _logger.LogDebug("Authorization failed because apikey were invalid.");
                return false;

            }

            return true;
        }
    }
}