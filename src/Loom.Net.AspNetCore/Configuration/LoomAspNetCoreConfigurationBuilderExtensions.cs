using Loom.Net.AspNetCore.Exceptions;
using Loom.Net.AspNetCore.Models;
using Microsoft.Extensions.Configuration;

namespace Loom.Net.AspNetCore.Configuration
{
    public static class LoomAspNetCoreConfigurationBuilderExtensions
    {
        extension (LoomAspNetCoreConfigurationBuilder builder)
        {
            public LoomAspNetCoreConfigurationBuilder LoadFromConfiguration(IConfiguration configuration)
            {
                var section = configuration.GetSection("Loom");

                var loomConfiguration = section.Get<LoomJsonConfiguration>() 
                    ?? throw new LoomAspNetCoreInvalidOperationException($"Failed to load Loom configuration from section '{section.Path}'");

                builder.RoutePrefix = loomConfiguration.RoutePrefix;

                foreach (var (name, host) in loomConfiguration.Backends)
                {
                    builder.AddHost(name, host);
                }

                foreach (var route in loomConfiguration.Routes)
                {
                    builder.Routes.Add(route);
                }

                return builder;
            }

            internal LoomAspNetCoreConfigurationBuilder AddHost(string name, LoomJsonBackend jsonHost)
            {
                var host = new LoomBackend(
                    jsonHost.Host,
                    jsonHost.Port
                );

                builder.Backends.Add(name, host);

                return builder;
            }
        }
    }
}
