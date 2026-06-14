using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace WorldCupStatistics.WinForms.Localization
{
    internal static class Loc
    {
        private static readonly ResourceManager Manager =
            new("WorldCupStatistics.WinForms.Localization.Strings", typeof(Loc).Assembly);

        public static CultureInfo Culture { get; private set; } = new CultureInfo("en-US");

        public static void ApplyCulture(string language)
        {
            Culture = (language?.ToLowerInvariant()) switch
            {
                "hr" => new CultureInfo("hr-HR"),
                _ => new CultureInfo("en-US")
            };
            CultureInfo.CurrentCulture = Culture;
            CultureInfo.CurrentUICulture = Culture;
            CultureInfo.DefaultThreadCurrentCulture = Culture;
            CultureInfo.DefaultThreadCurrentUICulture = Culture;
        }

        public static string T(string key) =>
            Manager.GetString(key, Culture) ?? key;
    }
}
