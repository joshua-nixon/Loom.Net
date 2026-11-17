namespace Loom.Net.AspNetCore.Models
{
    internal sealed record LoomJsonConfiguration(
        IReadOnlyList<LoomJsonRoute> Routes,
        IReadOnlyDictionary<string, LoomJsonBackend> Backends,
        string RoutePrefix = ""
    );

    internal readonly record struct LoomJsonBackend(string Host, int Port);

    internal readonly record struct LoomJsonRoute(LoomJsonRouteUpstream Upstream, string PathTemplate);

    internal readonly record struct LoomJsonRouteUpstream(string Name, string PathTemplate);

}
