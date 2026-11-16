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
    }
}
