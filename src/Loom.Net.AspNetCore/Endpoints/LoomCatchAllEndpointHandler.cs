using Loom.Net.AspNetCore.Extensions;
using Loom.Net.AspNetCore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Loom.Net.AspNetCore.Endpoints
{
    internal class LoomCatchAllEndpointHandler
    {
        public static async ValueTask HandleAsync(
            HttpContext context,
            LoomHttpForwarder forwarder,
            LoomDownstreamMatcher matcher,
            ILogger<LoomCatchAllEndpointHandler> logger)
        {
            if (!matcher.TryGetDownstream(context, out var downstream))
            {
                logger.LogUnmatchedRoute(context.Request.Path);

                context.Response.StatusCode = StatusCodes.Status404NotFound;

                return;
            }

            await forwarder.ForwardAsync(downstream);
        }
    }
}