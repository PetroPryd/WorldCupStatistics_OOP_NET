using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WorldCupStatistics.DataLayer.Mapping
{
    internal static partial class TextHelpers
    {
        [GeneratedRegex(@"\s+")]
        private static partial Regex Whitespace();

        public static string Normalize(string? value)
            => string.IsNullOrWhiteSpace(value) ? string.Empty : Whitespace().Replace(value.Trim(), " ");

        public static int ParseAttendance(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return 0;
            var digits = new string(value.Where(char.IsDigit).ToArray());
            return int.TryParse(digits, out var n) ? n : 0;
        }
    }
}
