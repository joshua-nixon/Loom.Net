using Loom.Net.AspNetCore.Services;
using Microsoft.AspNetCore.Http;

namespace Loom.Net.AspNetCore.Endpoints
{
    internal static class CatchEndpointHandler
    {
        public static async ValueTask HandleAsync(
            HttpContext context,
            LoomHttpForwarder forwarder,
            LoomDownstreamMatcher matcher)
        {
            var downstream = matcher.GetDownstream(context);

            await forwarder.ForwardAsync(downstream);
        }
    }
}