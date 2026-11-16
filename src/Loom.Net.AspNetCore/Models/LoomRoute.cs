namespace Loom.Net.AspNetCore.Models
{
    internal readonly record struct LoomRoute(
        string UpstreamPathTemplate,
        string PathTemplate,
        LoomBackend Backend
    )
    {
        public string Scheme { get; } = "http";
    }

    internal readonly record struct LoomBackend(string Host, int Port);
}
