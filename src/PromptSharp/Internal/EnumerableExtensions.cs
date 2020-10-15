using System.Collections.Generic;

namespace PromptSharp.Internal
{
    internal static class EnumerableExtensions
    {
        public static string Join(this IEnumerable<string> source, string separator) => string.Join(separator, source);
    }
}
