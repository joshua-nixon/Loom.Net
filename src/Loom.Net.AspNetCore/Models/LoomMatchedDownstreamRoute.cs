using Microsoft.AspNetCore.Http;

namespace Loom.Net.AspNetCore.Models
{
    internal sealed record LoomMatchedDownstreamRoute(HttpContext Context, Uri DownstreamUri);
}
