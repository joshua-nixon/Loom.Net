using Loom.Net.AspNetCore.Models;

namespace Loom.Net.AspNetCore.Configuration
{
    internal sealed class LoomAspNetCoreConfiguration
    {
        public readonly string RoutePrefix;
        public readonly string RoutePattern;
        public readonly IReadOnlyList<LoomRoute> Routes;

        public LoomAspNetCoreConfiguration(string routePrefix, List<LoomRoute> routes)
        {
            RoutePrefix = NormalizeRoutePrefix(routePrefix);

            RoutePattern = RoutePrefix.Length == 0
                ? "/{**catch-all}"
                : $"{RoutePrefix}{{**catch-all}}";

            Routes = routes;
        }

        private static string NormalizeRoutePrefix(string routePrefix)
        {
            var normalizedPrefix = routePrefix.Trim('/');

            if (normalizedPrefix.Length == 0)
            {
                return string.Empty;
            }

            return $"/{normalizedPrefix}/";
        }
    }
}
