using Loom.Net.AspNetCore.Common;
using Loom.Net.AspNetCore.Exceptions;
using Loom.Net.AspNetCore.Models;

namespace Loom.Net.AspNetCore.Configuration
{
    public sealed class LoomAspNetCoreConfigurationBuilder
    {
        public string RoutePrefix { get; internal set; } = string.Empty;

        internal Dictionary<string, LoomBackend> Backends { get; } = [];

        internal List<LoomJsonRoute> Routes { get; } = [];

        internal LoomAspNetCoreConfiguration Build()
        {
            return new LoomAspNetCoreConfiguration(
                RoutePrefix,
                ProcessRoutes()
            );
        }

        private List<LoomRoute> ProcessRoutes()
        {
            List<LoomRoute> routes = [];

            foreach (var route in Routes)
            {
                if (!Backends.TryGetValue(route.Upstream.Name, out var backend))
                {
                    throw new LoomAspNetCoreInvalidOperationException($"Host configuration not found for route '{route.PathTemplate}' with host '{route.Upstream.Name}'");
                }

                routes.Add(
                    new LoomRoute(
                        PathTemplateHelper.NormalizePathParameters(route.Upstream.PathTemplate),
                        backend,
                        PathTemplateHelper.CompilePathTemplate(route.PathTemplate)
                    )
                );
            }

            return routes;
        }
    }
}