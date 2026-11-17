using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;
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
        public bool TryGetDownstream(HttpContext context, [NotNullWhen(true)] out LoomMatchedDownstreamRoute? route)
        {
            route = default;

            // Unsure what scenario this would occur, but its easy enough
            // to check and handle the rare case.
            if (!context.Request.Path.HasValue)
            {
                throw new LoomAspNetCoreInvalidOperationException("Cannot proceed as HttpContext.Request.Path is not set");
            }

            var matchablePath = GetNormalisedRequestPath(context);

            if (!TryGetMatchedRoute(matchablePath, out var foundRoute, out var foundMatch)) 
            {
                return false;
            }

            route = CreateMatchedDownstreamRoute(context, foundRoute, foundMatch);

            logger.LogMatchedRoute(context.Request.Path, route.DownstreamUri);

            return true;
        }

        private bool TryGetMatchedRoute(
            string path, 
            out LoomRoute matched,
            [NotNullWhen(true)] out Match? match)
        {
            LoomRoute? foundRoute = null;

            match = null;

            // Since we check for duplicate routes, each request will iterate over the full array.
            // Can this be optimised?

            foreach (var route in configuration.Routes)
            {
                // Careful! Match is set before foundRoute is set, so if an exception is raised
                // the link between match and route will not be correct.
                match = route.CompiledPathTemplate.Match(path);

                if (!match.Success)
                {
                    continue;
                }

                // If we have already matched a route, then we have a conflict and should throw an exception.
                if (foundRoute.HasValue)
                {
                    throw new LoomAspNetCoreInvalidOperationException($"Multiple routes matched the path '{path}'. Please ensure that your route templates are unique.");
                }

                foundRoute = route;
            }

            matched = foundRoute.GetValueOrDefault();

            return foundRoute.HasValue;
        }

        private string GetNormalisedRequestPath(HttpContext context)
        {
            var matchablePath = context.Request.Path.Value!;

            // Strip the prefix.
            // Only need to perform a substring if we have a non-empty prefix
            // and if the path actually starts with the prefix - which it should if  we made it this far.
            if (!string.IsNullOrWhiteSpace(configuration.RoutePrefix) &&
                matchablePath.StartsWith(configuration.RoutePrefix))
            {
                matchablePath = matchablePath[configuration.RoutePrefix.Length..];
            }

            return matchablePath;
        }

        private static LoomMatchedDownstreamRoute CreateMatchedDownstreamRoute(
            HttpContext context, 
            LoomRoute route, 
            Match capturedMatch)
        {
            var downstreamPath = CreateDownstreamUrl(route, capturedMatch);

            // Optionally add the query string to the downstream path.
            if (context.Request.QueryString.HasValue)
            {
                downstreamPath += context.Request.QueryString.Value;
            }

            var downstreamUri = new Uri(downstreamPath);

            return new LoomMatchedDownstreamRoute(context, downstreamUri);
        }

        private static string CreateDownstreamUrl(LoomRoute route, Match capturedMatch)
        {
            var stringBuilder = new StringBuilder(route.UpstreamPathTemplate);

            foreach (var grp in capturedMatch.Groups.Cast<Group>())
            {
                // Skip the 0th group, which is the entire match, and any unnamed groups.
                if (!grp.Success || string.IsNullOrWhiteSpace(grp.Name) || grp.Name == "0")
                {
                    continue;
                }

                var groupName = grp.Name.Replace("_", "-");

                stringBuilder.Replace($"{{{groupName}}}", grp.Value);
            }

            return $"{route.Scheme}://{route.Backend.Host}:{route.Backend.Port}/{stringBuilder}";
        }
    }
}