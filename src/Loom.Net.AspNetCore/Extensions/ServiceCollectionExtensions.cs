using Loom.Net.AspNetCore.Configuration;
using Loom.Net.AspNetCore.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Loom.Net.AspNetCore.Extensions
{
    public static class ServiceCollectionExtensions
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AddLoom(Action<LoomAspNetCoreConfigurationBuilder> configure)
            {
                var builder = new LoomAspNetCoreConfigurationBuilder();

                configure(builder);

                var options = builder.Build();

                services.AddCoreServices();

                services.TryAddSingleton(options);

                return services;
            }

            private void AddCoreServices()
            {
                services.TryAddSingleton<LoomDownstreamMatcher>();
                services.TryAddSingleton<LoomHttpForwarder>();
            }
        }
    }
}
