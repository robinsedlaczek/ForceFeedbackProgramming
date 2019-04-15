using System.Collections.Generic;

namespace ForceFeedback.Adapters.VisualStudio
{
    public static class StringExtensions
    {
        private static readonly HashSet<string> NewLineMarker = new HashSet<string>
        {
            "\r\n", "\r", "\n"
        };

        public static bool IsNewLineMarker(this string value)
        {
            return value != null && NewLineMarker.Contains(value);
        }
    }
}