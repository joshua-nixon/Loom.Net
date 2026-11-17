using System.Text.RegularExpressions;

namespace Loom.Net.AspNetCore.Models
{
    internal readonly record struct LoomRoute(
        string UpstreamPathTemplate,
        LoomBackend Backend,
        Regex CompiledPathTemplate
    )
    {
        public string Scheme { get; } = "http";
    }

    internal readonly record struct LoomBackend(string Host, int Port);
}
