using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Loom.Net.AspNetCore.Extensions
{
    internal static partial class LoggerExtensions
    {
        [LoggerMessage(
            Level = LogLevel.Information,
            Message = "Matched route '{RequestPath}' to '{DownstreamUri}'"
        )]
        public static partial void LogMatchedRoute(this ILogger logger, PathString requestPath, Uri downstreamUri);

        [LoggerMessage(
            Level = LogLevel.Information,
            Message = "No matching route found for '{RequestPath}'"
        )]
        public static partial void LogUnmatchedRoute(this ILogger logger, PathString requestPath);
    }
}
