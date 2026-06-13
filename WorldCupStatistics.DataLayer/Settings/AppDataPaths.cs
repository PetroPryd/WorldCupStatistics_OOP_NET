using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldCupStatistics.DataLayer.Settings
{
    internal static class AppDataPaths
    {
        public static string Root => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "WorldCupStatistics");

        public static string SettingsFile => Path.Combine(Root, "settings.txt");
        public static string ImagesRoot => Path.Combine(Root, "Images");

        public static void EnsureRoot() => Directory.CreateDirectory(Root);
    }
}
