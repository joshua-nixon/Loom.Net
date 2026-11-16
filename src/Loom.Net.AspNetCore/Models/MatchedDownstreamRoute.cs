using Microsoft.AspNetCore.Http;

namespace Loom.Net.AspNetCore.Models
{
    internal sealed record MatchedDownstreamRoute(HttpContext Context, Uri DownstreamUri);
}
