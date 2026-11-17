using System.Text;
using System.Text.RegularExpressions;

namespace Loom.Net.AspNetCore.Common
{
    internal static partial class PathTemplateHelper
    {
        /// <summary>
        /// Captures patterns within braces, e.g. {user-id} in the path template /users/{user-id}
        /// </summary>
        [GeneratedRegex(@"\{([^}]+)\}")]
        private static partial Regex GroupCaptureRegex { get; }

        /// <summary>
        /// Normalizes path parameters by replacing hyphens with underscores.
        /// </summary>
        public static string NormalizePathParameters(string pathTemplate)
        {
            // C# does not allow hyphens in named groups,
            // so we need to replace any hyphens with underscores.

            return GroupCaptureRegex.Replace(pathTemplate, m =>
            {
                var paramName = m.Groups[1].Value;
                var validGroupName = paramName.Replace("-", "_");
                return $"{{{validGroupName}}}";
            });
        }

        /// <summary>
        /// Compiles a path template into a case-insensitive regex with named captures for its parameters.
        /// </summary>
        public static Regex CompilePathTemplate(string pathTemplate)
        {
            pathTemplate = NormalizePathParameters(pathTemplate);

            var patternBuilder = new StringBuilder(@"\A");
            var lastTemplateIndex = 0;

            foreach (Match placeholder in GroupCaptureRegex.Matches(pathTemplate))
            {
                // Escape literal text so regex characters in the template match themselves.
                patternBuilder.Append(Regex.Escape(pathTemplate[lastTemplateIndex..placeholder.Index]));

                // Each placeholder becomes a named capture for one non-empty path segment.
                patternBuilder
                    .Append("(?<")
                    .Append(placeholder.Groups[1].Value)
                    .Append(@">[^/]+)");

                lastTemplateIndex = placeholder.Index + placeholder.Length;
            }

            // Escape the final literal segment and require the regex to consume the entire path.
            patternBuilder
                .Append(Regex.Escape(pathTemplate[lastTemplateIndex..]))
                .Append(@"\z");

            return new Regex(
                patternBuilder.ToString(),
                RegexOptions.Compiled | RegexOptions.IgnoreCase,
                TimeSpan.FromSeconds(1)
            );
        }
    }
}
