using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldCupStatistics.DataLayer.Settings;

namespace WorldCupStatistics.DataLayer.Services
{
    public interface ISettingsService
    {
        string SettingsFilePath { get; }
        bool Exists { get; }

        /// <summary>Loads settings, or null if the file is missing/unreadable (so the app can re-prompt).</summary>
        AppSettings? Load();

        void Save(AppSettings settings);
    }
}
