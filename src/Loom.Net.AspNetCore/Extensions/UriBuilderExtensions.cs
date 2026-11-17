using System.Web;
using Microsoft.AspNetCore.Http;

namespace Loom.Net.AspNetCore.Extensions
{
    internal static class UriBuilderExtensions
    {
        public static UriBuilder MergeQueryString(this UriBuilder builder, IQueryCollection requestQuery)
        {
            if (requestQuery.Count == 0)
            {
                return builder;
            }

            var downstreamQuery = HttpUtility.ParseQueryString(builder.Query);

            foreach (var (key, stringValues) in requestQuery)
            {
                // Unsure if we should use a case insensitive comparison here,
                // but it seems like the safest option.
                if (downstreamQuery.AllKeys.Contains(key, StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }

                foreach (var value in stringValues)
                {
                    downstreamQuery.Add(key, value);
                }
            }

            builder.Query = downstreamQuery.ToString();

            return builder;

        }
    }
}
