using System.Text.RegularExpressions;
using Loom.Net.AspNetCore.Common;
using Loom.Net.AspNetCore.Exceptions;
using Loom.Net.AspNetCore.Models;

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
                        PathTemplateHelper.NormalizePathParameters(route.Upstream.PathTemplate),
                        backend,
                        CompilePathTemplate(PathTemplateHelper.NormalizePathParameters(route.PathTemplate))
                    )
                );
            }

            return routes;
        }

        private static Regex CompilePathTemplate(string normlizedPathTemplate)
        {
            // Replace the basic group i.e {user-id} with a named group that will match
            // any non-slash characters, e.g. (?<user-id>[^/]+)

            normlizedPathTemplate = GroupCaptureRegex.Replace(normlizedPathTemplate, "(?<$1>[^/]+)");

            return new Regex(
                normlizedPathTemplate, 
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