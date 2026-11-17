using Loom.Net.AspNetCore.Configuration;
using Loom.Net.AspNetCore.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

namespace Loom.Net.AspNetCore.Tests.Helpers
{
    internal static class LoomDownstreamMatcherHelper
    {
        public static LoomDownstreamMatcher CreateFromRoutes(params (string PathTemplate, string UpstreamPathTemplate)[] routes)
        {
            var configurationValues = new Dictionary<string, string?>
            {
                ["Loom:RoutePrefix"] = "api",
                ["Loom:Backends:backend:Host"] = "localhost",
                ["Loom:Backends:backend:Port"] = "8080",
            };

            for (var index = 0; index < routes.Length; index++)
            {
                configurationValues[$"Loom:Routes:{index}:PathTemplate"] = routes[index].PathTemplate;
                configurationValues[$"Loom:Routes:{index}:Upstream:Name"] = "backend";
                configurationValues[$"Loom:Routes:{index}:Upstream:PathTemplate"] = routes[index].UpstreamPathTemplate;
            }

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationValues)
                .Build();

            var builder = new LoomAspNetCoreConfigurationBuilder();

            builder.LoadFromConfiguration(configuration);

            return new LoomDownstreamMatcher(builder.Build(), NullLogger<LoomDownstreamMatcher>.Instance);
        }
    }
}
