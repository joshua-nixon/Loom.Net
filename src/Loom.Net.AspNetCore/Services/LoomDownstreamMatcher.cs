using Loom.Net.AspNetCore.Configuration;
using Loom.Net.AspNetCore.Exceptions;
using Loom.Net.AspNetCore.Extensions;
using Loom.Net.AspNetCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Loom.Net.AspNetCore.Services
{
    internal sealed class LoomDownstreamMatcher(
        LoomAspNetCoreConfiguration configuration,
        ILogger<LoomDownstreamMatcher> logger)
    {
        public MatchedDownstreamRoute GetDownstream(HttpContext context)
        {
            var loomRoute = GetLoomRoute(context);

            var downstream = CreateMatchedDownstreamRoute(context, loomRoute);

            logger.LogMatchedRoute(context.Request.Path, downstream.DownstreamUri);

            return downstream;
        }

        private LoomRoute GetLoomRoute(HttpContext context)
        {
            // Unsure what scenario this would occur, but its easy enough
            // to check and handle the rare case.
            if (!context.Request.Path.HasValue)
            {
                throw new LoomAspNetCoreInvalidOperationException("Cannot proceed as HttpContext.Request.Path is not set");
            }

            var matchablePath = context.Request.Path.Value;

            // Strip the prefix.
            // Only need to perform a substring if we have a non-empty prefix
            // and if the path actually starts with the prefix - which it should if  we made it this far.
            if (!string.IsNullOrWhiteSpace(configuration.RoutePrefix) &&
                matchablePath.StartsWith(configuration.RoutePrefix))
            {
                matchablePath = matchablePath[configuration.RoutePrefix.Length..];
            }

            return configuration.Routes.Single(x => x.PathTemplate == matchablePath);           
        }

        private MatchedDownstreamRoute CreateMatchedDownstreamRoute(HttpContext context, LoomRoute route)
        {
            var downstreamPath = $"{route.Scheme}://{route.Backend.Host}:{route.Backend.Port}/{route.UpstreamPathTemplate}";

            // Optionally add the query string to the downstream path.
            if (context.Request.QueryString.HasValue)
            {
                downstreamPath += context.Request.QueryString.Value;
            }

            var downstreamUri = new Uri(downstreamPath);

            return new MatchedDownstreamRoute(context, downstreamUri);
        }
    }
}
