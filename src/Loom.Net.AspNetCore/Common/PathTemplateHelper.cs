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

        public static Regex CompilePathTemplate(string pathTemplate)
        {
            // Replace the basic group i.e {user-id} with a named group that will match
            // any non-slash characters, e.g. (?<user-id>[^/]+)

            pathTemplate = NormalizePathParameters(pathTemplate);

            pathTemplate = GroupCaptureRegex.Replace(pathTemplate, "(?<$1>[^/]+)");

            return new Regex(
                $@"\A{pathTemplate}\z",
                RegexOptions.Compiled | RegexOptions.IgnoreCase,
                TimeSpan.FromSeconds(1)
            );
        }
    }
}
