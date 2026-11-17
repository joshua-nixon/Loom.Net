using System.Text.RegularExpressions;
using Loom.Net.AspNetCore.Exceptions;
using Loom.Net.AspNetCore.Models;
using Microsoft.Extensions.FileSystemGlobbing.Internal;

namespace Loom.Net.AspNetCore.Configuration
{
    public sealed partial class LoomAspNetCoreConfigurationBuilder
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
                        route.Upstream.PathTemplate,
                        backend,
                        CompilePathTemplate(route.PathTemplate)
                    )
                );
            }

            return routes;
        }

        private static string NormalisePathTemplate(string pathTemplate)
        {
            // C# does not allow hyphens in named groups,
            // so we need to replace any hyphens with underscores.
            return GroupCaptureRegex.Replace(pathTemplate, m =>
            {
                var paramName = m.Groups[1].Value;

                var validGroupName = paramName.Replace("-", "_");

                return $"(?<{validGroupName}>[^/]+)";
            });
        }

        private static Regex CompilePathTemplate(string pathTemplate)
        {
            pathTemplate = NormalisePathTemplate(pathTemplate);

            // Replace the basic group i.e {user-id} with a named group that will match
            // any non-slash characters, e.g. (?<user-id>[^/]+)
            pathTemplate = GroupCaptureRegex.Replace(pathTemplate, "(?<$1>[^/]+)");

            return new Regex(
                pathTemplate, 
                RegexOptions.Compiled | RegexOptions.IgnoreCase, 
                TimeSpan.FromSeconds(1)
            );
        }

        /// <summary>
        /// Captures patterns within parenthesis, e.g. {user-id} in the path template /users/{user-id}
        /// </summary>
        [GeneratedRegex(@"\{([^}]+)\}")]
        private static partial Regex GroupCaptureRegex { get; }
    }
}