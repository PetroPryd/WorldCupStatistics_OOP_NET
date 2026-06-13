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

        /// <summary>Sets the UI + formatting culture for the current and future threads.</summary>
        public static void ApplyCulture(string language)
        {
            var culture = (language?.ToLowerInvariant()) switch
            {
                "hr" => new CultureInfo("hr-HR"),
                _ => new CultureInfo("en-US")
            };
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }

        public static string T(string key) =>
            Manager.GetString(key, CultureInfo.CurrentUICulture) ?? key;
    }
}
