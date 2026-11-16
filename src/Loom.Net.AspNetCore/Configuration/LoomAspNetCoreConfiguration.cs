using Loom.Net.AspNetCore.Models;

namespace Loom.Net.AspNetCore.Configuration
{
    internal sealed class LoomAspNetCoreConfiguration(
        string routePrefix,
        List<LoomRoute> routes)
    {
        public readonly string RoutePrefix = '/' + routePrefix.TrimEnd('/') + '/';
        public readonly string RoutePattern = '/' + routePrefix.TrimEnd('/') + "/{**catch-all}";

        public readonly IReadOnlyList<LoomRoute> Routes = routes;
    }
}
