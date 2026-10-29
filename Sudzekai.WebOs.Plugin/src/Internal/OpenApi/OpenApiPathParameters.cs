using System.Text.RegularExpressions;

namespace Sudzekai.WebOs.Plugin
{
    internal static partial class OpenApiPathParameters
    {
        internal static string NormalizePath(string pattern)
            => "/" + PathParameterRegex().Replace(pattern.TrimStart('/'), "{$1}");

        internal static MatchCollection Find(string pattern)
            => PathParameterRegex().Matches(pattern);

        [GeneratedRegex(@"\{([^}:]+)(?::[^}]*)?\}")]
        private static partial Regex PathParameterRegex();
    }
}